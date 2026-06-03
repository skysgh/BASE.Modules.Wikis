using App.Modules.KWMODULENAME.Application.Domains.Examples.Dtos;
using App.Modules.KWMODULENAME.Application.Domains.Examples.Services.Implementations;
using App.Modules.KWMODULENAME.Shared.Domains.Examples.Models.Implmentations;
using App.Modules.Sys.Infrastructure.Services;
using App.Modules.Sys.Shared.Domains.Diagnostics;
using App.Modules.Sys.Shared.Repositories;
using NSubstitute;

namespace Tests.Modules.KWMODULENAME.Application
{
	/// <summary>
	/// Tests for <see cref="ExampleAApplicationService"/>.
	/// Verifies that the CRUST service correctly delegates to the
	/// repository and mapper via the base class plumbing.
	/// </summary>
	public class ExampleAApplicationServiceTests
	{
		private readonly ICrustStateRepository<ExampleA> _repository;
		private readonly IObjectMappingService _mapper;
		private readonly IAppLogger _logger;
		private readonly ExampleAApplicationService _service;

		/// <summary>
		/// Sets up mocked dependencies for each test.
		/// </summary>
		public ExampleAApplicationServiceTests()
		{
			this._repository = Substitute.For<ICrustStateRepository<ExampleA>>();
			this._mapper = Substitute.For<IObjectMappingService>();
			this._logger = Substitute.For<IAppLogger>();
			this._service = new ExampleAApplicationService(
				this._repository, this._mapper, this._logger);
		}

		[Fact]
		public void WhenQueryCalled_ThenProjectToIsInvokedOnce()
		{
			// Arrange
			var entities = new List<ExampleA>().AsQueryable();
			var expectedDtos = new List<ExampleADto>().AsQueryable();
			this._repository.Query().Returns(entities);
			this._mapper
				.ProjectTo<ExampleA, ExampleADto>(Arg.Any<IQueryable<ExampleA>>())
				.Returns(expectedDtos);

			// Act
			var result = this._service.Query();

			// Assert
			Assert.NotNull(result);
			this._mapper.Received(1)
				.ProjectTo<ExampleA, ExampleADto>(Arg.Any<IQueryable<ExampleA>>());
		}

		[Fact]
		public void WhenQueryByIdCalled_ThenProjectToIsInvokedOnce()
		{
			// Arrange
			var id = Guid.NewGuid();
			var entities = new List<ExampleA>().AsQueryable();
			var expectedDtos = new List<ExampleADto>().AsQueryable();
			this._repository.QueryById(id).Returns(entities);
			this._mapper
				.ProjectTo<ExampleA, ExampleADto>(Arg.Any<IQueryable<ExampleA>>())
				.Returns(expectedDtos);

			// Act
			var result = this._service.QueryById(id);

			// Assert
			Assert.NotNull(result);
			this._mapper.Received(1)
				.ProjectTo<ExampleA, ExampleADto>(Arg.Any<IQueryable<ExampleA>>());
		}

		[Fact]
		public void WhenConstructedWithNullRepository_ThenThrowsArgumentNullException()
		{
			// Arrange, Act & Assert
			Assert.Throws<ArgumentNullException>(() =>
				new ExampleAApplicationService(null!, this._mapper, this._logger));
		}

		[Fact]
		public void WhenConstructedWithNullMapper_ThenThrowsArgumentNullException()
		{
			// Arrange, Act & Assert
			Assert.Throws<ArgumentNullException>(() =>
				new ExampleAApplicationService(this._repository, null!, this._logger));
		}
	}
}
