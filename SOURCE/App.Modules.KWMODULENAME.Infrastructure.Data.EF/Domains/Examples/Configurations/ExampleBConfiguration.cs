using App.Modules.KWMODULENAME.Infrastructure.Constants;
using App.Modules.KWMODULENAME.Shared.Domains.Examples.Models.Implmentations;
using App.Modules.Sys.Infrastructure.Domains.Persistence.Relational.EF.Schema;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace App.Modules.KWMODULENAME.Infrastructure.Domains.Examples.Configurations
{
	/// <summary>
	/// EF Core configuration for <see cref="ExampleB"/> entity.
	/// </summary>
	/// <remarks>
	/// ExampleB references ExampleA via a non-navigable half-relationship
	/// (<c>ExampleAId</c> — suffix is 'Id' not 'FK' because there is no navigation property).
	/// </remarks>
	public sealed class ExampleBEFSchemaTypeConfiguration : IEFSchemaTypeConfiguration<ExampleB>
	{
		/// <inheritdoc />
		public void Configure(EntityTypeBuilder<ExampleB> builder)
		{
			int order = 0;

			// Phase 1: Table
			builder.DefineTable(DbSchemaTableNameConstants.ExampleBs, DbSchemaSchemaNameConstants.Examples);

			// Phase 2: Base entity (Id + Audit)
			builder.DefineDefaultEntityBase(ref order);

			// Phase 3: Half-relationship FK to ExampleA (no navigation, so '*Id' suffix).
			builder.DefineRequiredAggregateId(
				x => x.ExampleAId,
				ref order,
				optionalIndexName: $"IX_{DbSchemaTableNameConstants.ExampleBs}_ExampleAId");

			// Phase 4: Entity-specific contracts
			builder.DefineIHasName(ref order);
			builder.DefineInt(x => x.SortOrder, ref order, isRequired: true);
		}
	}
}
