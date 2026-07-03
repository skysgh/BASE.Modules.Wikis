using App.Modules.Sys.Application.Domains.AccessControl.Permissions.Constants;

namespace App.Modules.Wikis.Interfaces.API.REST.Constants
{
	/// <summary>
	/// Wikis module API route constants.
	/// NO MAGIC STRINGS - all routes composed from constants.
	/// Organized by: {root}/{api-type}/{module}/{version}/{path}
	/// </summary>
	/// <remarks>
	/// Pattern: api/rest/wikis/v1/{controller-path}
	/// Built on shared <see cref="ApiConstants"/> from Substrate.
	/// </remarks>
	public static class ApiRoutes
	{
		private const string ModuleId = ModuleConstants.Key;
		private const string RestModuleBase = ApiConstants.Root + "/" + ApiConstants.RestType + "/" + ModuleId;

		/// <summary>
		/// REST API routes for Wikis module.
		/// </summary>
		public static class Rest
		{
			/// <summary>
			/// Version 1 of Wikis module REST APIs.
			/// </summary>
			public static class V1
			{
				private const string VersionBase = RestModuleBase + "/" + ApiConstants.Versions.V1;

				/// <summary>
				/// Standard controller route template.
				/// Value: "api/rest/wikis/v1/{controller}"
				/// </summary>
				public const string ControllerRoute = VersionBase + "/{controller}";

				/// <summary>Wiki root endpoints.</summary>
				public static class Wikis
				{
					/// <summary>Value: "api/rest/wikis/v1/wiki"</summary>
					public const string Base = VersionBase + "/wiki";

					/// <summary>
					/// Controller-relative action route for the public client
					/// configuration projection (no authentication required).
					/// Value: "client-config"
					/// </summary>
					public const string ClientConfigAction = "client-config";
				}

				/// <summary>WikiPage endpoints.</summary>
				public static class WikiPages
				{
					/// <summary>Value: "api/rest/wikis/v1/wiki-page"</summary>
					public const string Base = VersionBase + "/wiki-page";

					/// <summary>
					/// Controller-relative action route template for the
					/// server-composed single-GET render projection of a page by
					/// its id (page metadata + current version metadata + inline
					/// body). Value: "{id:guid}/content"
					/// </summary>
					/// <remarks>
					/// The CRUST surface (inherited from the base controller)
					/// returns the thin page row; this action returns the
					/// render-ready <c>WikiPageContentReadDto</c> so a reader
					/// renders from one GET. Kept as a constant so the controller
					/// carries no magic strings.
					/// </remarks>
					public const string ContentByIdAction = "{id:guid}/content";

					/// <summary>
					/// Controller-relative action route template for the
					/// server-composed single-GET render projection of a page by
					/// its canonical DokuWiki-style path within a wiki root.
					/// Value: "wiki/{wikiId:guid}/content/{**path}"
					/// </summary>
					/// <remarks>
					/// The <c>{**path}</c> catch-all carries the full slash-shaped
					/// page path (e.g. <c>engineering/onboarding/setup</c>); a
					/// page that does not exist yet returns a projection with
					/// <c>HasContent = false</c> so the client renders the "create
					/// this page" invitation without a separate existence probe.
					/// </remarks>
					public const string ContentByPathAction = "wiki/{wikiId:guid}/content/{**path}";

					/// <summary>
					/// Controller-relative action route template for listing the thin
					/// page rows that belong to a single wiki root in canonical path
					/// order. Value: "wiki/{wikiId:guid}".
					/// </summary>
					public const string ByWikiAction = "wiki/{wikiId:guid}";

					/// <summary>
					/// Controller-relative action route template for saving an
					/// edit to a page's content (store body → append immutable
					/// version → repoint current). Value: "content"
					/// </summary>
					/// <remarks>
					/// The owning wiki root and canonical path travel in the
					/// request body (<c>WikiPageContentWriteDto</c>) rather than
					/// the route, because a save may also <i>create</i> a
					/// previously-missing page. Kept as a constant so the
					/// controller carries no magic strings.
					/// </remarks>
					public const string SaveContentAction = "content";
				}

				/// <summary>WikiPageVersion endpoints.</summary>
				public static class WikiPageVersions
				{
					/// <summary>Value: "api/rest/wikis/v1/wiki-page-version"</summary>
					public const string Base = VersionBase + "/wiki-page-version";
				}

				/// <summary>WikiMedia endpoints.</summary>
				public static class WikiMedias
				{
					/// <summary>Value: "api/rest/wikis/v1/wiki-media"</summary>
					public const string Base = VersionBase + "/wiki-media";

					/// <summary>
					/// Controller-relative action route template for the media
					/// <b>byte</b> round trip (upload/download), keyed by media id.
					/// Value: "{id:guid}/bytes"
					/// </summary>
					/// <remarks>
					/// The CRUST surface (inherited from the base controller)
					/// addresses the immutable media <i>handle</i>; this action
					/// addresses the underlying object-store <i>bytes</i>. Kept as
					/// a constant so the controller carries no magic strings.
					/// </remarks>
					public const string BytesAction = "{id:guid}/bytes";

					/// <summary>
					/// Controller-relative action route template for storing a
					/// draw.io diagram as its two-artifact pair (editable source
					/// mxfile plus display render SVG) under one owning page.
					/// Value: "diagram"
					/// </summary>
					/// <remarks>
					/// The render is the artifact authored content displays via a
					/// <c>drawio:{id}</c> token; the source is what the editor
					/// reopens for edit. A single share-based authorization check
					/// governs both writes. Kept as a constant so the controller
					/// carries no magic strings.
					/// </remarks>
					public const string DiagramAction = "diagram";

					/// <summary>
					/// Controller-relative action route template for resolving the
					/// editable <b>source</b> (mxfile) of a stored diagram
					/// <b>render</b>, keyed by the render media id, so the editor
					/// can reopen it for edit. Value: "{id:guid}/diagram-source"
					/// </summary>
					public const string DiagramSourceAction = "{id:guid}/diagram-source";
				}

				/// <summary>WikiAcl endpoints.</summary>
				public static class WikiAcls
				{
					/// <summary>Value: "api/rest/wikis/v1/wiki-acl"</summary>
					public const string Base = VersionBase + "/wiki-acl";
				}

				/// <summary>WikiTemplate endpoints (ADR-018C).</summary>
				public static class WikiTemplates
				{
					/// <summary>Value: "api/rest/wikis/v1/wiki-template"</summary>
					public const string Base = VersionBase + "/wiki-template";
				}

				/// <summary>WikiNodeStyle reference-data endpoints.</summary>
				public static class WikiNodeStyles
				{
					/// <summary>Value: "api/rest/wikis/v1/wiki-node-style"</summary>
					public const string Base = VersionBase + "/wiki-node-style";

					/// <summary>Controller-relative action route for page-scoped node-style lookup.</summary>
					public const string ByPageAction = "page/{wikiPageId:guid}";
				}
							}
						}
					}
				}
