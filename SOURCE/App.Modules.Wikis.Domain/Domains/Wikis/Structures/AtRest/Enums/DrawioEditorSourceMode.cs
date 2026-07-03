namespace App.Modules.Wikis.Domain.Domains.Wikis.Enums
{
    /// <summary>
    /// Controls where the draw.io diagram editor is sourced from when a wiki
    /// author opens a diagram for editing.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This is a per-deployment administrator choice, surfaced through
    /// <see cref="Configuration.Implementations.WikiConfigurationObject.DrawioEditorSourceMode"/>.
    /// The distinction matters for governance: the <see cref="Remote"/> editor
    /// loads the editing application from a hosted origin (no local diagram-app
    /// hosting required), which is the shipped default because the sensitivity is
    /// around local <em>storage</em> of content, not the remote editor logic.
    /// The <see cref="SelfHosted"/> option exists for deployments that must keep
    /// even the editor assets on-premise.
    /// </para>
    /// <para>
    /// Note: the source of the <em>editor application</em> is independent of where
    /// the diagram <em>content</em> (the draw.io XML) is persisted. Diagram XML is
    /// authored wiki content and is stored exactly like every other version body
    /// (object store today; one day round-tripped to the source tree per the
    /// build plan's Phase J "documentation-as-source-code" work). Switching the
    /// editor source never changes where content lives.
    /// </para>
    /// <para>
    /// Follows the framework enum convention: the first four members are the
    /// reserved sentinels and real options begin at <c>4</c>.
    /// </para>
    /// </remarks>
    public enum DrawioEditorSourceMode
    {
        /// <summary>No value assigned.</summary>
        Undefined = 0,

        /// <summary>Not applicable in this context.</summary>
        NotApplicable = 1,

        /// <summary>Source mode is unspecified.</summary>
        Unspecified = 2,

        /// <summary>Source mode is not known.</summary>
        Unknown = 3,

        /// <summary>
        /// Load the draw.io editor application from a remote hosted origin (the
        /// configured editor base URL, defaulting to the canonical embed host).
        /// This is the shipped default: no local hosting of the editor assets is
        /// required, and the governance-sensitive concern (local storage of
        /// content) is unaffected because content persistence is independent of
        /// editor source.
        /// </summary>
        Remote = 4,

        /// <summary>
        /// Load the draw.io editor application from a self-hosted / on-premise
        /// origin (the configured editor base URL pointed at an in-house deploy).
        /// Opt-in for deployments that must keep even the editor assets local.
        /// </summary>
        SelfHosted = 5,
    }
}
