using App.Modules.KWMODULENAME.Infrastructure.Constants;
using App.Modules.KWMODULENAME.Shared.Domains.Examples.Models.Implmentations;
using App.Modules.Sys.Infrastructure.Domains.Persistence.Relational.EF.Schema;
using App.Modules.Sys.Infrastructure.Persistence.EF.Schema.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace App.Modules.KWMODULENAME.Infrastructure.Domains.Examples.Configurations
{
	/// <summary>
	/// EF Core configuration for the <see cref="ExampleAExampleB"/> explicit join entity.
	/// </summary>
	/// <remarks>
	/// Composite key (no surrogate Id) — audit columns inherited from
	/// <see cref="Sys.Shared.Models.Base.DefaultNoneIdEntityBase"/>.
	/// <c>DefineExplicitJoinEntity</c> configures composite key, both FK relationships, and indexes.
	/// </remarks>
	public sealed class ExampleAExampleBEFSchemaTypeConfiguration : IEFSchemaTypeConfiguration<ExampleAExampleB>
	{
		/// <inheritdoc />
		public void Configure(EntityTypeBuilder<ExampleAExampleB> builder)
		{
			int order = 0;

			// Phase 1: Table
			builder.DefineTable(DbSchemaTableNameConstants.ExampleAExampleBs, DbSchemaSchemaNameConstants.Examples);

			// Phase 2: Base entity (Audit only, no Id — composite key entity)
			builder.DefineDefaultNoneIdEntityBase(ref order);

			// Phase 3: Payload
			builder.DefineString(x => x.Notes, ref order, isRequired: false, maxLength: 2000, unicode: true);

			// Phase 4: Navigable relationships (composite key, FK navigations, indexes)
			builder.DefineExplicitJoinEntity<ExampleAExampleB, ExampleA, ExampleB>(
				joinToLeft: j => j.ExampleA!,
				leftForeignKey: j => j.ExampleAFK,
				joinToRight: j => j.ExampleB!,
				rightForeignKey: j => j.ExampleBFK,
				onDeleteLeft: DeleteBehavior.Restrict,
				onDeleteRight: DeleteBehavior.Restrict);
		}
	}
}
