using App.Modules.Wikis.Domain.Domains.Wikis.Constants;

namespace App.Modules.Wikis.Domain.Domains.Wikis.Services.BodyStorage
{
    /// <summary>
    /// Maps a wiki page-version <b>body locator</b> to the deterministic storage
    /// path each non-database body sink uses (ADR-018N). Pure and deterministic,
    /// mirroring <see cref="WikiMediaBlobPathFactory"/>'s discipline: the locator
    /// is the single source of truth and the path is <b>derived on demand</b>,
    /// never persisted as a second column that could drift.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>Object-store path</b> (<see cref="BuildObjectStorePath"/>) places body
    /// blobs under <see cref="WikiDomainConstants.BodyBlobPathPrefix"/>
    /// (<c>wikis/bodies</c>) — deliberately distinct from the media prefix so a
    /// version body and a media asset can never collide in the store and the two
    /// lifecycles stay operationally separable.
    /// </para>
    /// <para>
    /// <b>Content-repo path</b> (<see cref="BuildFileSystemRelativePath"/>)
    /// builds a human- and Git-friendly path from the wiki key, slug, and version
    /// number, so someone browsing the external content repository sees a
    /// recognisable per-page version history rather than opaque GUIDs. The path
    /// is still recomputable from immutable inputs, so a page move never strands a
    /// stored path. The leaf extension is cosmetic (ADR-018N): the authoritative
    /// format is the version's <c>ContentFormatKey</c>, never the file suffix.
    /// </para>
    /// </remarks>
    public static class WikiBodyPathFactory
    {
        /// <summary>
        /// Builds the container-relative object-store path for a version body
        /// blob, e.g. <c>wikis/bodies/{locator}.txt</c>.
        /// </summary>
        /// <param name="bodyLocator">
        /// The immutable body locator (a GUID handle). Must not be
        /// <see cref="Guid.Empty"/>, which is the "no body" sentinel.
        /// </param>
        /// <returns>The container-relative body blob path.</returns>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="bodyLocator"/> is <see cref="Guid.Empty"/>.
        /// </exception>
        public static string BuildObjectStorePath(Guid bodyLocator)
        {
            if (bodyLocator == Guid.Empty)
            {
                throw new ArgumentException(
                    "An empty body locator has no object-store path; the empty guid is the 'no body' sentinel.",
                    nameof(bodyLocator));
            }

            return WikiDomainConstants.BodyBlobPathPrefix
                + WikiDomainConstants.PathSegmentSeparator
                + bodyLocator.ToString("D")
                + "."
                + WikiDomainConstants.FileSystemBodyFileExtension;
        }

        /// <summary>
        /// Builds the content-repository-relative path (using <c>/</c> separators)
        /// for a version body file, e.g.
        /// <c>wikis/{wikiKey}/{slug}/v{versionNumber}.txt</c>. The caller combines
        /// this with the configured external content-repo root.
        /// </summary>
        /// <param name="wikiKey">The owning wiki's mount key (e.g. <c>docs</c>).</param>
        /// <param name="slug">The page's slug relative to the wiki root (e.g. <c>a/b/c</c>).</param>
        /// <param name="versionNumber">The 1-based version number.</param>
        /// <returns>The content-repo-relative body file path with <c>/</c> separators.</returns>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="wikiKey"/> is null or whitespace, or
        /// <paramref name="versionNumber"/> is not positive.
        /// </exception>
        public static string BuildFileSystemRelativePath(string wikiKey, string? slug, int versionNumber)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(wikiKey);

            if (versionNumber <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(versionNumber),
                    "A body file path requires a positive (1-based) version number.");
            }

            string separator = WikiDomainConstants.PathSegmentSeparator;
            string normalizedSlug = (slug ?? string.Empty).Trim(separator[0]);

            string slugSegment = normalizedSlug.Length > 0
                ? normalizedSlug + separator
                : string.Empty;

            // Format: {prefix}/{wikiKey}/{slug}/v{n}.{ext}
            return WikiDomainConstants.FileSystemBodyPathPrefix
                + separator
                + wikiKey.Trim(separator[0])
                + separator
                + slugSegment
                + "v"
                + versionNumber.ToString(System.Globalization.CultureInfo.InvariantCulture)
                + "."
                + WikiDomainConstants.FileSystemBodyFileExtension;
        }
    }
}
