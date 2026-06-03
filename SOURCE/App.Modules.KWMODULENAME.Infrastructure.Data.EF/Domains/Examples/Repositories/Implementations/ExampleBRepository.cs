using App.Modules.KWMODULENAME.Domain.Domains.Examples.Repositories;
using App.Modules.KWMODULENAME.Infrastructure.Data.EF;
using App.Modules.KWMODULENAME.Shared.Domains.Examples.Models.Implmentations;
using App.Modules.Sys.Infrastructure.Domains.Persistence.Relational.EF.Repositories.Implementations.Base;
using App.Modules.Sys.Shared.Domains.Diagnostics;

namespace App.Modules.KWMODULENAME.Infrastructure.Domains.Persistence.Relational.EF.Repositories.Implementations
{
	/// <summary>
	/// CRUST repository for <see cref="ExampleB"/> entities.
	/// Extends <see cref="CrustStateRepositoryBase{T}"/> with
	/// domain-specific state transition logic.
	/// </summary>
	public class ExampleBRepository : CrustStateRepositoryBase<ExampleB>, IExampleBRepository
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ExampleBRepository"/> class.
		/// </summary>
		/// <param name="logger">Logger instance for diagnostics.</param>
		/// <param name="db">The module database context.</param>
		public ExampleBRepository(IAppLogger logger, ModuleDbContext db)
			: base(logger, db)
		{
		}

		/// <inheritdoc/>
		/// <remarks>
		/// Domain-specific transition validation should be added here when
		/// business rules for ExampleB lifecycle are defined.
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

			ExampleB? entity = await this.GetForUpdateAsync(id, cancellationToken);
			if (entity is null)
			{
				throw new InvalidOperationException(
					"ExampleB with ID " + id + " was not found.");
			}

			await this.DbContext.SaveChangesAsync(cancellationToken);
			this.LoggerService.LogInformation(
				"Transitioned ExampleB " + id + " to state " + stateKey);
		}
	}
}
