using App.Modules.KWMODULENAME.Infrastructure.Constants;
using App.Modules.KWMODULENAME.Shared.Domains.Examples.Models.Implmentations;
using App.Modules.Sys.Infrastructure.Domains.Persistence.Relational.EF.Schema;
using App.Modules.Sys.Infrastructure.Persistence.EF.Schema.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace App.Modules.KWMODULENAME.Infrastructure.Domains.Examples.Configurations
{
	/// <summary>
	/// EF Core configuration for <see cref="ExampleA"/> entity.
	/// </summary>
	/// <remarks>
	/// ExampleA demonstrates all three relationship cardinalities:
	/// <list type="bullet">
	/// <item>*-1 to <see cref="ExampleType"/> (required reference via FK)</item>
	/// <item>1-* to <see cref="ExampleValueObject"/> (owned collection)</item>
	/// <item>*-* to <see cref="ExampleB"/> via <see cref="ExampleAExampleB"/> join entity</item>
	/// </list>
	/// </remarks>
	public sealed class ExampleAEFSchemaTypeConfiguration : IEFSchemaTypeConfiguration<ExampleA>
	{
		/// <inheritdoc />
		public void Configure(EntityTypeBuilder<ExampleA> builder)
		{
			int order = 0;

			// Phase 1: Table
			builder.DefineTable(DbSchemaTableNameConstants.ExampleAs, DbSchemaSchemaNameConstants.Examples);

			// Phase 2: Base entity (Id + Audit)
			builder.DefineDefaultEntityBase(ref order);

			// Phase 3: Entity-specific contracts
			builder.DefineIHasTitleAndDescription(ref order);
			builder.DefineBool(x => x.IsActive, ref order);

			// Phase 4: Relationships
			// *-1: Required reference to ExampleType (classifying reference data).
			// Uses the "with configured FK" overload so the FK column gets ordered and indexed.
			// No inverse collection — ExampleType doesn't need to navigate back to all ExampleAs.
			builder.DefineRequiredReferenceWithConfiguredFK<ExampleA, ExampleType>(
				a => a.ExampleType!,
				a => a.ExampleTypeFK,
				ref order,
				onDelete: DeleteBehavior.Restrict);

			// 1-*: Owned collection of ExampleValueObjects.
			// Each value object MUST belong to an ExampleA (required FK).
			builder.DefineOneToZeroOrManyRequired<ExampleA, ExampleValueObject>(
				a => a.ValueObjects,
				v => v.ExampleA!,
				v => v.ExampleAFK,
				onDelete: DeleteBehavior.Cascade);
		}
	}
}
