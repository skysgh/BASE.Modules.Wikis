using App.Modules.KWMODULENAME.Shared.Domains.Examples.Models.Implmentations;

namespace Tests.Modules.KWMODULENAME.Domain
{
	/// <summary>
	/// Tests for <see cref="ExampleA"/> domain entity.
	/// Verifies entity construction, defaults, and basic constraints.
	/// </summary>
	public class ExampleATests
	{
		[Fact]
		public void WhenCreated_ThenIdIsNotEmpty()
		{
			// Arrange & Act
			var entity = new ExampleA();

			// Assert — DefaultEntityBase auto-generates a Guid via UUIDFactory
			Assert.NotEqual(Guid.Empty, entity.Id);
		}

		[Fact]
		public void WhenCreated_ThenTitleDefaultsToEmpty()
		{
			// Arrange & Act
			var entity = new ExampleA();

			// Assert
			Assert.Equal(string.Empty, entity.Title);
		}

		[Fact]
		public void WhenCreated_ThenDescriptionDefaultsToEmpty()
		{
			// Arrange & Act
			var entity = new ExampleA();

			// Assert
			Assert.Equal(string.Empty, entity.Description);
		}

		[Fact]
		public void WhenCreated_ThenIsActiveDefaultsToFalse()
		{
			// Arrange & Act
			var entity = new ExampleA();

			// Assert
			Assert.False(entity.IsActive);
		}

		[Fact]
		public void WhenTitleSet_ThenTitleIsRetained()
		{
			// Arrange
			var entity = new ExampleA();
			const string expected = "Test Title";

			// Act
			entity.Title = expected;

			// Assert
			Assert.Equal(expected, entity.Title);
		}

		[Fact]
		public void WhenCreated_ThenTwoInstancesHaveDifferentIds()
		{
			// Arrange & Act
			var a = new ExampleA();
			var b = new ExampleA();

			// Assert — each instance gets a unique auto-generated Id
			Assert.NotEqual(a.Id, b.Id);
		}
	}
}
