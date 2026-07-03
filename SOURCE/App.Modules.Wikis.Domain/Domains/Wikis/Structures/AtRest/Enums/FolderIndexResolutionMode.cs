namespace App.Modules.Wikis.Domain.Domains.Wikis.Enums
{
    /// <summary>
    /// Controls how a folder-style path (for example <c>a/b/c</c>) is resolved
    /// against a folder index document versus a single leaf page.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Wikis differ on whether <c>a/b/c</c> should first look for a folder index
    /// document (<c>a/b/c/home</c>) and only then fall back to a leaf page
    /// (<c>a/b/c</c>), or the reverse, or only ever one of them. This is a
    /// per-deployment administrator choice, surfaced through
    /// <see cref="Configuration.Implementations.WikiConfigurationObject.FolderIndexResolutionMode"/>.
    /// </para>
    /// <para>
    /// Follows the framework enum convention: the first four members are the
    /// reserved sentinels and real options begin at <c>4</c>.
    /// </para>
    /// </remarks>
    public enum FolderIndexResolutionMode
    {
        /// <summary>No value assigned.</summary>
        Undefined = 0,

        /// <summary>Not applicable in this context.</summary>
        NotApplicable = 1,

        /// <summary>Resolution mode is unspecified.</summary>
        Unspecified = 2,

        /// <summary>Resolution mode is not known.</summary>
        Unknown = 3,

        /// <summary>
        /// Prefer the folder index: resolve <c>a/b/c</c> as the index document
        /// <c>a/b/c/{root}</c> first, falling back to the leaf page <c>a/b/c</c>
        /// only when no index exists. This is the conventional wiki default.
        /// </summary>
        IndexThenPage = 4,

        /// <summary>
        /// Prefer the leaf page: resolve <c>a/b/c</c> as the page <c>a/b/c</c>
        /// first, falling back to the folder index <c>a/b/c/{root}</c> only when
        /// no leaf page exists.
        /// </summary>
        PageThenIndex = 5,

        /// <summary>
        /// Only ever resolve the folder index document <c>a/b/c/{root}</c>;
        /// never treat <c>a/b/c</c> itself as a leaf page.
        /// </summary>
        IndexOnly = 6,

        /// <summary>
        /// Only ever resolve the leaf page <c>a/b/c</c>; never look for a folder
        /// index document.
        /// </summary>
        PageOnly = 7,
    }
}
