using App.Modules.KWMODULENAME.Application.Domains.Examples.Dtos;
using App.Modules.KWMODULENAME.Shared.Domains.Examples.Models.Implmentations;
using App.Modules.Sys.Shared.ObjectMaps.Models;
using App.Modules.Sys.Shared.ObjectMaps.Models.Implementations.Base;

namespace App.Modules.KWMODULENAME.Application.Domains.Examples.Maps
{
	/// <summary>
	/// Forward map: <see cref="ExampleA"/> entity to <see cref="ExampleADto"/>.
	/// Discovered at startup via <see cref="IObjectMap"/> reflection scan.
	/// </summary>
	/// <remarks>
	/// <para>
	/// Note the FK naming reversal: entity uses <c>ExampleTypeFK</c>
	/// (navigable convention) while the DTO uses <c>ExampleTypeId</c>
	/// (non-navigable convention).
	/// </para>
	/// </remarks>
	public class ExampleAToExampleADtoMap : ObjectMapBase<ExampleA, ExampleADto>
	{
		/// <inheritdoc/>
		protected override void ConfigureMapping()
		{
			// -- Mapping instructions ------------------------------------
			// 1. Map EVERY property explicitly, one by one.
			//    No auto-mapping / convention-based magic.
			// 2. Use OUR extension methods (MapGuidId, MapTitleAndDescription,
			//    MapFrom, etc.) - never vendor-specific helpers.
			//    This keeps the assembly free of object-mapping library refs.
			// 3. For contract-backed properties shared by both sides,
			//    prefer the contract extension (e.g. MapTitleAndDescription
			//    when both implement IHasTitleAndDescription).
			// 4. For custom or cross-named properties use
			//    MapFrom(dest => dest.X, src => src.Y).
			// 5. Navigation / collection properties are NOT mapped.
			// -------------------------------------------------------------

			this.CreateMap()
				.MapGuidId()
				.MapFrom(dest => dest.ExampleTypeId, src => src.ExampleTypeFK)
				.MapTitleAndDescription()
				.MapFrom(dest => dest.IsActive, src => src.IsActive);
		}
	}
}
