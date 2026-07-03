using App.Modules.Sys.Shared.Models;
using App.Modules.Sys.Shared.Models.Persistence;

namespace App.Modules.Wikis.Application.Domains.Wikis.Structures.InTransit.Dtos
{
    /// <summary>
    /// Write DTO for <see cref="Domain.Domains.Wikis.Entities.Implementations.WikiAcl"/>.
    /// Used for POST (create) and PUT (update) operations and serves as the
    /// structural base for <see cref="WikiAclReadDto"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// An ACL row binds a principal to a permission on a wiki scope, following
    /// the framework's share-based access pattern. Exactly one scope FK is
    /// populated: a grant applies either to a whole wiki (<see cref="WikiFK"/>)
    /// or to a single page (<see cref="WikiPageFK"/>, the more specific
    /// override).
    /// </para>
    /// </remarks>
    public class WikiAclWriteDto : IHasGuidId
    {
        /// <inheritdoc/>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the FK to the wiki root this grant applies to, when the
        /// grant is wiki-wide. <c>null</c> when the grant is page-scoped.
        /// </summary>
        public Guid? WikiFK { get; set; }

        /// <summary>
        /// Gets or sets the FK to the page this grant applies to, when the grant
        /// is page-scoped. <c>null</c> when the grant is wiki-wide.
        /// </summary>
        public Guid? WikiPageFK { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the principal the grant is issued to.
        /// Interpreted together with <see cref="PrincipalType"/>.
        /// </summary>
        public Guid PrincipalId { get; set; }

        /// <summary>
        /// Gets or sets the kind of principal (User, Group, Workspace, or
        /// Everyone), stored as the integer of the shared PrincipalType
        /// contract.
        /// </summary>
        public int PrincipalType { get; set; }

        /// <summary>
        /// Gets or sets the permission key granted, e.g. <c>Wiki:Read</c> or
        /// <c>WikiPage:Write</c>.
        /// </summary>
        public string PermissionKey { get; set; } = string.Empty;
    }
}
