using App.Modules.KWMODULENAME.Shared.Domains.Examples.Enums;
using App.Modules.KWMODULENAME.Shared.Domains.Examples.Models.Implmentations;
using App.Modules.Sys.Infrastructure.Domains.Persistence.Relational.EF.Schema;
using App.Modules.Sys.Infrastructure.Domains.Persistence.Relational.EF.Schema.Implementations;
using App.Modules.Sys.Shared.Domains.Indexes;
using App.Modules.Sys.Shared.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace App.Modules.KWMODULENAME.Infrastructure.Domains.Examples.Seeding
{
	/// <summary>
	/// Seeds the <see cref="ExampleType"/> table
	/// from the <see cref="ExampleTypeCode"/> enum.
	/// </summary>
	/// <remarks>
	/// Discovered via reflection by <c>IModelBuilderOrchestrator</c>.
	/// Each enum value gets a deterministic Guid via <see cref="DeterministicGuid.FromEnum{TEnum}"/>.
	/// End users may add custom entries beyond these built-in values.
	/// </remarks>
	public sealed class ExampleEFTypeSeeder : EFDataSeederBase
    {
		/// <inheritdoc />
		public override void Seed(ModelBuilder modelBuilder)
		{
			List<ExampleType> entries = new List<ExampleType>();
			int order = 0;

			foreach (ExampleTypeCode value in Enum.GetValues<ExampleTypeCode>())
			{
				string name = value.ToString();
				bool isSentinel = value is ExampleTypeCode.Undefined
					or ExampleTypeCode.Unknown
					or ExampleTypeCode.Unspecified;

				entries.Add(new ExampleType
				{
					Id = DeterministicGuid.FromEnum(value),
					Key = name,
					Value = value.ToString(),
					Title = name,
					Description = "Example type: " + name + ".",
					Enabled = !isSentinel,
					ReferenceDataType = ReferenceDataType.System,
					EnumValue = (int)value,
					DisplayOrderHint = order++
				});
			}

			modelBuilder.Entity<ExampleType>().HasData(entries);
		}
	}
}
