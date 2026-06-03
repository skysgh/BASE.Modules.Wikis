using App.Modules.Sys.Shared.Models;
using App.Modules.Sys.Shared.Models.Base;

namespace App.Modules.KWMODULENAME.Shared.Domains.Examples.Models.Implmentations
{
	/// <summary>
	/// Example entity B - demonstrates a child/related domain entity
	/// that references <see cref="ExampleA"/> via a foreign key.
	/// Inherits from <see cref="DefaultEntityBase"/> for standard
	/// identity, audit, and record-state plumbing.
	/// Replace with your actual domain entity when cloning.
	/// </summary>
	public class ExampleB : DefaultEntityBase, IHasName
	{
		/// <summary>
		/// Gets or sets the parent <see cref="ExampleA"/> identifier.
		/// </summary>
		public Guid ExampleAId { get; set; }

		/// <summary>
		/// Gets or sets the name.
		/// </summary>
		public string Name { get; set; } = string.Empty;

		/// <summary>
		/// Gets or sets the sort order.
		/// </summary>
		public int SortOrder { get; set; }



    }
}
