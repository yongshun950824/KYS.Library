using Microsoft.EntityFrameworkCore;

namespace KYS.EFCore.Library.DBContext
{
    /// <summary>
    /// Base <see cref="DbContext"/> that provides an <see cref="DBContext.ActionLog"/> table and
    /// audit-aware saving through <see cref="SaveChangesWithAuditAsync"/>.
    /// <br /><br />
    /// Derive from this class to add your own <see cref="DbSet{TEntity}"/> properties.
    /// </summary>
    /// <remarks>
    /// The <see cref="DBContext.ActionLog"/> mapping is provider-aware: when the database provider name contains
    /// <c>Npgsql</c>, snake_case table/column/index names and PostgreSQL column types are used;
    /// otherwise PascalCase names and provider-default column types are used (e.g. for SQLite in tests).
    /// </remarks>
    public partial class ApplicationDbContext : DbContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationDbContext"/> class.
        /// </summary>
        /// <param name="options">The options to be used by the <see cref="DbContext"/>.</param>
        public ApplicationDbContext(DbContextOptions options)
            : base(options)
        {

        }

        /// <summary>
        /// Gets or sets the audit records written by <see cref="SaveChangesWithAuditAsync"/>.
        /// </summary>
        public virtual DbSet<ActionLog> ActionLog { get; set; }

        /// <summary>
        /// Configures the <see cref="DBContext.ActionLog"/> entity mapping (table, index, column names and types)
        /// based on the current database provider.
        /// </summary>
        /// <param name="modelBuilder">The builder used to construct the model for this context.</param>
        /// <remarks>
        /// Derived contexts that override this method should call <c>base.OnModelCreating(modelBuilder)</c>
        /// so that the <see cref="DBContext.ActionLog"/> mapping is kept.
        /// </remarks>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Provider-aware mapping: keep Postgres-specific mappings when running
            // against Npgsql; for other providers (e.g. SQLite InMemory) avoid
            // provider-specific SQL/types so tests can run.

            var isPostgres = Database?.ProviderName != null
                && Database.ProviderName.Contains("Npgsql");

            modelBuilder.Entity<ActionLog>(entity =>
            {
                #region Table, Index, Column Names by SQL Provider
                string tableName = "ActionLog";
                string referenceTableIndexName = "IDX_ActionLog_ReferenceTable";
                string idColumnName = "Id";
                string actionTypeColumnName = "ActionType";
                string columnName = "ColumnName";
                string columnNewValueColumnName = "ColumnNewValue";
                string columnOldValueColumnName = "ColumnOldValue";
                string createdDateColumnName = "CreatedDate";
                string createdUserColumnName = "CreatedBy";
                string recordValueColumnName = "RecordValue";
                string referenceIdColumnName = "ReferenceId";
                string referenceTableColumnName = "ReferenceTable";

                if (isPostgres)
                {
                    tableName = "action_log";
                    referenceTableIndexName = "idx_action_log_reference_table";
                    idColumnName = "id";
                    actionTypeColumnName = "action_type";
                    columnName = "column_name";
                    columnNewValueColumnName = "column_new_value";
                    columnOldValueColumnName = "column_old_value";
                    createdDateColumnName = "created_date";
                    createdUserColumnName = "created_by";
                    recordValueColumnName = "record_value";
                    referenceIdColumnName = "reference_id";
                    referenceTableColumnName = "reference_table";
                }
                #endregion

                entity.ToTable(tableName);

                entity.HasIndex(e => e.ReferenceTable)
                    .HasDatabaseName(referenceTableIndexName);

                entity.Property(e => e.Id)
                    .HasColumnName(idColumnName);

                entity.Property(e => e.ActionType)
                    .HasConversion<string>()
                    .HasColumnName(actionTypeColumnName)
                    .HasMaxLength(10);

                var colNameProp = entity.Property(e => e.ColumnName)
                    .HasColumnName(columnName);
                if (isPostgres)
                {
                    colNameProp.HasColumnType("character varying");
                }

                var colNewValProp = entity.Property(e => e.ColumnNewValue)
                    .HasColumnName(columnNewValueColumnName);
                if (isPostgres)
                {
                    colNewValProp.HasColumnType("character varying");
                }

                var colOldValProp = entity.Property(e => e.ColumnOldValue)
                    .HasColumnName(columnOldValueColumnName);
                if (isPostgres)
                {
                    colOldValProp.HasColumnType("character varying");
                }

                var createdDateProp = entity.Property(e => e.CreatedDate)
                    .HasColumnName(createdDateColumnName);
                if (isPostgres)
                {
                    createdDateProp.HasColumnType("timestamp(6) without time zone");
                }

                entity.Property(e => e.CreatedBy).HasColumnName(createdUserColumnName);

                // EF Core 3.1 JSON column
                var recordValueProp = entity.Property(e => e.RecordValue)
                    .HasColumnName(recordValueColumnName);
                if (isPostgres)
                {
                    recordValueProp.HasColumnType("json");
                }

                entity.Property(e => e.ReferenceId)
                    .HasColumnName(referenceIdColumnName);

                entity.Property(e => e.ReferenceTable)
                    .HasColumnName(referenceTableColumnName);
            });
        }
    }
}
