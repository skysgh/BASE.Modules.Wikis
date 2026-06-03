using App.Modules.KWMODULENAME.Domain.Domains.Examples.Repositories;
using App.Modules.KWMODULENAME.Infrastructure.Data.EF;
using App.Modules.KWMODULENAME.Shared.Domains.Examples.Models.Implmentations;
using App.Modules.Sys.Infrastructure.Domains.Persistence.Relational.EF.Repositories.Implementations.Base;
using App.Modules.Sys.Shared.Domains.Diagnostics;

namespace App.Modules.KWMODULENAME.Infrastructure.Domains.Examples.Repositories.Implementations
{
	/// <summary>
	/// EF-backed repository for <see cref="ExampleValueObject"/>.
	/// </summary>
	public class ExampleValueObjectRepository : CrustStateRepositoryBase<ExampleValueObject>, IExampleValueObjectRepository
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ExampleValueObjectRepository"/> class.
		/// </summary>
		/// <param name="logger">Logger instance for diagnostics.</param>
		/// <param name="db">The module database context.</param>
		public ExampleValueObjectRepository(IAppLogger logger, ModuleDbContext db)
			: base(logger, db)
		{
		}

		/// <inheritdoc />
		/// <remarks>
		/// Value objects typically don't have domain-specific state transitions.
		/// Override if lifecycle semantics are added.
		/// </remarks>
		public override Task TransitionStateAsync(
			Guid id,
			string stateKey,
			CancellationToken cancellationToken = default)
		{
			throw new NotSupportedException(
				"ExampleValueObject does not support domain state transitions.");
		}
	}
}
