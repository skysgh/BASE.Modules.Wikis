using App.Modules.Sys.Shared.Models;
using App.Modules.Sys.Shared.Models.Base;

namespace App.Modules.Wikis.Domain.Domains.Wikis.Entities.Implementations
{
    /// <summary>
    /// A single ordered block of a <see cref="WikiTemplate"/> scaffold (ADR-018C,
    /// build-plan Phase D step 15): a heading plus authoring guidance, and the
    /// structural expectation a page is later linted against.
    /// <para>
    /// The order here is <b>determinate</b>, not cosmetic: it fixes the sequence
    /// sections are emitted into a scaffolded page <i>and</i> the sequence the
    /// structural lint walks them, so it implements
    /// <see cref="IHasPrecedenceOrder"/> (logic ordering) rather than
    /// <see cref="IHasDisplayOrderHint"/> (display hint). See the
    /// <c>DisplayOrderHint</c>/<c>PrecedenceOrder</c> distinction in the house
    /// rules.
    /// </para>
    /// </summary>
    public class WikiTemplateSection : DefaultEntityBase, IHasKey, IHasTitleAndDescription, IHasPrecedenceOrder
    {
        /// <summary>
        /// FK to the owning <see cref="WikiTemplate"/>.
        /// <para>Navigable, so the suffix is <c>FK</c>, not <c>Id</c>.</para>
        /// </summary>
        public Guid WikiTemplateFK { get; set; }

        /// <summary>
        /// Stable, URL-safe key identifying the section within its template
        /// (e.g. <c>context</c>, <c>decision</c>, <c>consequences</c>). The lint
        /// uses this to report which expected section is missing.
        /// </summary>
        public string Key { get; set; } = string.Empty;

        /// <inheritdoc />
        public string Title { get; set; } = string.Empty;

        /// <inheritdoc />
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// The deterministic order this section is scaffolded and linted in.
        /// Lower values come first. See <see cref="IHasPrecedenceOrder"/>.
        /// </summary>
        public int PrecedenceOrder { get; set; }

        /// <summary>
        /// Whether the structural lint treats this section as required. When
        /// <c>true</c>, a page missing this section's heading raises an advisory
        /// lint finding (advisory-by-default — it never blocks saving).
        /// </summary>
        public bool IsRequired { get; set; }

        /// <summary>
        /// Optional placeholder/guidance prose emitted under the heading when a
        /// page is scaffolded from the template. Plain authoring text in the
        /// template's declared content format; never executable.
        /// </summary>
        public string PlaceholderBody { get; set; } = string.Empty;

        /// <summary>
        /// Navigation: the template this section belongs to.
        /// </summary>
        public WikiTemplate? Template { get; set; }
    }
}
