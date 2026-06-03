using App.Modules.KWMODULENAME.Application.Domains.Examples.Dtos;
using App.Modules.KWMODULENAME.Shared.Domains.Examples.Models.Implmentations;
using App.Modules.Sys.Shared.ObjectMaps.Models;
using App.Modules.Sys.Shared.ObjectMaps.Models.Implementations.Base;

namespace App.Modules.KWMODULENAME.Application.Domains.Examples.Maps
{
	/// <summary>
	/// Maps <see cref="ExampleValueObject"/> entity to <see cref="ExampleValueObjectDto"/>.
	/// Discovered at startup via <see cref="IObjectMap"/> reflection scan.
	/// </summary>
	public class ExampleValueObjectToExampleValueObjectDtoMap : ObjectMapBase<ExampleValueObject, ExampleValueObjectDto>
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
			//    prefer the contract extension (e.g. MapGuidId
			//    when both implement IHasGuidId).
			// 4. For custom or cross-named properties use
			//    MapFrom(dest => dest.X, src => src.Y).
			// 5. Navigation / collection properties are NOT mapped.
			// -------------------------------------------------------------

			this.CreateMap()
				.MapGuidId()
				.MapFrom(dest => dest.ExampleAId, src => src.ExampleAFK)
				.MapFrom(dest => dest.Name, src => src.Name)
				.MapFrom(dest => dest.Description, src => src.Description)
				.MapFrom(dest => dest.SortOrder, src => src.SortOrder);
		}
	}
}
