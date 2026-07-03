using App.Modules.Sys.Shared.Application;
using App.Modules.Wikis.Application.Domains.Wikis.Structures.InTransit.Dtos;

namespace App.Modules.Wikis.Application.Domains.Wikis.Services
{
    /// <summary>
    /// Application service contract for <c>WikiTemplate</c> CRUST operations
    /// (ADR-018C templates-as-pages).
    /// </summary>
    /// <remarks>
    /// Provides standard create/read/update/state-transition orchestration for wiki
    /// templates. Section and binding management will later be exposed via dedicated
    /// operations or separate child services.
    /// </remarks>
    public interface IWikiTemplateAppService
        : ICrudStateAppService<WikiTemplateDto, WikiTemplateDto, WikiTemplateDto>,
          IHasApplicationScopeService
    {
    }
}
