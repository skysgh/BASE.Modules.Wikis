using App.Modules.KWMODULENAME.Domain.Domains.Examples.Repositories;
using App.Modules.KWMODULENAME.Infrastructure.Data.EF;
using App.Modules.KWMODULENAME.Shared.Domains.Examples.Models.Implmentations;
using App.Modules.Sys.Infrastructure.Domains.Persistence.Relational.EF.Repositories.Implementations.Base;
using App.Modules.Sys.Shared.Domains.Diagnostics;

namespace App.Modules.KWMODULENAME.Infrastructure.Domains.Persistence.Relational.EF.Repositories.Implementations
{
	/// <summary>
	/// CRUST repository for <see cref="ExampleA"/> entities.
	/// Extends <see cref="CrustStateRepositoryBase{T}"/> with
	/// domain-specific state transition logic.
	/// </summary>
	public class ExampleARepository : CrustStateRepositoryBase<ExampleA>, IExampleARepository
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ExampleARepository"/> class.
		/// </summary>
		/// <param name="logger">Logger instance for diagnostics.</param>
		/// <param name="db">The module database context.</param>
		public ExampleARepository(IAppLogger logger, ModuleDbContext db)
			: base(logger, db)
		{
		}

		/// <inheritdoc/>
		/// <remarks>
		/// Updates the <see cref="ExampleA.IsActive"/> property based on the requested state.
		/// Override to add domain-specific transition validation when business rules are defined.
		/// </remarks>
		public override async Task TransitionStateAsync(
			Guid id,
			string stateKey,
			CancellationToken cancellationToken = default)
		{
			if (string.IsNullOrWhiteSpace(stateKey))
			{
				throw new ArgumentException("State cannot be null or whitespace.", nameof(stateKey));
			}

			ExampleA? entity = await this.GetForUpdateAsync(id, cancellationToken);
			if (entity is null)
			{
				throw new InvalidOperationException(
					"ExampleA with ID " + id + " was not found.");
			}

			entity.IsActive = string.Equals(stateKey, "Active", StringComparison.OrdinalIgnoreCase);

			await this.DbContext.SaveChangesAsync(cancellationToken);
			this.LoggerService.LogInformation(
				"Transitioned ExampleA " + id + " to state " + stateKey);
		}
	}
}
