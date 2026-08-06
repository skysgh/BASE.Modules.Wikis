using System.Text;
using App.Modules.Wikis.Domain.Domains.Wikis.Services.BodyStorage;
using Tests.Modules.Wikis.Static.Helpers;

namespace Tests.Modules.Wikis.Static.Domain
{
    /// <summary>
    /// Gate tests for <see cref="WikiBodyContentHasher"/> (ADR-018N §2.6): the
    /// body content hash MUST be sink-independent and deterministic, because it
    /// is the property that lets a body be mirrored across sinks and verified
    /// identical, and is the subject an ADR-018M endorsement pins to. If hashing
    /// drifts (encoding, casing, algorithm), mirror verification and endorsement
    /// both silently break — so these are gate tests, not nice-to-haves.
    /// </summary>
    [Trait(TestTraits.Mode, TestTraits.Modes.Static)]
    [Trait(TestTraits.Capability, TestTraits.Capabilities.BodyStorage)]
    [Trait(TestTraits.Quality, TestTraits.Iso25010.FunctionalSuitability.Correctness)]
    public class WikiBodyContentHasherTests
    {
        [Fact]
        public void WhenHashingSameBytesTwice_ThenTheHashIsIdentical()
        {
            byte[] body = Encoding.UTF8.GetBytes("# Title\n\nSome wiki body.");

            string first = WikiBodyContentHasher.ComputeHash(body);
            string second = WikiBodyContentHasher.ComputeHash(body);

            Assert.Equal(first, second);
        }

        [Fact]
        public void WhenHashingDifferentBytes_ThenTheHashDiffers()
        {
            string a = WikiBodyContentHasher.ComputeHash(Encoding.UTF8.GetBytes("alpha"));
            string b = WikiBodyContentHasher.ComputeHash(Encoding.UTF8.GetBytes("beta"));

            Assert.NotEqual(a, b);
        }

        [Fact]
        public void WhenHashing_ThenItIsLowerCaseHexSha256()
        {
            string hash = WikiBodyContentHasher.ComputeHash(Encoding.UTF8.GetBytes("x"));

            // SHA-256 hex digest is 64 lower-case hex characters.
            Assert.Equal(64, hash.Length);
            Assert.Equal(hash.ToLowerInvariant(), hash);
            Assert.All(hash, c => Assert.True(Uri.IsHexDigit(c)));
        }

        [Fact]
        public void WhenHashingTheSameBytesAsTwoDifferentSinksWould_ThenHashesMatch()
        {
            // The DB sink and object-store sink both hash the raw bytes via this
            // helper; prove the two callers cannot diverge for identical input.
            byte[] body = Encoding.UTF8.GetBytes("mirror me");

            string asDbSinkWouldCompute = WikiBodyContentHasher.ComputeHash(body);
            string asObjectStoreSinkWouldCompute = WikiBodyContentHasher.ComputeHash(body.AsSpan());

            Assert.Equal(asDbSinkWouldCompute, asObjectStoreSinkWouldCompute);
        }
    }
}
