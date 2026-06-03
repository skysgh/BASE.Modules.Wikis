# About #

This is a repo template for creating BASE Modules.

Key aspects to notice:

* Module and relevant file names contain a `KWMODULENAME` token for quick renaming using the provided `post-build.ps1` script.
* Module is composed of multiple Assemblies, following DDD patterns, approximately as follows:
  * `*.Interface.UI.Web` invokes APIs found in
  * `*.Interface.API.GraphQL`, composed of thin Controllers wrapping
  * `*.Interface.API.REST`, composed of thin Controllers wrapping
  * `*.Interface.API.ODATA`, composed of thin Controllers wrapping
  * `*.Interface.Models` and
  * `*.Application` services that orchestrate calls between
  * `*.Domain`
  * `*.Infrastructure` and
  * `*.Infrastructure.Data.EF` and
  * `*.Substrate` referring to base contracts, etc.
 

