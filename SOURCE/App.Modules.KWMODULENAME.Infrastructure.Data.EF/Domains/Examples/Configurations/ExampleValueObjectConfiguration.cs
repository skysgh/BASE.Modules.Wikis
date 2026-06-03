using App.Modules.KWMODULENAME.Infrastructure.Constants;
using App.Modules.KWMODULENAME.Shared.Domains.Examples.Models.Implmentations;
using App.Modules.Sys.Infrastructure.Domains.Persistence.Relational.EF.Schema;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace App.Modules.KWMODULENAME.Infrastructure.Domains.Examples.Configurations
{
	/// <summary>
	/// EF Core configuration for <see cref="ExampleValueObject"/> entity.
	/// </summary>
	/// <remarks>
	/// The one-to-many relationship (ExampleA → ExampleValueObjects) is configured
	/// in <see cref="ExampleAEFSchemaTypeConfiguration"/> via
	/// <c>DefineOneToZeroOrManyRequired</c>. This configuration handles table,
	/// base entity, and scalar properties only.
	/// </remarks>
	public sealed class ExampleValueObjectEFSchemaTypeConfiguration : IEFSchemaTypeConfiguration<ExampleValueObject>
	{
		/// <inheritdoc />
		public void Configure(EntityTypeBuilder<ExampleValueObject> builder)
		{
			int order = 0;

			// Phase 1: Table
			builder.DefineTable(DbSchemaTableNameConstants.ExampleValueObjects, DbSchemaSchemaNameConstants.Examples);

			// Phase 2: Base entity (Id + Audit)
			builder.DefineDefaultEntityBase(ref order);

			// Phase 3: Entity-specific contracts
			// Note: ExampleAFK relationship is configured from ExampleA's config.
			builder.DefineIHasName(ref order);
			builder.DefineIHasDescription(ref order);
			builder.DefineInt(x => x.SortOrder, ref order, isRequired: true);
		}
	}
}
