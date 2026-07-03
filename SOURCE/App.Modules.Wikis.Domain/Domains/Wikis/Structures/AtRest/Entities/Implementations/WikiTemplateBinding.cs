using App.Modules.Sys.Shared.Models;
using App.Modules.Sys.Shared.Models.Base;

namespace App.Modules.Wikis.Domain.Domains.Wikis.Entities.Implementations
{
    /// <summary>
    /// Attaches a <see cref="WikiTemplate"/> to a part of a wiki so new pages
    /// there default to that template, and existing pages there are linted
    /// against it (ADR-018C, build-plan Phase D step 15).
    /// <para>
    /// A binding targets either a whole namespace (by slug prefix) or a specific
    /// page subtree (by <see cref="ScopeWikiPageFK"/>). When more than one
    /// binding could apply, <see cref="PrecedenceOrder"/> decides which wins —
    /// this is logic ordering (which template governs), not display, so it uses
    /// <see cref="IHasPrecedenceOrder"/>.
    /// </para>
    /// </summary>
    public class WikiTemplateBinding : DefaultEntityBase, IHasEnabled, IHasPrecedenceOrder
    {
        /// <summary>
        /// FK to the <see cref="WikiTemplate"/> being bound.
        /// <para>Navigable, so the suffix is <c>FK</c>, not <c>Id</c>.</para>
        /// </summary>
        public Guid WikiTemplateFK { get; set; }

        /// <summary>
        /// Id of the owning <see cref="Wiki"/> root the binding lives in.
        /// <para>
        /// Deliberately a non-navigable aggregate id (no <c>Wiki</c> navigation):
        /// the binding already cascades from its <see cref="WikiTemplate"/>
        /// (which is itself owned by the wiki), so adding a second direct
        /// <c>Wiki -&gt; Binding</c> cascade would create multiple cascade paths
        /// that SQL Server rejects. This scalar is kept purely as a denormalised
        /// scope/query key, mirroring the <c>WikiPage.CurrentVersionId</c>
        /// "plain id to avoid a cycle" precedent.
        /// </para>
        /// </summary>
        public Guid WikiId { get; set; }

        /// <summary>
        /// Optional FK to a <see cref="WikiPage"/> whose subtree this binding
        /// scopes to. <c>null</c> means the binding scopes by
        /// <see cref="ScopeSlugPrefix"/> instead.
        /// </summary>
        public Guid? ScopeWikiPageFK { get; set; }

        /// <summary>
        /// Optional slug prefix this binding scopes to (e.g. <c>how-to/</c>).
        /// Empty when the binding scopes by <see cref="ScopeWikiPageFK"/>, or to
        /// bind the whole wiki.
        /// </summary>
        public string ScopeSlugPrefix { get; set; } = string.Empty;

        /// <inheritdoc />
        public bool Enabled { get; set; }

        /// <summary>
        /// The deterministic order used to pick a winning binding when several
        /// overlap. Lower values win first. See <see cref="IHasPrecedenceOrder"/>.
        /// </summary>
        public int PrecedenceOrder { get; set; }

        /// <summary>
        /// Navigation: the bound template.
        /// </summary>
        public WikiTemplate? Template { get; set; }

        /// <summary>
        /// Navigation: the page whose subtree this binding scopes to (when
        /// page-scoped).
        /// </summary>
        public WikiPage? ScopePage { get; set; }
    }
}
