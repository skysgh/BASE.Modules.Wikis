using App.Modules.Sys.Shared.Models;
using App.Modules.Sys.Shared.Models.Base;

namespace App.Modules.Wikis.Domain.Domains.Wikis.Entities.Implementations
{
    /// <summary>
    /// A <c>Wiki</c> is a single, independently mountable wiki root (a "space").
    /// <para>
    /// It is the top of the page tree: every <see cref="WikiPage"/> belongs to
    /// exactly one <c>Wiki</c>, and the <see cref="IHasKey.Key"/> is the stable
    /// mount key (the namespace segment used in <c>wiki:{key}:{slug}</c>
    /// cross-links and in routing). Multiple wikis can co-exist on the platform
    /// (per ADR-018H multi-root), so domain-neutral key + title + description is
    /// all that is modelled here; everything richer hangs off the pages.
    /// </para>
    /// </summary>
    public class Wiki : DefaultEntityBase, IHasKey, IHasTitleAndDescription, IHasEnabled
    {
        /// <inheritdoc />
        public string Key { get; set; } = string.Empty;

        /// <inheritdoc />
        public string Title { get; set; } = string.Empty;

        /// <inheritdoc />
        public string Description { get; set; } = string.Empty;

        /// <inheritdoc />
        public bool Enabled { get; set; }

        /// <summary>
        /// Optional owning Workspace. <c>null</c> for a platform-level wiki that
        /// is not scoped to a single workspace.
        /// </summary>
        public Guid? OwnerWorkspaceId { get; set; }

        /// <summary>
        /// Navigation: the pages that belong to this wiki root.
        /// </summary>
        public ICollection<WikiPage> Pages { get; set; } = new List<WikiPage>();

        /// <summary>
        /// Navigation: wiki-wide access-control grants scoped to this root.
        /// </summary>
        public ICollection<WikiAcl> Acls { get; set; } = new List<WikiAcl>();
    }
}
