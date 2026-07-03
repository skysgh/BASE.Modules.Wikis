using App.Modules.Sys.Shared.Application;
using App.Modules.Wikis.Application.Domains.Wikis.Structures.InTransit.Dtos;

namespace App.Modules.Wikis.Application.Domains.Wikis.Services
{
    /// <summary>
    /// Application service contract for <see cref="WikiPageVersionReadDto"/>
    /// operations. Extends
    /// <see cref="ICrudStateAppService{TReadDto,TCreateDto,TUpdateDto}"/> for
    /// standard CRUST operations, returning IQueryable for OData filtering,
    /// paging, and sorting at the API boundary.
    /// </summary>
    public interface IWikiPageVersionApplicationService
        : ICrudStateAppService<WikiPageVersionReadDto, WikiPageVersionWriteDto, WikiPageVersionWriteDto>
    {
    }
}
