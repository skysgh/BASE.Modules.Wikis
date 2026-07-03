using App.Modules.Sys.Infrastructure.Domains.Persistence.Relational.EF.Schema;
using App.Modules.Sys.Infrastructure.Domains.Persistence.Relational.EF.Schema.Implementations;
using App.Modules.Sys.Shared.Domains.Indexes;
using App.Modules.Wikis.Domain.Domains.Wikis.Constants;
using App.Modules.Wikis.Domain.Domains.Wikis.Entities.Implementations;
using Microsoft.EntityFrameworkCore;

namespace App.Modules.Wikis.Infrastructure.Domains.Wikis.Seeding
{
    /// <summary>
    /// Seeds the platform's shipped <see cref="Wiki"/> roots from the closed,
    /// code-defined <see cref="WikiRootKeys"/> set.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The root <i>shells</i> (key + title + description) are a closed set
    /// defined entirely in this module's own code, so per the EF Seed
    /// Determinism Pattern they are eligible for EF <c>HasData()</c>: the row set
    /// is byte-identical between EF design time and the runtime host. Identity is
    /// deterministic via <see cref="DeterministicGuid.FromString(string, int, string?)"/>
    /// keyed on <c>wiki:{key}</c>, and the base-class audit columns
    /// (<c>CreatedOnDateTimeUtc</c>, <c>RecordState</c>, principals) come from the
    /// fixed framework defaults on the entity base, never from
    /// <c>DateTimeOffset.UtcNow</c> or <c>Guid.NewGuid()</c>.
    /// </para>
    /// <para>
    /// Page <i>content</i> is deliberately NOT seeded here: a page body is held
    /// behind the sink-agnostic body-store coordinator (ADR-018N), which is
    /// external infrastructure and therefore not <c>HasData()</c>-eligible. A
    /// freshly seeded root with no pages renders a DokuWiki-style
    /// create-invitation on first view, which is the intended blank-page
    /// behaviour. Starter page content, if wanted, is seeded at runtime through
    /// the body-store path by a separate <c>IModuleSeedingInitialiser</c>.
    /// </para>
    /// </remarks>
    public sealed class WikiRootSeeder : EFDataSeederBase, IHasEFDataSeeder
    {
        private const string CrossLinkKeyPrefix = WikiDomainConstants.CrossLinkScheme
            + WikiDomainConstants.CrossLinkSeparator;

        /// <inheritdoc />
        public override void Seed(ModelBuilder modelBuilder)
        {
            ArgumentNullException.ThrowIfNull(modelBuilder);

            List<Wiki> roots = new List<Wiki>();

            // WikiRootKeys.All is already in stable mount-key order, satisfying
            // the determinism requirement that HasData rows are stably ordered.
            foreach (string key in WikiRootKeys.All)
            {
                WikiRootSeedMetadata metadata = ResolveMetadata(key);

                roots.Add(new Wiki
                {
                    Id = DeterministicGuid.FromString(CrossLinkKeyPrefix + key),
                    Key = key,
                    Title = metadata.Title,
                    Description = metadata.Description,
                    Enabled = true,
                });
            }

            modelBuilder.Entity<Wiki>().HasData(roots);
        }

        /// <summary>
        /// Maps a shipped root key to its human-facing title and description.
        /// </summary>
        private static WikiRootSeedMetadata ResolveMetadata(string key)
        {
            return key switch
            {
                WikiRootKeys.Repo1 => new WikiRootSeedMetadata(
                    "Repository 1",
                    "The default wiki document store."),
                WikiRootKeys.Developers => new WikiRootSeedMetadata(
                    "Developers",
                    "Developer documentation, spikes, and engineering notes."),
                WikiRootKeys.Commons => new WikiRootSeedMetadata(
                    "Commons",
                    "Shared, cross-cutting knowledge common to everyone."),
                WikiRootKeys.Intranet => new WikiRootSeedMetadata(
                    "Intranet",
                    "Internal, organisation-facing wiki space."),
                WikiRootKeys.Resources => new WikiRootSeedMetadata(
                    "Resources",
                    "Reference material and resource documentation."),
                WikiRootKeys.Settings => new WikiRootSeedMetadata(
                    "Settings",
                    "Configuration and administration guidance."),
                _ => new WikiRootSeedMetadata(key, key),
            };
        }

        /// <summary>
        /// The human-facing presentation values for a seeded wiki root.
        /// </summary>
        private readonly record struct WikiRootSeedMetadata(string Title, string Description);
    }
}
