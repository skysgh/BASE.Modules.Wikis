using System.Security.Cryptography;

namespace App.Modules.Wikis.Domain.Domains.Wikis.Services.BodyStorage
{
    /// <summary>
    /// Computes the <b>sink-independent</b> content hash of a wiki page-version
    /// body (ADR-018N §2.6). Every <see cref="IWikiBodyStore"/> hashes through
    /// this single helper so the same bytes always yield the same
    /// <see cref="WikiBodyStoreResult.ContentHash"/> regardless of which sink
    /// stored them — the property that makes a body verifiable-equal when
    /// mirrored across sinks, and that an ADR-018M endorsement pins to.
    /// </summary>
    /// <remarks>
    /// Pure and deterministic (SHA-256, lower-case hex), so it is trivially
    /// unit-testable and free of any storage dependency. SHA-256 is chosen as a
    /// widely-available, collision-resistant content fingerprint; the value is an
    /// integrity/identity anchor, not a security secret.
    /// </remarks>
    public static class WikiBodyContentHasher
    {
        /// <summary>
        /// Computes the lower-case hex SHA-256 hash of the supplied body bytes.
        /// </summary>
        /// <param name="body">The raw body bytes to hash (may be empty).</param>
        /// <returns>The lower-case hex SHA-256 digest of <paramref name="body"/>.</returns>
        public static string ComputeHash(ReadOnlySpan<byte> body)
        {
            Span<byte> digest = stackalloc byte[SHA256.HashSizeInBytes];
            SHA256.HashData(body, digest);
            return Convert.ToHexStringLower(digest);
        }
    }
}
