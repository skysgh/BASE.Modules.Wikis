using App.Modules.Wikis.Domain.Domains.Wikis.Constants;

namespace Tests.Modules.Wikis.Domain.Domains.Wikis.Configuration
{
    /// <summary>
    /// Failing-test prompt for the deferred config-section rename debt
    /// (see <c>DOCUMENTATION/03.Governance/01.Registries/TECHNICAL-DEBT-RENAME-WIKIS-CONFIG-SECTION.md</c>).
    /// </summary>
    /// <remarks>
    /// <para>
    /// The wiki behaviour settings used to bind to the doubled path
    /// <c>App:Domains:Wikis:Wikis</c>. The agreed correction for this slice is
    /// the singular, non-doubled path <c>App:Domains:Wiki</c>.
    /// </para>
    /// <para>
    /// This is parked deliberately (it is a public config-contract change that
    /// must re-key appsettings + secrets in lockstep). This test exists so the
    /// debt stays visible rather than living only in prose. It is
    /// <see cref="FactAttribute.Skip"/>-marked so it does not break the build for
    /// unrelated work; remove the <c>Skip</c> when starting the rename and make it
    /// pass per the acceptance criteria in the registry note.
    /// </para>
    /// </remarks>
    public class WikisConfigSectionNamingDebtTests
    {
        // The corrected path. When the rename is done, WikisConfigKeys.Wikis
        // must equal this, and the Skip below is removed.
        private const string IntendedRenamedSectionPath = "App:Domains:Wiki";

        [Fact(Skip = "Deferred by decision. See DOCUMENTATION/03.Governance/01.Registries/TECHNICAL-DEBT-RENAME-WIKIS-CONFIG-SECTION.md. Remove Skip to start the rename.")]
        public void WhenInspectingTheWikiSettingsSection_ThenItIsNotTheDoubledModuleName()
        {
            // Today this assertion should pass only once the old doubled/plural
            // path has been removed from the bound constant and host config.
            Assert.Equal(IntendedRenamedSectionPath, WikisConfigKeys.Wikis);

            // Defence: the doubled module name must be gone from the bound path.
            Assert.DoesNotContain("Wikis:Wikis", WikisConfigKeys.Wikis, System.StringComparison.Ordinal);
        }
    }
}
