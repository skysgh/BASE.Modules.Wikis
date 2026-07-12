using App.Modules.Sys.Infrastructure.Services;
using App.Modules.Sys.Shared.Domains.Diagnostics;
using App.Modules.Wikis.Application.Domains.Wikis.Services.Implementations;
using App.Modules.Wikis.Application.Domains.Wikis.Structures.InTransit.Dtos;
using App.Modules.Wikis.Domain.Domains.Wikis.Entities.Implementations;
using App.Modules.Wikis.Domain.Domains.Wikis.Repositories;
using NSubstitute;

namespace Tests.Modules.Wikis.Application.Domains.Wikis.Services
{
    /// <summary>
    /// Phase-D gate tests for <see cref="WikiTemplateAppService"/> (ADR-018C
    /// templates-as-pages, build-plan Phase D step 15).
    /// <para>
    /// These exercise the CRUST orchestration contract the controller depends
    /// on: a create maps the write DTO to an entity, persists it through the
    /// repository, and maps the persisted entity back — preserving the template
    /// key, declared content format and owning-wiki FK unchanged through the
    /// round trip. An update must resolve the existing aggregate, re-stamp its
    /// identity, and persist it. These prove the wiring is correct without
    /// reaching a database.
    /// </para>
    /// </summary>
    public class WikiTemplateApplicationServiceTests
    {
        private static WikiTemplateAppService CreateService(
            IWikiTemplateRepository repository,
            IObjectMappingService mapper)
        {
            IAppLogger logger = Substitute.For<IAppLogger>();

            // ADR-027: authorization is no longer a constructor-injected demand
            // mechanism on the CRUST base — it is enforced by the ADR-020
            // pre-query / pre-commit persistence pipeline instead. These tests
            // stay focused on the map -> persist -> map-back orchestration; the
            // controller/integration layer is where authorization is exercised.
            return new WikiTemplateAppService(
                repository,
                mapper,
                logger);
        }

