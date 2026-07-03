using App.Modules.Sys.Shared.Models;
using App.Modules.Sys.Shared.Models.Persistence;

namespace App.Modules.Wikis.Application.Domains.Wikis.Structures.InTransit.Dtos
{
    /// <summary>
    /// Data transfer object for <c>WikiTemplate</c> (ADR-018C templates-as-pages).
    /// <para>
    /// A template is a reusable scaffold composed of ordered sections; it can be
    /// bound to namespaces (slug prefixes) or page subtrees so authoring surfaces
    /// pre-populate structure and editors surface advisory lint feedback.
    /// </para>
    /// </summary>
    public class WikiTemplateDto : IHasGuidId, IHasKey, IHasTitleAndDescription, IHasEnabled
    {
        /// <inheritdoc />
        public Guid Id { get; set; }

        /// <summary>
        /// FK to the owning <c>Wiki</c> root.
        /// </summary>
        public Guid WikiFK { get; set; }

        /// <inheritdoc />
        public string Key { get; set; } = string.Empty;

        /// <inheritdoc />
        public string Title { get; set; } = string.Empty;

        /// <inheritdoc />
        public string Description { get; set; } = string.Empty;

        /// <inheritdoc />
        public bool Enabled { get; set; }

        /// <summary>
        /// Declared content format key for pages using this template
        /// (e.g., "markdown", "html"). Authoring surfaces default to this format.
        /// </summary>
        public string ContentFormatKey { get; set; } = string.Empty;
    }
}
