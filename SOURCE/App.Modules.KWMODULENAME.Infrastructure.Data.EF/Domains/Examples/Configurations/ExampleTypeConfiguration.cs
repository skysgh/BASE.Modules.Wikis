using App.Modules.KWMODULENAME.Infrastructure.Constants;
using App.Modules.KWMODULENAME.Shared.Domains.Examples.Models.Implmentations;
using App.Modules.Sys.Infrastructure.Domains.Persistence.Relational.EF.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace App.Modules.KWMODULENAME.Infrastructure.Domains.Examples.Configurations
{
	/// <summary>
	/// EF Core configuration for <see cref="ExampleType"/> reference-data entity.
	/// </summary>
	/// <remarks>
	/// Pattern: Table -> PK -> Contracts (in column order) -> Custom -> Timestamp/Audit.
	/// Seed data is in <see cref="Seeding.ExampleEFTypeSeeder"/>.
	/// </remarks>
	public sealed class ExampleTypeEFSchemaTypeConfiguration : IEFSchemaTypeConfiguration<ExampleType>
	{
		/// <inheritdoc />
		public void Configure(EntityTypeBuilder<ExampleType> builder)
		{
			int order = 0;

			// Phase 1: Table
			builder.DefineTable(DbSchemaTableNameConstants.ExampleTypes, DbSchemaSchemaNameConstants.ReferenceData);

			// Phase 2: Reference-data base (Id, Key, Enabled, Title/Desc, Display, ReferenceDataType, Value, FromTo)
			builder.DefineDefaultReferenceDataEntityBase(ref order);

			// Phase 3: Entity-specific properties
			builder.DefineInt(x => x.EnumValue, ref order, isRequired: false);

			// Phase 4: Indexes
			builder.HasIndex(e => e.EnumValue)
				.HasDatabaseName($"IX_{DbSchemaTableNameConstants.ExampleTypes}_EnumValue")
				.HasFilter("[EnumValue] IS NOT NULL")
				.IsUnique();
		}
	}
}
