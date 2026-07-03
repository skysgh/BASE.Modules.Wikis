namespace App.Modules.Wikis.Application.Domains.Wikis.Structures.InTransit.Dtos
{
    /// <summary>
    /// Read DTO for <see cref="Domain.Domains.Wikis.Entities.Implementations.WikiPage"/>.
    /// Returned by all GET endpoints and IQueryable projections.
    /// Derives from <see cref="WikiPageWriteDto"/>, which carries the identity,
    /// addressing, and all writable scalar fields.
    /// </summary>
    /// <remarks>
    /// The distinct type is retained to honour the read/write DTO split
    /// convention. Version and media content are addressed through their own
    /// endpoints rather than projected inline here.
    /// </remarks>
    public class WikiPageReadDto : WikiPageWriteDto
    {
    }
}
