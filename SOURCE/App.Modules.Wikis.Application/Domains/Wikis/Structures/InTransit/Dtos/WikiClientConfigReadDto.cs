namespace App.Modules.Wikis.Application.Domains.Wikis.Structures.InTransit.Dtos
{
    /// <summary>
    /// Lightweight read DTO projecting the subset of
    /// <see cref="Domain.Domains.Wikis.Configuration.Implementations.WikiConfigurationObject"/>
    /// that the client-side wiki host component needs to initialise itself.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This DTO is served from an unauthenticated GET endpoint so the Angular
    /// application can read the operator-configured defaults (store key, slug,
    /// no-content message, root document name) at startup without requiring a
    /// logged-in session.
    /// </para>
    /// </remarks>
    public class WikiClientConfigReadDto
    {
        /// <summary>
        /// Gets or sets the default wiki-store key delivered to the client when
        /// a hosted wiki leaf model carries no explicit binding. An empty
        /// string means the client should use the configured default root.
        /// </summary>
        public string DefaultHostNamespace { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the default page slug delivered to the client when a
        /// hosted wiki leaf model carries no explicit slug. An empty string
        /// instructs the client to substitute <see cref="DefaultRootDocumentName"/>
        /// as the effective slug.
        /// </summary>
        public string DefaultHostSlug { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the extension-less slug used as the index/root document
        /// within a folder (e.g. <c>home</c>). Used by the client to resolve an
        /// empty <see cref="DefaultHostSlug"/> to a concrete page slug.
        /// </summary>
        public string DefaultRootDocumentName { get; set; } = "home";

        /// <summary>
        /// Gets or sets the message rendered inside the wiki host control once
        /// the server has confirmed the requested page has no content. Never
        /// shown while the request is still in flight.
        /// </summary>
        public string NoContentMessage { get; set; } = "No content available. Create a page by selecting Edit.";
    }
}
