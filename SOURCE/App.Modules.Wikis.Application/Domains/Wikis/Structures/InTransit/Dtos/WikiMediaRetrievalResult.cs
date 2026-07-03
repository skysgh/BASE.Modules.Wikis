using App.Modules.Sys.Shared.ObjectStorage.Models.Enums;

namespace App.Modules.Wikis.Application.Domains.Wikis.Structures.InTransit.Dtos
{
    /// <summary>
    /// The outcome of an authorized wiki-media retrieval, shaped by the
    /// configured <see cref="MediaDeliveryMode"/>.
    /// <para>
    /// <b>Proxy</b> deliveries carry a live <see cref="Content"/> stream that the
    /// backend streams to the client (no storage CORS required, but the bytes
    /// flow through the backend). <b>Direct</b> deliveries carry a short-lived
    /// <see cref="SignedUrl"/> the client fetches straight from storage (no
    /// double bandwidth, but the storage account must allow CORS).
    /// </para>
    /// <para>
    /// In both cases the retrieval is only ever produced after the
    /// Application-layer share-based authorization check has passed, so the two
    /// transports are equally gated; they differ only in who moves the bytes.
    /// </para>
    /// </summary>
    public sealed class WikiMediaRetrievalResult
    {
        /// <summary>
        /// The delivery mode this result was produced for.
        /// </summary>
        public MediaDeliveryMode Mode { get; init; }

        /// <summary>
        /// The authoritative media (MIME) type of the blob.
        /// </summary>
        public string MediaType { get; init; } = string.Empty;

        /// <summary>
        /// The content stream, populated only for
        /// <see cref="MediaDeliveryMode.Proxy"/>. The caller is responsible for
        /// disposing it. <c>null</c> for <see cref="MediaDeliveryMode.Direct"/>.
        /// </summary>
        public Stream? Content { get; init; }

        /// <summary>
        /// The time-limited signed read URL, populated only for
        /// <see cref="MediaDeliveryMode.Direct"/>. <c>null</c> for
        /// <see cref="MediaDeliveryMode.Proxy"/>.
        /// </summary>
        public string? SignedUrl { get; init; }
    }
}
