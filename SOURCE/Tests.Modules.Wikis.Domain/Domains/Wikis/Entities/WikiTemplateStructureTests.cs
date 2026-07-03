using App.Modules.Wikis.Domain.Domains.Wikis.Entities.Implementations;

namespace Tests.Modules.Wikis.Domain.Domains.Wikis.Entities
{
    /// <summary>
    /// Phase-D gate tests for the <see cref="WikiTemplate"/> scaffold model
    /// (ADR-018C templates-as-pages, build-plan Phase D step 15).
    /// <para>
    /// These characterise the structural invariants the scaffolding and lint
    /// stages depend on: a template owns an ordered set of sections whose order
    /// is <b>determinate</b> (logic ordering via <c>PrecedenceOrder</c>,
    /// not a cosmetic display hint), required-ness drives advisory lint, and a
    /// binding scopes by either a slug prefix or a page subtree with a
    /// deterministic precedence winner when several overlap.
    /// </para>
    /// </summary>
    public class WikiTemplateStructureTests
    {
        private static readonly string[] ExpectedScaffoldOrder = { "context", "decision", "consequences" };
        private static readonly string[] ExpectedRequiredKeys = { "context" };

        [Fact]
        public void WhenTemplateIsFreshlyCreated_ThenItHasNoSectionsAndNoBindings()
        {
            WikiTemplate template = new WikiTemplate
            {
                WikiFK = Guid.NewGuid(),
                Key = "decision-record",
                Title = "Decision Record",
                ContentFormatKey = "markdown",
            };

            Assert.Empty(template.Sections);
            Assert.Empty(template.Bindings);
            Assert.False(template.Enabled);
        }

        [Fact]
        public void WhenSectionsAreAddedOutOfOrder_ThenPrecedenceOrderEstablishesTheScaffoldSequence()
        {
            // Sections are deliberately appended out of their intended order to
            // prove the scaffold sequence is governed by PrecedenceOrder (logic
            // ordering), never by insertion/collection order.
            WikiTemplate template = new WikiTemplate
            {
                Id = Guid.NewGuid(),
                Key = "decision-record",
            };

            WikiTemplateSection consequences = new WikiTemplateSection
            {
                WikiTemplateFK = template.Id,
                Key = "consequences",
                Title = "Consequences",
                PrecedenceOrder = 2,
            };
            WikiTemplateSection context = new WikiTemplateSection
            {
                WikiTemplateFK = template.Id,
                Key = "context",
                Title = "Context",
                PrecedenceOrder = 0,
            };
            WikiTemplateSection decision = new WikiTemplateSection
            {
                WikiTemplateFK = template.Id,
                Key = "decision",
                Title = "Decision",
                PrecedenceOrder = 1,
            };

            template.Sections.Add(consequences);
            template.Sections.Add(context);
            template.Sections.Add(decision);

            List<string> scaffoldOrder = template.Sections
                .OrderBy(section => section.PrecedenceOrder)
                .Select(section => section.Key)
                .ToList();

            Assert.Equal(ExpectedScaffoldOrder, scaffoldOrder);
        }

        [Fact]
        public void WhenSectionIsRequired_ThenItIsDistinguishedFromOptionalSectionsForLint()
        {
            // The structural lint only raises an advisory finding for a *required*
            // section that is missing from a page; optional sections never do.
            WikiTemplateSection required = new WikiTemplateSection
            {
                Key = "context",
                IsRequired = true,
            };
            WikiTemplateSection optional = new WikiTemplateSection
            {
                Key = "notes",
                IsRequired = false,
            };

            WikiTemplate template = new WikiTemplate { Id = Guid.NewGuid() };
            template.Sections.Add(required);
            template.Sections.Add(optional);

            List<string> requiredKeys = template.Sections
                .Where(section => section.IsRequired)
                .Select(section => section.Key)
                .ToList();

            Assert.Equal(ExpectedRequiredKeys, requiredKeys);
        }

        [Fact]
        public void WhenBindingScopesBySlugPrefix_ThenItHasNoPageScope()
        {
            // A namespace binding scopes by slug prefix and leaves the page-scope
            // FK null: the two scoping modes are mutually exclusive.
            WikiTemplateBinding binding = new WikiTemplateBinding
            {
                WikiTemplateFK = Guid.NewGuid(),
                WikiId = Guid.NewGuid(),
                ScopeSlugPrefix = "how-to/",
                Enabled = true,
            };

            Assert.Null(binding.ScopeWikiPageFK);
            Assert.Equal("how-to/", binding.ScopeSlugPrefix);
        }

        [Fact]
        public void WhenBindingScopesByPageSubtree_ThenItHasNoSlugPrefix()
        {
            WikiTemplateBinding binding = new WikiTemplateBinding
            {
                WikiTemplateFK = Guid.NewGuid(),
                WikiId = Guid.NewGuid(),
                ScopeWikiPageFK = Guid.NewGuid(),
                Enabled = true,
            };

            Assert.NotNull(binding.ScopeWikiPageFK);
            Assert.Equal(string.Empty, binding.ScopeSlugPrefix);
        }

        [Fact]
        public void WhenSeveralBindingsOverlap_ThenLowerPrecedenceOrderWinsDeterministically()
        {
            // When more than one enabled binding could apply to a page, the one
            // with the lowest PrecedenceOrder governs. This proves the selection
            // is deterministic and ordering-driven, not arbitrary.
            WikiTemplateBinding wholeWiki = new WikiTemplateBinding
            {
                ScopeSlugPrefix = string.Empty,
                PrecedenceOrder = 100,
                Enabled = true,
            };
            WikiTemplateBinding namespaceScoped = new WikiTemplateBinding
            {
                ScopeSlugPrefix = "how-to/",
                PrecedenceOrder = 50,
                Enabled = true,
            };
            WikiTemplateBinding pageScoped = new WikiTemplateBinding
            {
                ScopeWikiPageFK = Guid.NewGuid(),
                PrecedenceOrder = 10,
                Enabled = true,
            };

            List<WikiTemplateBinding> candidates = new List<WikiTemplateBinding>
            {
                wholeWiki,
                namespaceScoped,
                pageScoped,
            };

            WikiTemplateBinding winner = candidates
                .Where(binding => binding.Enabled)
                .OrderBy(binding => binding.PrecedenceOrder)
                .First();

            Assert.Same(pageScoped, winner);
        }

        [Fact]
        public void WhenBindingIsDisabled_ThenItIsExcludedFromSelectionEvenIfItHasHighestPrecedence()
        {
            // A disabled binding must never govern, regardless of how favourable
            // its precedence is. Enabled-ness gates candidacy before ordering.
            WikiTemplateBinding disabledFavourite = new WikiTemplateBinding
            {
                ScopeWikiPageFK = Guid.NewGuid(),
                PrecedenceOrder = 1,
                Enabled = false,
            };
            WikiTemplateBinding enabledFallback = new WikiTemplateBinding
            {
                ScopeSlugPrefix = "how-to/",
                PrecedenceOrder = 50,
                Enabled = true,
            };

            List<WikiTemplateBinding> candidates = new List<WikiTemplateBinding>
            {
                disabledFavourite,
                enabledFallback,
            };

            WikiTemplateBinding? winner = candidates
                .Where(binding => binding.Enabled)
                .OrderBy(binding => binding.PrecedenceOrder)
                .FirstOrDefault();

            Assert.Same(enabledFallback, winner);
        }
    }
}
