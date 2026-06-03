using App.Modules.Sys.Shared.Models.Base;

namespace App.Modules.KWMODULENAME.Shared.Domains.Examples.Models.Implmentations
{
	/// <summary>
	/// Explicit join entity linking <see cref="ExampleA"/> and <see cref="ExampleB"/>
	/// in a many-to-many relationship.
	/// </summary>
	/// <remarks>
	/// Inherits from <see cref="DefaultEntityBase"/> for Id, audit, and record-state.
	/// Prefer explicit join entities over EF Core implicit many-to-many
	/// so the join table gets full audit columns and can carry payload.
	/// </remarks>
	public class ExampleAExampleB : DefaultJoinEntityBase<ExampleA, ExampleB>
	{
		/// <summary>
		/// Gets or sets the <see cref="ExampleA"/> foreign key.
        /// <para>Since it is navigable, the property name suffix is an 'FK', not 'Id'</para>
		/// </summary>
		public Guid ExampleAFK { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="ExampleB"/> foreign key.
        /// <para>Since it is navigable, the property name suffix is an 'FK', not 'Id'</para>
        /// </summary>
        public Guid ExampleBFK { get; set; }

		/// <summary>
		/// Navigation to <see cref="ExampleA"/>.
		/// </summary>
		public ExampleA? ExampleA { get; set; }

		/// <summary>
		/// Navigation to <see cref="ExampleB"/>.
		/// </summary>
		public ExampleB? ExampleB { get; set; }

        /// <summary>
        /// Optional payload: notes or context about this association.
        /// </summary>
        public string? Notes { get; set; }


    }
}
