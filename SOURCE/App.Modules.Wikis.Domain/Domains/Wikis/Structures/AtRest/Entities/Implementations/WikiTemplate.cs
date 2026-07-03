using App.Modules.Sys.Shared.Models;
using App.Modules.Sys.Shared.Models.Base;

namespace App.Modules.Wikis.Domain.Domains.Wikis.Entities.Implementations
{
    /// <summary>
    /// A <c>WikiTemplate</c> is a reusable page scaffold (ADR-018C, build-plan
    /// Phase D step 15). It defines the shape a new page should start from — an
    /// ordered set of <see cref="WikiTemplateSection"/> blocks (headings +
    /// authoring guidance) — and the structural expectations a page is later
    /// <i>lint</i>ed against (advisory by default, never blocking).
    /// <para>
    /// Templates are themselves authored as pages ("templates-as-pages") under
    /// the conventional <c>_templates/</c> namespace, so the same versioning,
    /// ACL, and rendering machinery applies to a template as to any page. This
    /// row is the structured, queryable projection of that template: the body
    /// prose lives in a <see cref="WikiPageVersion"/> blob like any page, while
    /// the section contract and binding live here so scaffolding and lint can
    /// reason about structure without re-parsing prose.
    /// </para>
    /// </summary>
    public class WikiTemplate : DefaultEntityBase, IHasKey, IHasTitleAndDescription, IHasEnabled
    {
        /// <summary>
        /// FK to the owning <see cref="Wiki"/> root the template belongs to.
        /// <para>Navigable, so the suffix is <c>FK</c>, not <c>Id</c>.</para>
        /// </summary>
        public Guid WikiFK { get; set; }

        /// <summary>
        /// Stable, URL-safe key identifying the template within its
        /// <see cref="Wiki"/> (e.g. <c>how-to</c>, <c>decision-record</c>). Used
        /// by <see cref="WikiTemplateBinding"/> and by authors when picking a
        /// template to scaffold from.
        /// </summary>
        public string Key { get; set; } = string.Empty;

        /// <inheritdoc />
        public string Title { get; set; } = string.Empty;

        /// <inheritdoc />
        public string Description { get; set; } = string.Empty;

        /// <inheritdoc />
        public bool Enabled { get; set; }

        /// <summary>
        /// The declared content format the template scaffolds in (per the
        /// ADR-018E content-format DSL, e.g. <c>markdown</c>). New pages created
        /// from this template start in this format so the editor emits a
        /// declared format from the outset.
        /// </summary>
        public string ContentFormatKey { get; set; } = string.Empty;

        /// <summary>
        /// Navigation: the owning wiki root.
        /// </summary>
        public Wiki? Wiki { get; set; }

        /// <summary>
        /// Navigation: the ordered sections that make up the template scaffold.
        /// </summary>
        public ICollection<WikiTemplateSection> Sections { get; set; } = new List<WikiTemplateSection>();

        /// <summary>
        /// Navigation: the bindings that attach this template to namespaces or
        /// page subtrees.
        /// </summary>
        public ICollection<WikiTemplateBinding> Bindings { get; set; } = new List<WikiTemplateBinding>();
    }
}
