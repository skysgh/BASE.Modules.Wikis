using App.Modules.Sys.Shared.Application;
using App.Modules.Wikis.Application.Domains.Wikis.Structures.InTransit.Dtos;

namespace App.Modules.Wikis.Application.Domains.Wikis.Services
{
    /// <summary>
    /// Application service contract for <see cref="WikiReadDto"/> operations.
    /// Extends <see cref="ICrudStateAppService{TReadDto,TCreateDto,TUpdateDto}"/>
    /// for standard CRUST operations, returning IQueryable for OData filtering,
    /// paging, and sorting at the API boundary.
    /// </summary>
    public interface IWikiApplicationService
        : ICrudStateAppService<WikiReadDto, WikiWriteDto, WikiWriteDto>
    {
        /// <summary>
        /// Returns the operator-configured client-side defaults for the wiki
        /// host component (namespace, slug, root document name, no-content
        /// message). This projection is read from
        /// <see cref="Domain.Domains.Wikis.Configuration.Implementations.WikiConfigurationObject"/>
        /// and requires no database access.
        /// </summary>
        /// <returns>A populated <see cref="WikiClientConfigReadDto"/>.</returns>
        WikiClientConfigReadDto GetClientConfig();
    }
}
