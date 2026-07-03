namespace App.Modules.Wikis.Domain.Domains.Wikis.Structures.AtRest.Enums
{
    /// <summary>
    /// Bounded text-contrast modes for a <see cref="Entities.Implementations.WikiNodeStyle"/> background.
    /// </summary>
    public enum WikiNodeStyleContrastMode
    {
        Undefined = 0,
        NotApplicable = 1,
        Unspecified = 2,
        Unknown = 3,
        Auto = 4,
        LightText = 5,
        DarkText = 6,
    }
}
