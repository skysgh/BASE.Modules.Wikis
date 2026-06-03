using App.Modules.KWMODULENAME.Application.Domains.Examples.Dtos;
using App.Modules.KWMODULENAME.Shared.Domains.Examples.Models.Implmentations;
using App.Modules.Sys.Shared.ObjectMaps.Models;
using App.Modules.Sys.Shared.ObjectMaps.Models.Implementations.Base;

namespace App.Modules.KWMODULENAME.Application.Domains.Examples.Maps
{
	/// <summary>
	/// Reverse map: <see cref="ExampleTypeDto"/> to <see cref="ExampleType"/> entity.
	/// Uses individual map extensions matching the display-facing subset
	/// that <c>ReferenceDataBaseDto</c> exposes to API consumers.
	/// Discovered at startup via <see cref="IObjectMap"/> reflection scan.
	/// </summary>
	/// <remarks>
	/// <para>
	/// Cannot use <c>MapReferenceData()</c> because it requires both sides
	/// to implement <c>IHasReferenceData</c>, which includes infrastructure
	/// contracts (Timestamp, RecordState, Audit) that
	/// <c>ReferenceDataBaseDto</c> intentionally omits.
	/// </para>
	/// <para>
	/// <c>EnumValue</c> is entity-only (no DTO counterpart) and is NOT mapped.
	/// </para>
	/// </remarks>
	public class ExampleTypeDtoToExampleTypeMap : ObjectMapBase<ExampleTypeDto, ExampleType>
	{
		/// <inheritdoc/>
		protected override void ConfigureMapping()
		{
			// -- Mapping instructions ------------------------------------
			// 1. Map EVERY property explicitly, one by one.
			//    No auto-mapping / convention-based magic.
			// 2. Use OUR extension methods (MapGuidId, MapTitleAndDescription,
			//    MapFrom, etc.) — never vendor-specific helpers.
			//    This keeps the assembly free of object-mapping library refs.
			// 3. For contract-backed properties shared by both sides,
			//    prefer the contract extension (e.g. MapTitleAndDescription
			//    when both implement IHasTitleAndDescription).
			// 4. For custom or cross-named properties use
			//    MapFrom(dest => dest.X, src => src.Y).
			// 5. Navigation / collection properties are NOT mapped.
			// 6. Cannot use MapReferenceData() here — see class remarks.
			// -------------------------------------------------------------

			this.CreateMap()
				.MapGuidId()
				.MapEnabled()
				.MapTitleAndDescription()
				.MapImage()
				.MapDisplayHint()
				.MapKey()
				.MapFrom(dest => dest.ReferenceDataType, src => src.ReferenceDataType);
		}
	}
}
