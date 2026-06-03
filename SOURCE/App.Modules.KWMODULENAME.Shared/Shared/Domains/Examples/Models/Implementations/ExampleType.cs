using App.Modules.Sys.Shared.Models.Base;

namespace App.Modules.KWMODULENAME.Shared.Domains.Examples.Models.Implmentations
{
	/// <summary>
	/// Reference data entity that classifies <see cref="ExampleA"/> instances.
	/// Many-to-one: each <see cref="ExampleA"/> has one <see cref="ExampleType"/>.
	/// </summary>
	/// <remarks>
	/// Inherits from <see cref="DefaultReferenceDataEntityBase"/> which provides:
	/// Id (Guid, auto-generated), Key, Value, Title, Description, Enabled,
	/// ImageUrl, DisplayOrderHint, DisplayStyleHint, ReferenceDataType,
	/// FromUtc/ToUtc, Timestamp, RecordState, and full audit fields.
	/// </remarks>
	public class ExampleType : DefaultReferenceDataEntityBase
	{


        // IMPORTANT:
        // NOte that the underlying Guid Id is derived from the original enum value, of type <see cref="Enums.ExampleTypeCode"/>, 
        // using an enum/int to Guid conversion algorithm, so the EnumValue property is not strictly required to reconstruct the enum value from the Id.
        // This allows end users to add custom entries beyond the original enum values without needing to assign them an enum value (requiring rebuild),
        // while still preserving the ability to reconstruct the original enum value for seeded entries.

        /// <summary>
        /// The integer value from the original <see cref="Enums.ExampleTypeCode"/>.
        /// <c>null</c> for custom entries added beyond the built-in enum values.
        /// </summary>
        public int? EnumValue { get; set; }

	}
}
