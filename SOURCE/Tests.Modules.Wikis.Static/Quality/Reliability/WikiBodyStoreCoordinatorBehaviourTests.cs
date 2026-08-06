using App.Modules.Wikis.Domain.Domains.Wikis.Configuration.Implementations;
using App.Modules.Wikis.Domain.Domains.Wikis.Enums;
using App.Modules.Wikis.Domain.Domains.Wikis.Services.BodyStorage;
using App.Modules.Sys.Infrastructure.Services.Contracts;
using App.Modules.Sys.Shared.Domains.Diagnostics;
using App.Modules.Sys.Shared.Models.Enums;
using Tests.Modules.Wikis.Static.Helpers;
using Tests.Modules.Wikis.Static.Quality.Helpers;

namespace Tests.Modules.Wikis.Static.Quality.Reliability
{
    /// <summary>
    /// Behavioural gate for <c>WikiBodyStoreCoordinator</c> (ADR-018N §2.3/§2.4):
    /// proves the primary/mirror policy that the entire "defer the storage
    /// decision" design depends on — primary is authoritative for write and read,
    /// mirrors are write-only and best-effort, and a misconfigured sink fails
    /// loudly rather than silently dropping the body.
    /// </summary>
    /// <remarks>
    /// Uses hand-written fakes rather than a mocking package: the coordinator is
    /// resolved reflectively from the module assemblies (it is internal by
    /// convention), and the seam contracts are small enough that explicit fakes
    /// read more clearly than a mock and add no test dependency.
    /// </remarks>
    [Trait(TestTraits.Mode, TestTraits.Modes.Static)]
    [Trait(TestTraits.Capability, TestTraits.Capabilities.Conventions)]
    public class WikiBodyStoreCoordinatorBehaviourTests
    {
        private static readonly WikiBodyStoreContext Context = new WikiBodyStoreContext
        {
            WikiId = Guid.NewGuid(),
            WikiKey = "docs",
            WikiPageId = Guid.NewGuid(),
            Slug = "a/b/c",
            WikiPageVersionId = Guid.NewGuid(),
            VersionNumber = 1,
            ContentFormatKey = "markdown",
            IsPrivate = true,
        };

        [Fact]
        [Trait(TestTraits.Quality, TestTraits.Iso25010.Reliability.Maturity)]
        public async Task WhenStoring_ThenOnlyThePrimaryResultIsReturned()
        {
            RecordingBodyStore primary = new RecordingBodyStore(WikiBodyStorageSinkKind.Database, "db-locator");
            RecordingBodyStore mirror = new RecordingBodyStore(WikiBodyStorageSinkKind.FileSystem, "fs-locator");

            IWikiBodyStoreCoordinator coordinator = CreateCoordinator(
                Config(WikiBodyStorageSinkKind.Database, mirrors: [WikiBodyStorageSinkKind.FileSystem]),
                primary,
                mirror);

            WikiBodyStoreResult result = await coordinator.StoreBodyAsync(Context, Bytes("hello"));

            // The version row must persist the PRIMARY locator, never a mirror's.
            Assert.Equal("db-locator", result.BodyLocator);
        }

        [Fact]
        [Trait(TestTraits.Quality, TestTraits.Iso25010.Reliability.Maturity)]
        public async Task WhenStoringWithAMirror_ThenBothPrimaryAndMirrorReceiveTheBody()
        {
            RecordingBodyStore primary = new RecordingBodyStore(WikiBodyStorageSinkKind.Database, "db-locator");
            RecordingBodyStore mirror = new RecordingBodyStore(WikiBodyStorageSinkKind.FileSystem, "fs-locator");

            IWikiBodyStoreCoordinator coordinator = CreateCoordinator(
                Config(WikiBodyStorageSinkKind.Database, mirrors: [WikiBodyStorageSinkKind.FileSystem]),
                primary,
                mirror);

            await coordinator.StoreBodyAsync(Context, Bytes("hello"));

            Assert.Equal(1, primary.StoreCount);
            Assert.Equal(1, mirror.StoreCount);
        }

