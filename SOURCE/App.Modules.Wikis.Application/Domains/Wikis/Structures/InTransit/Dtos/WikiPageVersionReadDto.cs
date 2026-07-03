namespace App.Modules.Wikis.Application.Domains.Wikis.Structures.InTransit.Dtos
{
    /// <summary>
    /// Read DTO for <see cref="Domain.Domains.Wikis.Entities.Implementations.WikiPageVersion"/>.
    /// Returned by all GET endpoints and IQueryable projections.
    /// Derives from <see cref="WikiPageVersionWriteDto"/>, which carries the
    /// identity and all immutable snapshot fields.
    /// </summary>
    /// <remarks>
    /// The distinct type is retained to honour the read/write DTO split
    /// convention.
    /// </remarks>
    public class WikiPageVersionReadDto : WikiPageVersionWriteDto
    {
    }
}
