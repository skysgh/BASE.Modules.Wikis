using App.Modules.Sys.Shared.Models;
using App.Modules.Sys.Shared.Models.Base;

namespace App.Modules.KWMODULENAME.Shared.Domains.Examples.Models.Implmentations
{
	/// <summary>
	/// A value-object–style child of <see cref="ExampleA"/>.
	/// One-to-many: each <see cref="ExampleA"/> owns zero or more of these.
	/// </summary>
	/// <remarks>
	/// Inherits from <see cref="DefaultEntityBase"/> for Id, audit, and record-state.
	/// Implements <see cref="IHasName"/> for the standard Name property contract.
	/// </remarks>
	public class ExampleValueObject : DefaultEntityBase, IHasName, IHasDescription
	{
		/// <summary>
		/// Gets or sets the parent <see cref="ExampleA"/> foreign key.
		/// <para>Since it is navigable, the property name suffix is an 'FK', not 'Id'.</para>
		/// </summary>
		public Guid ExampleAFK { get; set; }

		/// <summary>
		/// Gets or sets the name of this value object.
		/// </summary>
		public string Name { get; set; } = string.Empty;


		/// <summary>
		/// Gets or sets an optional description.
		/// </summary>
		public string Description { get; set; } = string.Empty;

		/// <summary>
		/// Gets or sets the sort order hint for display purposes.
		/// </summary>
		public int SortOrder { get; set; }

		/// <summary>
		/// Navigation back to the owning <see cref="ExampleA"/>.
		/// </summary>
		public ExampleA? ExampleA { get; set; }
	}
}
