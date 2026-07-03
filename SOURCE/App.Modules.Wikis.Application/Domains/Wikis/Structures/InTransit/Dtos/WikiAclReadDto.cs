namespace App.Modules.Wikis.Application.Domains.Wikis.Structures.InTransit.Dtos
{
    /// <summary>
    /// Read DTO for <see cref="Domain.Domains.Wikis.Entities.Implementations.WikiAcl"/>.
    /// Returned by all GET endpoints and IQueryable projections.
    /// Derives from <see cref="WikiAclWriteDto"/>, which carries the identity
    /// and all grant fields.
    /// </summary>
    /// <remarks>
    /// The distinct type is retained to honour the read/write DTO split
    /// convention.
    /// </remarks>
    public class WikiAclReadDto : WikiAclWriteDto
    {
    }
}
