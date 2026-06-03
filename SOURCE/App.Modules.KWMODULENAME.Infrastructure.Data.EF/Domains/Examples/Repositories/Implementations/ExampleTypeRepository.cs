using App.Modules.KWMODULENAME.Domain.Domains.Examples.Repositories;
using App.Modules.KWMODULENAME.Infrastructure.Data.EF;
using App.Modules.KWMODULENAME.Shared.Domains.Examples.Models.Implmentations;
using App.Modules.Sys.Infrastructure.Domains.Persistence.Relational.EF.Repositories.Implementations.Base;
using App.Modules.Sys.Shared.Domains.Diagnostics;

namespace App.Modules.KWMODULENAME.Infrastructure.Domains.Examples.Repositories.Implementations
{
	/// <summary>
	/// EF-backed repository for <see cref="ExampleType"/> reference data.
	/// </summary>
	public class ExampleTypeRepository : CrustStateRepositoryBase<ExampleType>, IExampleTypeRepository
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ExampleTypeRepository"/> class.
		/// </summary>
		/// <param name="logger">Logger instance for diagnostics.</param>
		/// <param name="db">The module database context.</param>
		public ExampleTypeRepository(IAppLogger logger, ModuleDbContext db)
			: base(logger, db)
		{
		}

		/// <inheritdoc />
		/// <remarks>
		/// Reference data toggles the inherited Enabled property based on the requested state.
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

			ExampleType? entity = await this.GetForUpdateAsync(id, cancellationToken);
			if (entity is null)
			{
				throw new InvalidOperationException(
					"ExampleType with ID " + id + " was not found.");
			}

			entity.Enabled = string.Equals(stateKey, "enabled", StringComparison.OrdinalIgnoreCase);

			await this.DbContext.SaveChangesAsync(cancellationToken);
		}
	}
}
