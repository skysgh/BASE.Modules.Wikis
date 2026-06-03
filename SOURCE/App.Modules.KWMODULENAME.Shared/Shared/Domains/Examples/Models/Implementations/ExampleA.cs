using App.Modules.Sys.Shared.Models;
using App.Modules.Sys.Shared.Models.Base;

namespace App.Modules.KWMODULENAME.Shared.Domains.Examples.Models.Implmentations
{
	/// <summary>
	/// Example entity A - demonstrates a domain entity inheriting from
	/// <see cref="DefaultEntityBase"/> with title, description, active flag,
	/// and a foreign key to <see cref="ExampleType"/> reference data.
	/// Replace with your actual domain entity when cloning.
	/// </summary>
	public class ExampleA : DefaultEntityBase, IHasTitleAndDescription
	{
        /// <summary>
        /// Gets or sets the <see cref="ExampleType"/> foreign key.
        /// Classifies this entity by a reference-data type.
        /// <para>Since it is navigable, the property name suffix is an 'FK', not 'Id'</para>
        /// </summary>
        public Guid ExampleTypeFK { get; set; }

		/// <summary>
		/// Gets or sets the title.
		/// </summary>
		public string Title { get; set; } = string.Empty;

		/// <summary>
		/// Gets or sets the description.
		/// </summary>
		public string Description { get; set; } = string.Empty;

		/// <summary>
		/// Gets or sets whether this entity is active.
		/// </summary>
		public bool IsActive { get; set; }

		/// <summary>
		/// Navigation to the classifying <see cref="ExampleType"/>.
		/// </summary>
		public ExampleType? ExampleType { get; set; }


        /// <summary>
        /// Navigation: the value objects owned by this example type.
        /// </summary>
        public ICollection<ExampleValueObject> ValueObjects { get; set; } = new List<ExampleValueObject>();

    }
}
