namespace App.Modules.Wikis.Domain.Domains.Wikis.Services.BodyStorage
{
    /// <summary>
    /// The outcome of storing a single page-version body through a
    /// <see cref="IWikiBodyStore"/> (ADR-018N §2.1).
    /// </summary>
    /// <remarks>
    /// <para>
    /// <see cref="BodyLocator"/> is the <em>sink-specific</em> handle the same
    /// sink needs to read the bytes back (a GUID for the DB and object-store
    /// sinks, a repo-relative path for the file-system sink). It is opaque to
    /// callers and is what the version row persists (ADR-018N §2.6), so callers
    /// never interpret it.
    /// </para>
    /// <para>
    /// <see cref="ContentHash"/> is sink-independent, which is the property that
    /// lets a body be mirrored across sinks and verified identical by hash, and
    /// that an ADR-018M endorsement continues to pin to regardless of where the
    /// bytes physically live.
    /// </para>
    /// </remarks>
    public sealed class WikiBodyStoreResult
    {
        /// <summary>
        /// Initialises a new <see cref="WikiBodyStoreResult"/>.
        /// </summary>
        /// <param name="bodyLocator">The sink-specific handle to read the bytes back.</param>
        /// <param name="contentHash">The sink-independent content hash of the stored body.</param>
        /// <param name="contentByteLength">The byte length of the stored body.</param>
        public WikiBodyStoreResult(string bodyLocator, string contentHash, long contentByteLength)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(bodyLocator);
            ArgumentException.ThrowIfNullOrWhiteSpace(contentHash);

            this.BodyLocator = bodyLocator;
            this.ContentHash = contentHash;
            this.ContentByteLength = contentByteLength;
        }

        /// <summary>
        /// The sink-specific handle used to retrieve the bytes back from the same
        /// sink. Opaque to callers; persisted on the version row.
        /// </summary>
        public string BodyLocator { get; }

        /// <summary>
        /// The content hash of the stored body, computed identically regardless
        /// of sink so mirrored copies can be verified equal.
        /// </summary>
        public string ContentHash { get; }

        /// <summary>The byte length of the stored body.</summary>
        public long ContentByteLength { get; }
    }
}
