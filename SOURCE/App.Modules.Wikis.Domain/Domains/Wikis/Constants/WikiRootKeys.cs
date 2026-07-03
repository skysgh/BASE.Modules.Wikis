namespace App.Modules.Wikis.Domain.Domains.Wikis.Constants
{
    /// <summary>
    /// The fixed, code-defined mount keys of the platform's shipped wiki roots.
    /// </summary>
    /// <remarks>
    /// <para>
    /// A wiki root's <c>Key</c> is the stable store key used in
    /// <c>wiki:{key}:{slug}</c> cross-links and root resolution. Because this is a
    /// <b>closed set defined in code</b>, the roots are seeded deterministically
    /// through EF <c>HasData()</c> (per the EF Seed Determinism Pattern), keeping
    /// the shipped key set and the seeded rows from ever drifting apart.
    /// </para>
    /// <para>
    /// <see cref="Repo1"/> is the authoritative default root the consumer client
    /// resolves on first view. The remaining keys mirror the platform's primary
    /// navigation sections so an author can host a wiki under any of them without
    /// a fresh migration.
    /// </para>
    /// </remarks>
    public static class WikiRootKeys
    {
        /// <summary>
        /// The default shipped wiki root. This is the root the consumer client
        /// addresses by default.
        /// </summary>
        public const string Repo1 = "repo1";

        /// <summary>
        /// The wiki root for the Developers navigation section.
        /// </summary>
        public const string Developers = "developers";

        /// <summary>
        /// The wiki root for the Commons navigation section.
        /// </summary>
        public const string Commons = "commons";

        /// <summary>
        /// The wiki root for the Intranet navigation section.
        /// </summary>
        public const string Intranet = "intranet";

        /// <summary>
        /// The wiki root for the Resources navigation section.
        /// </summary>
        public const string Resources = "resources";

        /// <summary>
        /// The wiki root for the Settings navigation section.
        /// </summary>
        public const string Settings = "settings";

        /// <summary>
        /// All shipped wiki root keys, in deterministic mount-key order. Used as
        /// the stable, ordered seed input for the <c>HasData()</c> projection so
        /// the seeded row set is byte-identical between EF design time and the
        /// runtime host.
        /// </summary>
        public static readonly IReadOnlyList<string> All = new[]
        {
            Commons,
            Developers,
            Intranet,
            Repo1,
            Resources,
            Settings,
        };
    }
}
