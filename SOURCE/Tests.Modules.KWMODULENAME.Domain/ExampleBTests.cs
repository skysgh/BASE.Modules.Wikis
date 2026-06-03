using App.Modules.KWMODULENAME.Shared.Domains.Examples.Models.Implmentations;

namespace Tests.Modules.KWMODULENAME.Domain
{
	/// <summary>
	/// Tests for <see cref="ExampleB"/> domain entity.
	/// Verifies entity construction, defaults, and FK constraints.
	/// </summary>
	public class ExampleBTests
	{
		[Fact]
		public void WhenCreated_ThenIdIsNotEmpty()
		{
			// Arrange & Act
			var entity = new ExampleB();

			// Assert — DefaultEntityBase auto-generates a Guid via UUIDFactory
			Assert.NotEqual(Guid.Empty, entity.Id);
		}

		[Fact]
		public void WhenCreated_ThenExampleAIdDefaultsToEmpty()
		{
			// Arrange & Act
			var entity = new ExampleB();

			// Assert — FK is not auto-generated, defaults to Guid.Empty
			Assert.Equal(Guid.Empty, entity.ExampleAId);
		}

		[Fact]
		public void WhenCreated_ThenNameDefaultsToEmpty()
		{
			// Arrange & Act
			var entity = new ExampleB();

			// Assert
			Assert.Equal(string.Empty, entity.Name);
		}

		[Fact]
		public void WhenCreated_ThenSortOrderDefaultsToZero()
		{
			// Arrange & Act
			var entity = new ExampleB();

			// Assert
			Assert.Equal(0, entity.SortOrder);
		}

		[Fact]
		public void WhenParentIdSet_ThenParentIdIsRetained()
		{
			// Arrange
			var entity = new ExampleB();
			var parentId = Guid.NewGuid();

			// Act
			entity.ExampleAId = parentId;

			// Assert
			Assert.Equal(parentId, entity.ExampleAId);
		}
	}
}
