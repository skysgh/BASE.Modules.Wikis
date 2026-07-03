namespace App.Modules.Wikis.Application.Domains.Wikis.Structures.InTransit.Dtos
{
    /// <summary>
    /// Read DTO for <see cref="Domain.Domains.Wikis.Entities.Implementations.Wiki"/>.
    /// Returned by all GET endpoints and IQueryable projections.
    /// Derives from <see cref="WikiWriteDto"/>, which carries the identity and
    /// all writable scalar fields.
    /// </summary>
    /// <remarks>
    /// The wiki root is intentionally thin; no additional read-only navigation
    /// projections are exposed here (pages are addressed through the page
    /// endpoints). The distinct type is retained to honour the read/write DTO
    /// split convention.
    /// </remarks>
    public class WikiReadDto : WikiWriteDto
    {
    }
}
