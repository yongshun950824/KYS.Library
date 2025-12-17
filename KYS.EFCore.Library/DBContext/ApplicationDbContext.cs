using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Logging;

namespace KYS.EFCore.Library.DBContext
{
    public partial class ApplicationDbContext : DbContext
    {
        private readonly ILogger<ApplicationDbContext> _logger;

        public ApplicationDbContext()
        {
        }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options,
            ILogger<ApplicationDbContext> logger)
            : base(options)
        {
            _logger = logger;
        }

        public virtual DbSet<ActionLog> ActionLog { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // In PostgreSQL

            modelBuilder.Entity<ActionLog>(entity =>
            {
                entity.ToTable("action_log");

                entity.HasIndex(e => e.ReferenceTable)
                    .HasDatabaseName("idx_reference_table");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("uuid_generate_v4()");

                entity.Property(e => e.ActionType).HasColumnName("action_type");

                entity.Property(e => e.ColumnName)
                    .HasColumnType("character varying")
                    .HasColumnName("column_name");

                entity.Property(e => e.ColumnNewValue)
                    .HasColumnType("character varying")
                    .HasColumnName("column_new_value");

                entity.Property(e => e.ColumnOldValue)
                    .HasColumnType("character varying")
                    .HasColumnName("column_old_value");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("timestamp(6) without time zone")
                    .HasColumnName("created_date");

                entity.Property(e => e.CreatedUser).HasColumnName("created_user");

                entity.Property(e => e.RecordValue)
                    .HasColumnType("json")
                    .HasColumnName("record_value");

                entity.Property(e => e.ReferenceId).HasColumnName("reference_id");

                entity.Property(e => e.ReferenceTable).HasColumnName("reference_table");
            });

        }
    }
}
