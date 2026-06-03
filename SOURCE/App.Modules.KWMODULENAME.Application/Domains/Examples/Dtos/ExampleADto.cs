using App.Modules.Sys.Shared.Models;
using App.Modules.Sys.Shared.Models.Persistence;

namespace App.Modules.KWMODULENAME.Application.Domains.Examples.Dtos
{
	/// <summary>
	/// Data transfer object for <see cref="Shared.Domains.Examples.Models.Implmentations.ExampleA"/>.
	/// Exposed to API consumers - never expose domain entities directly.
	/// </summary>
	public class ExampleADto :
		IHasGuidId,
		IHasTitleAndDescription
	{
		/// <inheritdoc/>
		public Guid Id { get; set; }

		/// <summary>Gets or sets the classifying ExampleType identifier.</summary>
		public Guid ExampleTypeId { get; set; }

		/// <inheritdoc/>
		public string Title { get; set; } = string.Empty;

		/// <inheritdoc/>
		public string Description { get; set; } = string.Empty;

		/// <summary>Gets or sets whether this entity is active.</summary>
		public bool IsActive { get; set; }
	}
}
