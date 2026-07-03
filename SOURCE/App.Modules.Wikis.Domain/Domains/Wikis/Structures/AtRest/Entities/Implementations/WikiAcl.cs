using App.Modules.Sys.Shared.Models.Base;

namespace App.Modules.Wikis.Domain.Domains.Wikis.Entities.Implementations
{
    /// <summary>
    /// An access-control entry binding a principal to a permission on a wiki
    /// scope, following the framework's share-based access pattern (a
    /// <c>TenantId</c>-per-row model is an explicit anti-pattern here). Access
    /// is granted by issuing <c>WikiAcl</c> rows to principals rather than by
    /// owning a tenant.
    /// <para>
    /// The <see cref="PrincipalId"/>/<see cref="PrincipalType"/> pair identifies
    /// who the grant is for (User, Group, Workspace, or Everyone). Exactly one
    /// scope FK is populated: an ACL applies either to a whole
    /// <see cref="Wiki"/> (<see cref="WikiFK"/>) or to a single
    /// <see cref="WikiPage"/> (<see cref="WikiPageFK"/>); the page-level grant
    /// is the more specific override. Every resolver applies the reader's grants
    /// and must never act as an existence/content oracle for content the reader
    /// cannot see.
    /// </para>
    /// </summary>
    public class WikiAcl : DefaultEntityBase
    {
        /// <summary>
        /// FK to the <see cref="Wiki"/> root this grant applies to, when the
        /// grant is wiki-wide. <c>null</c> when the grant is page-scoped.
        /// </summary>
        public Guid? WikiFK { get; set; }

        /// <summary>
        /// FK to the <see cref="WikiPage"/> this grant applies to, when the
        /// grant is page-scoped. <c>null</c> when the grant is wiki-wide.
        /// </summary>
        public Guid? WikiPageFK { get; set; }

        /// <summary>
        /// The identifier of the principal the grant is issued to. Interpreted
        /// together with <see cref="PrincipalType"/>.
        /// </summary>
        public Guid PrincipalId { get; set; }

        /// <summary>
        /// The kind of principal (User, Group, Workspace, or Everyone), stored
        /// as the integer of the shared <c>PrincipalType</c> contract to avoid a
        /// hard enum dependency leaking into this Shared model.
        /// </summary>
        public int PrincipalType { get; set; }

        /// <summary>
        /// The permission key granted, e.g. <c>Wiki:Read</c> or
        /// <c>WikiPage:Write</c>, composed from permission constants rather than
        /// a magic string at the call site.
        /// </summary>
        public string PermissionKey { get; set; } = string.Empty;

        /// <summary>
        /// Navigation: the wiki root, when this grant is wiki-scoped.
        /// </summary>
        public Wiki? Wiki { get; set; }

        /// <summary>
        /// Navigation: the page, when this grant is page-scoped.
        /// </summary>
        public WikiPage? Page { get; set; }
    }
}
