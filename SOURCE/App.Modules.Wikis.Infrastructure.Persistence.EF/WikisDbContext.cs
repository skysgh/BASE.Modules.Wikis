using App.Modules.Wikis.Domain.Domains.Wikis.Entities.Implementations;
using App.Modules.Sys.Infrastructure.Domains.Persistence.Relational.EF.DbContexts.Implementations.Base;
using Microsoft.EntityFrameworkCore;

namespace App.Modules.Wikis.Infrastructure.Persistence.EF
{
	/// <summary>
	/// Database context for the Wikis module.
	/// Each module has its own DbContext to enforce bounded context separation.
	/// Schema configurations are discovered via <c>IEntityTypeConfiguration&lt;T&gt;</c>.
	/// </summary>
	public class ModuleDbContext : ModuleDbContextBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ModuleDbContext"/> class.
		/// </summary>
		/// <param name="options">The database context options.</param>
		public ModuleDbContext(DbContextOptions<ModuleDbContext> options)
			: base(options)
		{
		}

		/// <summary>Gets or sets the Wiki roots.</summary>
		public DbSet<Wiki> Wikis { get; set; } = null!;

		/// <summary>Gets or sets the WikiPage entities (the page tree).</summary>
		public DbSet<WikiPage> WikiPages { get; set; } = null!;

		/// <summary>Gets or sets the immutable WikiPageVersion snapshots.</summary>
		public DbSet<WikiPageVersion> WikiPageVersions { get; set; } = null!;

		/// <summary>Gets or sets the WikiPageVersionBody rows (Database body sink, ADR-018N).</summary>
		public DbSet<WikiPageVersionBody> WikiPageVersionBodies { get; set; } = null!;

		/// <summary>Gets or sets the WikiMedia immutable media handles.</summary>
		public DbSet<WikiMedia> WikiMedia { get; set; } = null!;

		/// <summary>Gets or sets the WikiAcl share-based access-control entries.</summary>
		public DbSet<WikiAcl> WikiAcls { get; set; } = null!;

		/// <summary>Gets or sets the WikiTemplate reusable page scaffolds (ADR-018C).</summary>
		public DbSet<WikiTemplate> WikiTemplates { get; set; } = null!;

		/// <summary>Gets or sets the WikiTemplateSection ordered scaffold/lint sections.</summary>
		public DbSet<WikiTemplateSection> WikiTemplateSections { get; set; } = null!;

		/// <summary>Gets or sets the WikiTemplateBinding namespace/subtree template bindings.</summary>
		public DbSet<WikiTemplateBinding> WikiTemplateBindings { get; set; } = null!;

		/// <summary>Gets or sets the WikiNodeStyle additive page-style rows for section backgrounds.</summary>
		public DbSet<WikiNodeStyle> WikiNodeStyles { get; set; } = null!;

		/// <inheritdoc/>
		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			ArgumentNullException.ThrowIfNull(modelBuilder);
			this.SchemaKey = App.Modules.Wikis.ModuleConstants.DbSchemaKey;

			base.OnModelCreating(modelBuilder);

			// EF configurations are discovered via IEntityTypeConfiguration<T>
			modelBuilder.ApplyConfigurationsFromAssembly(typeof(ModuleDbContext).Assembly);
		}
	}
}