        [Fact]
        [Trait(TestTraits.Quality, TestTraits.Iso25010.Reliability.Maturity)]
        public async Task WhenAMirrorFailsBestEffort_ThenTheStoreStillSucceeds()
        {
            RecordingBodyStore primary = new RecordingBodyStore(WikiBodyStorageSinkKind.Database, "db-locator");
            ThrowingBodyStore mirror = new ThrowingBodyStore(WikiBodyStorageSinkKind.FileSystem);

            IWikiBodyStoreCoordinator coordinator = CreateCoordinator(
                Config(
                    WikiBodyStorageSinkKind.Database,
                    mirrors: [WikiBodyStorageSinkKind.FileSystem],
                    failIfMirrorUnavailable: false),
                primary,
                mirror);

            WikiBodyStoreResult result = await coordinator.StoreBodyAsync(Context, Bytes("hello"));

            // A best-effort mirror failure must not fail the authoring save.
            Assert.Equal("db-locator", result.BodyLocator);
            Assert.Equal(1, primary.StoreCount);
        }

        [Fact]
        [Trait(TestTraits.Quality, TestTraits.Iso25010.Reliability.Maturity)]
        public async Task WhenAMirrorFailsAndMandatory_ThenTheStoreThrows()
        {
            RecordingBodyStore primary = new RecordingBodyStore(WikiBodyStorageSinkKind.Database, "db-locator");
            ThrowingBodyStore mirror = new ThrowingBodyStore(WikiBodyStorageSinkKind.FileSystem);

            IWikiBodyStoreCoordinator coordinator = CreateCoordinator(
                Config(
                    WikiBodyStorageSinkKind.Database,
                    mirrors: [WikiBodyStorageSinkKind.FileSystem],
                    failIfMirrorUnavailable: true),
                primary,
                mirror);

            await Assert.ThrowsAsync<InvalidOperationException>(
                () => coordinator.StoreBodyAsync(Context, Bytes("hello")));
        }

        [Fact]
        [Trait(TestTraits.Quality, TestTraits.Iso25010.Reliability.Maturity)]
        public async Task WhenReading_ThenOnlyThePrimaryIsConsulted()
        {
            RecordingBodyStore primary = new RecordingBodyStore(WikiBodyStorageSinkKind.Database, "db-locator");
            RecordingBodyStore mirror = new RecordingBodyStore(WikiBodyStorageSinkKind.FileSystem, "fs-locator");

            IWikiBodyStoreCoordinator coordinator = CreateCoordinator(
                Config(WikiBodyStorageSinkKind.Database, mirrors: [WikiBodyStorageSinkKind.FileSystem]),
                primary,
                mirror);

            await coordinator.GetBodyBytesAsync(Context, "db-locator");

            Assert.Equal(1, primary.GetCount);
            Assert.Equal(0, mirror.GetCount);
        }

        [Fact]
        [Trait(TestTraits.Quality, TestTraits.Iso25010.Reliability.Maturity)]
        public async Task WhenThePrimarySinkIsNotRegistered_ThenStoreThrows()
        {
            // Configure ObjectStore as primary but only register the Database sink.
            RecordingBodyStore onlyDatabase = new RecordingBodyStore(WikiBodyStorageSinkKind.Database, "db-locator");

            IWikiBodyStoreCoordinator coordinator = CreateCoordinator(
                Config(WikiBodyStorageSinkKind.ObjectStore, mirrors: []),
                onlyDatabase);

            await Assert.ThrowsAsync<InvalidOperationException>(
                () => coordinator.StoreBodyAsync(Context, Bytes("hello")));
        }

        private static IWikiBodyStoreCoordinator CreateCoordinator(
            WikiConfigurationObject configuration,
            params IWikiBodyStore[] sinks)
        {
            Type coordinatorType = AssemblyUnderTest.AllConcreteTypes
                .Single(t => typeof(IWikiBodyStoreCoordinator).IsAssignableFrom(t));

            object instance = Activator.CreateInstance(
                coordinatorType,
                (IEnumerable<IWikiBodyStore>)sinks,
                new FakeAppConfiguration(configuration),
                new NoOpAppLogger())!;

            return (IWikiBodyStoreCoordinator)instance;
        }