        [Fact]
        public async Task WhenTemplateIsCreated_ThenKeyFormatAndWikiFkAreRoundTrippedUnchanged()
        {
            Guid wikiId = Guid.NewGuid();
            WikiTemplateDto writeDto = new WikiTemplateDto
            {
                WikiFK = wikiId,
                Key = "decision-record",
                Title = "Decision Record",
                Description = "ADR scaffold",
                Enabled = true,
                ContentFormatKey = "markdown",
            };

            IWikiTemplateRepository repository = Substitute.For<IWikiTemplateRepository>();
            IObjectMappingService mapper = Substitute.For<IObjectMappingService>();

            // Map write DTO -> entity (the service maps before persisting).
            mapper.Map<WikiTemplateDto, WikiTemplate>(Arg.Any<WikiTemplateDto>())
                .Returns(callInfo =>
                {
                    WikiTemplateDto source = callInfo.Arg<WikiTemplateDto>();
                    return new WikiTemplate
                    {
                        WikiFK = source.WikiFK,
                        Key = source.Key,
                        Title = source.Title,
                        Description = source.Description,
                        Enabled = source.Enabled,
                        ContentFormatKey = source.ContentFormatKey,
                    };
                });

            // Persist echoes the entity back (assigning an identity).
            repository.CreateAsync(Arg.Any<WikiTemplate>(), Arg.Any<CancellationToken>())
                .Returns(callInfo =>
                {
                    WikiTemplate entity = callInfo.Arg<WikiTemplate>();
                    entity.Id = Guid.NewGuid();
                    return Task.FromResult(entity);
                });

            // Map entity -> read DTO (the service maps the persisted entity back).
            mapper.Map<WikiTemplate, WikiTemplateDto>(Arg.Any<WikiTemplate>())
                .Returns(callInfo =>
                {
                    WikiTemplate entity = callInfo.Arg<WikiTemplate>();
                    return new WikiTemplateDto
                    {
                        Id = entity.Id,
                        WikiFK = entity.WikiFK,
                        Key = entity.Key,
                        Title = entity.Title,
                        Description = entity.Description,
                        Enabled = entity.Enabled,
                        ContentFormatKey = entity.ContentFormatKey,
                    };
                });

            WikiTemplateAppService service = CreateService(repository, mapper);

            WikiTemplateDto readDto = await service.CreateAsync(writeDto, CancellationToken.None);

            Assert.NotEqual(Guid.Empty, readDto.Id);
            Assert.Equal(wikiId, readDto.WikiFK);
            Assert.Equal("decision-record", readDto.Key);
            Assert.Equal("markdown", readDto.ContentFormatKey);
            Assert.True(readDto.Enabled);

            await repository.Received(1).CreateAsync(
                Arg.Is<WikiTemplate>(template =>
                    template.Key == "decision-record" &&
                    template.ContentFormatKey == "markdown" &&
                    template.WikiFK == wikiId),
                Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task WhenTemplateIsUpdated_ThenExistingAggregateIsResolvedAndItsIdentityPreserved()
        {
            Guid templateId = Guid.NewGuid();
            Guid wikiId = Guid.NewGuid();

            WikiTemplateDto updateDto = new WikiTemplateDto
            {
                Id = templateId,
                WikiFK = wikiId,
                Key = "decision-record",
                Title = "Decision Record (revised)",
                Enabled = true,
                ContentFormatKey = "markdown",
            };

            WikiTemplate existing = new WikiTemplate
            {
                Id = templateId,
                WikiFK = wikiId,
                Key = "decision-record",
                Title = "Decision Record",
                ContentFormatKey = "markdown",
            };

            IWikiTemplateRepository repository = Substitute.For<IWikiTemplateRepository>();
            IObjectMappingService mapper = Substitute.For<IObjectMappingService>();

            repository.GetForUpdateAsync(templateId, Arg.Any<CancellationToken>())
                .Returns(Task.FromResult<WikiTemplate?>(existing));

            // The in-place map mutates the existing entity from the update DTO.
            mapper.When(callTo => callTo.Map<WikiTemplateDto, WikiTemplate>(
                    Arg.Any<WikiTemplateDto>(), Arg.Any<WikiTemplate>()))
                .Do(callInfo =>
                {
                    WikiTemplateDto source = callInfo.Arg<WikiTemplateDto>();
                    WikiTemplate destination = callInfo.Arg<WikiTemplate>();
                    destination.Title = source.Title;
                    destination.Enabled = source.Enabled;
                    destination.ContentFormatKey = source.ContentFormatKey;
                });

            repository.UpdateAsync(Arg.Any<WikiTemplate>(), Arg.Any<CancellationToken>())
                .Returns(callInfo => Task.FromResult(callInfo.Arg<WikiTemplate>()));

            mapper.Map<WikiTemplate, WikiTemplateDto>(Arg.Any<WikiTemplate>())
                .Returns(callInfo =>
                {
                    WikiTemplate entity = callInfo.Arg<WikiTemplate>();
                    return new WikiTemplateDto
                    {
                        Id = entity.Id,
                        WikiFK = entity.WikiFK,
                        Key = entity.Key,
                        Title = entity.Title,
                        Enabled = entity.Enabled,
                        ContentFormatKey = entity.ContentFormatKey,
                    };
                });

            WikiTemplateAppService service = CreateService(repository, mapper);

            WikiTemplateDto readDto = await service.UpdateAsync(templateId, updateDto, CancellationToken.None);

            Assert.Equal(templateId, readDto.Id);
            Assert.Equal("Decision Record (revised)", readDto.Title);

            await repository.Received(1).GetForUpdateAsync(templateId, Arg.Any<CancellationToken>());
            await repository.Received(1).UpdateAsync(
                Arg.Is<WikiTemplate>(template => template.Id == templateId),
                Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task WhenUpdatingAMissingTemplate_ThenItThrowsRatherThanSilentlyCreatingOne()
        {
            Guid missingId = Guid.NewGuid();
            WikiTemplateDto updateDto = new WikiTemplateDto
            {
                Id = missingId,
                Key = "decision-record",
            };

            IWikiTemplateRepository repository = Substitute.For<IWikiTemplateRepository>();
            IObjectMappingService mapper = Substitute.For<IObjectMappingService>();

            repository.GetForUpdateAsync(missingId, Arg.Any<CancellationToken>())
                .Returns(Task.FromResult<WikiTemplate?>(null));

            WikiTemplateAppService service = CreateService(repository, mapper);

            await Assert.ThrowsAsync<InvalidOperationException>(
                () => service.UpdateAsync(missingId, updateDto, CancellationToken.None));

            await repository.DidNotReceive().UpdateAsync(
                Arg.Any<WikiTemplate>(), Arg.Any<CancellationToken>());
        }
    }
}
