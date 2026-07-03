using App.Modules.Wikis.Domain.Domains.Wikis.Constants;

namespace App.Modules.Wikis.Domain.Domains.Wikis
{
    /// <summary>
    /// Maps a wiki media <b>blob identity</b> (<see cref="System.Guid"/>) to the
    /// container-relative <b>object-store path</b> the Sys media seam expects.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>Why this exists (the design decision).</b> The wiki domain models a
    /// media asset by a canonical immutable <c>BlobId</c> of type
    /// <see cref="System.Guid"/> (ADR-018: "replace" means a new id and a
    /// repoint, never a mutation). The framework's object-store and media
    /// pipeline, however, address blobs by a <i>string path</i> (for example
    /// <c>wikis/media/{blobId}.png</c>). Rather than persist a second,
    /// redundant <c>BlobPath</c> column that could drift from the id, we keep
    /// the <c>Guid</c> as the single source of truth and <b>derive</b> the path
    /// from it on demand. The mapping is pure and deterministic, so the same
    /// blob id always yields the same path in every process.
    /// </para>
    /// <para>
    /// <b>Why the extension is cosmetic.</b> The trailing extension is included
    /// only so that a human browsing the object store sees a recognisable file
    /// suffix. It is never the authority on content type — the authoritative
    /// MIME type is the <c>MediaType</c> stored on the
    /// <see cref="Entities.Implementations.WikiMedia"/> handle. Because the
    /// extension is non-authoritative, an unknown MIME type falls back to a
    /// neutral <see cref="WikiDomainConstants.MediaBlobFallbackExtension"/>
    /// without any loss of correctness: retrieval keys off the
    /// <c>{prefix}/{blobId}</c> stem, and a single blob id only ever maps to one
    /// concrete path because its <c>MediaType</c> is itself immutable.
    /// </para>
    /// </remarks>
    public static class WikiMediaBlobPathFactory
    {
        /// <summary>
        /// Builds the deterministic container-relative object-store path for a
        /// media blob, e.g. <c>wikis/media/{blobId}.png</c>.
        /// </summary>
        /// <param name="blobId">
        /// The immutable blob identity. Must not be <see cref="System.Guid.Empty"/>,
        /// since the empty guid is the "no blob" sentinel and has no storage path.
        /// </param>
        /// <param name="mediaType">
        /// The IANA media (MIME) type of the blob (e.g. <c>image/png</c>). Used
        /// only to choose a readable path extension; never the authority on
        /// content type.
        /// </param>
        /// <returns>The container-relative blob path.</returns>
        /// <exception cref="System.ArgumentException">
        /// Thrown when <paramref name="blobId"/> is <see cref="System.Guid.Empty"/>.
        /// </exception>
        public static string BuildBlobPath(Guid blobId, string? mediaType)
        {
            if (blobId == Guid.Empty)
            {
                throw new ArgumentException(
                    "An empty blob id has no object-store path; the empty guid is the 'no blob' sentinel.",
                    nameof(blobId));
            }

            string extension = ResolveExtension(mediaType);

            // The path stem is the blob id; the extension is purely cosmetic.
            // Format: {prefix}/{blobId}.{ext}
            return WikiDomainConstants.MediaBlobPathPrefix
                + WikiDomainConstants.PathSegmentSeparator
                + blobId.ToString("D")
                + "."
                + extension;
        }

        /// <summary>
        /// Resolves a readable storage path extension from a media (MIME) type.
        /// </summary>
        /// <remarks>
        /// Mapping is explicit and conservative (no guessing): only the common
        /// wiki media types are mapped, and anything else falls back to the
        /// neutral <see cref="WikiDomainConstants.MediaBlobFallbackExtension"/>.
        /// The extension is cosmetic, so an unmapped type is fully correct — it
        /// simply produces a <c>.bin</c> suffix while the authoritative type
        /// remains the stored <c>MediaType</c>.
        /// </remarks>
        /// <param name="mediaType">The IANA media (MIME) type, or null/empty.</param>
        /// <returns>A lower-case extension without a leading dot.</returns>
        private static string ResolveExtension(string? mediaType)
        {
            if (string.IsNullOrWhiteSpace(mediaType))
            {
                return WikiDomainConstants.MediaBlobFallbackExtension;
            }

            // Normalise: strip any parameters (e.g. "; charset=...") and case.
            string normalised = mediaType.Split(';')[0].Trim().ToLowerInvariant();

            switch (normalised)
            {
                case "image/png":
                {
                    return "png";
                }

                case "image/jpeg":
                {
                    return "jpg";
                }

                case "image/gif":
                {
                    return "gif";
                }

                case "image/webp":
                {
                    return "webp";
                }

                case "image/svg+xml":
                {
                    return "svg";
                }

                case "application/pdf":
                {
                    return "pdf";
                }

                default:
                {
                    return WikiDomainConstants.MediaBlobFallbackExtension;
                }
            }
        }
    }
}