        private static WikiConfigurationObject Config(
            WikiBodyStorageSinkKind primary,
            IReadOnlyList<WikiBodyStorageSinkKind> mirrors,
            bool failIfMirrorUnavailable = false)
        {
            return new WikiConfigurationObject
            {
                BodyStoragePrimarySink = primary,
                BodyStorageMirrorSinks = mirrors,
                BodyStorageFailIfMirrorSinkUnavailable = failIfMirrorUnavailable,
            };
        }

        private static ReadOnlyMemory<byte> Bytes(string text)
        {
            return System.Text.Encoding.UTF8.GetBytes(text);
        }

        /// <summary>A fake sink that records calls and returns a fixed locator.</summary>
        private sealed class RecordingBodyStore : IWikiBodyStore
        {
            private readonly string _locator;

            public RecordingBodyStore(WikiBodyStorageSinkKind kind, string locator)
            {
                this.Kind = kind;
                this._locator = locator;
            }

            public WikiBodyStorageSinkKind Kind { get; }

            public int StoreCount { get; private set; }

            public int GetCount { get; private set; }

            public Task<WikiBodyStoreResult> StoreBodyAsync(
                WikiBodyStoreContext context,
                ReadOnlyMemory<byte> body,
                CancellationToken cancellationToken = default)
            {
                this.StoreCount++;
                return Task.FromResult(new WikiBodyStoreResult(this._locator, "hash", body.Length));
            }

            public Task<ReadOnlyMemory<byte>?> GetBodyBytesAsync(
                WikiBodyStoreContext context,
                string bodyLocator,
                CancellationToken cancellationToken = default)
            {
                this.GetCount++;
                ReadOnlyMemory<byte>? bytes = new ReadOnlyMemory<byte>(Array.Empty<byte>());
                return Task.FromResult(bytes);
            }
        }

        /// <summary>A fake sink whose store operation always throws.</summary>
        private sealed class ThrowingBodyStore : IWikiBodyStore
        {
            public ThrowingBodyStore(WikiBodyStorageSinkKind kind)
            {
                this.Kind = kind;
            }

            public WikiBodyStorageSinkKind Kind { get; }

            public Task<WikiBodyStoreResult> StoreBodyAsync(
                WikiBodyStoreContext context,
                ReadOnlyMemory<byte> body,
                CancellationToken cancellationToken = default)
            {
                throw new IOException("Simulated mirror failure.");
            }

            public Task<ReadOnlyMemory<byte>?> GetBodyBytesAsync(
                WikiBodyStoreContext context,
                string bodyLocator,
                CancellationToken cancellationToken = default)
            {
                throw new IOException("Simulated mirror failure.");
            }
        }

        /// <summary>A fake <see cref="IAppConfiguration{T}"/> over a fixed instance.</summary>
        private sealed class FakeAppConfiguration : IAppConfiguration<WikiConfigurationObject>
        {
            private readonly WikiConfigurationObject _value;

            public FakeAppConfiguration(WikiConfigurationObject value)
            {
                this._value = value;
            }

            public WikiConfigurationObject Value => this._value;

            public WikiConfigurationObject GetValueOrDefault() => this._value;
        }

        /// <summary>A no-op <see cref="IAppLogger"/> for tests.</summary>
        private sealed class NoOpAppLogger : IAppLogger
        {
            public void Log(TraceLevel level, string message) { }

            public void LogTrace(string message) { }

            public void LogTrace(string messageTemplate, params object[] args) { }

            public void LogDebug(string message) { }

            public void LogDebug(string messageTemplate, params object[] args) { }

            public void LogInformation(string message) { }

            public void LogInformation(string messageTemplate, params object[] args) { }

            public void LogWarning(string message) { }

            public void LogWarning(string messageTemplate, params object[] args) { }

            public void LogWarning(Exception exception, string message) { }

            public void LogError(string message) { }

            public void LogError(string messageTemplate, params object[] args) { }

            public void LogError(Exception exception, string message) { }

            public void LogError(Exception exception, string messageTemplate, params object[] args) { }

            public void LogCritical(string message) { }

            public void LogCritical(string messageTemplate, params object[] args) { }

            public void LogCritical(Exception exception, string message) { }
        }
    }
}
