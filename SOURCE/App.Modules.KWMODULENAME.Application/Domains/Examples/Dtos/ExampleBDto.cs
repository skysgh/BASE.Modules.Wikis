using App.Modules.Sys.Shared.Models;
using App.Modules.Sys.Shared.Models.Persistence;

namespace App.Modules.KWMODULENAME.Application.Domains.Examples.Dtos
{
	/// <summary>
	/// Data transfer object for <see cref="Shared.Domains.Examples.Models.Implmentations.ExampleB"/>.
	/// Exposed to API consumers - never expose domain entities directly.
	/// </summary>
	public class ExampleBDto :
		IHasGuidId,
		IHasName
	{
		/// <inheritdoc/>
		public Guid Id { get; set; }

		/// <summary>
		/// Gets or sets the parent ExampleA identifier.
		/// </summary>
		public Guid ExampleAId { get; set; }

		/// <inheritdoc/>
		public string Name { get; set; } = string.Empty;

		/// <summary>
		/// Gets or sets the sort order.
		/// </summary>
		public int SortOrder { get; set; }
	}
}
