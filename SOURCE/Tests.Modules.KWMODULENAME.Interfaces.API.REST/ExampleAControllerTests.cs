using App.Modules.KWMODULENAME.Application.Domains.Examples.Dtos;
using App.Modules.KWMODULENAME.Application.Domains.Examples.Services;
using App.Modules.KWMODULENAME.Interfaces.API.REST.Domains.V1.Examples;
using NSubstitute;

namespace Tests.Modules.KWMODULENAME.Interfaces.API.REST
{
	/// <summary>
	/// Tests for <see cref="ExampleAController"/>.
	/// Verifies the CRUST controller delegates to the service
	/// via the base class plumbing.
	/// </summary>
	public class ExampleAControllerTests
	{
		private readonly IExampleAApplicationService _mockService;
		private readonly ExampleAController _controller;

		/// <summary>
		/// Sets up a mocked service and controller for each test.
		/// </summary>
		public ExampleAControllerTests()
		{
			this._mockService = Substitute.For<IExampleAApplicationService>();
			this._controller = new ExampleAController(this._mockService);
		}

		[Fact]
		public void WhenGetAllCalled_ThenDelegatesToServiceQuery()
		{
			// Arrange
			var expectedDtos = new List<ExampleADto>
			{
				new ExampleADto { Id = Guid.NewGuid(), Title = "First" },
				new ExampleADto { Id = Guid.NewGuid(), Title = "Second" }
			}.AsQueryable();

			this._mockService.Query().Returns(expectedDtos);

			// Act
			var result = this._controller.GetAll();

			// Assert
			Assert.NotNull(result);
			Assert.Equal(2, result.Count());
			this._mockService.Received(1).Query();
		}

		[Fact]
		public void WhenGetByIdCalled_ThenDelegatesToServiceQueryById()
		{
			// Arrange
			var id = Guid.NewGuid();
			var expectedDtos = new List<ExampleADto>
			{
				new ExampleADto { Id = id, Title = "Found" }
			}.AsQueryable();

			this._mockService.QueryById(id).Returns(expectedDtos);

			// Act
			var result = this._controller.GetById(id);

			// Assert
			Assert.NotNull(result);
			Assert.Single(result);
			this._mockService.Received(1).QueryById(id);
		}

		[Fact]
		public void WhenConstructedWithNullService_ThenThrowsArgumentNullException()
		{
			// Arrange, Act & Assert
			Assert.Throws<ArgumentNullException>(() =>
				new ExampleAController(null!));
		}
	}
}
