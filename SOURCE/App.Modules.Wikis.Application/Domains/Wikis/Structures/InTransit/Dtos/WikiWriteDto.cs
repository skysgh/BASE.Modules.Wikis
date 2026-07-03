using App.Modules.Sys.Shared.Models;
using App.Modules.Sys.Shared.Models.Persistence;

namespace App.Modules.Wikis.Application.Domains.Wikis.Structures.InTransit.Dtos
{
    /// <summary>
    /// Write DTO for <see cref="Domain.Domains.Wikis.Entities.Implementations.Wiki"/>.
    /// Used for POST (create) and PUT (update) operations and serves as the
    /// structural base for <see cref="WikiReadDto"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Carries only the writable scalar fields. A <c>Wiki</c> is the top of the
    /// page tree (a mountable "space"); its richer content hangs off the pages,
    /// so the write surface is deliberately minimal.
    /// </para>
    /// </remarks>
    public class WikiWriteDto : IHasGuidId, IHasKey, IHasTitleAndDescription, IHasEnabled
    {
        /// <inheritdoc/>
        public Guid Id { get; set; }

        /// <inheritdoc/>
        public string Key { get; set; } = string.Empty;

        /// <inheritdoc/>
        public string Title { get; set; } = string.Empty;

        /// <inheritdoc/>
        public string Description { get; set; } = string.Empty;

        /// <inheritdoc/>
        public bool Enabled { get; set; }

        /// <summary>
        /// Gets or sets the optional owning Workspace. <c>null</c> for a
        /// platform-level wiki that is not scoped to a single workspace.
        /// </summary>
        public Guid? OwnerWorkspaceId { get; set; }
    }
}
