using System;
using System.ComponentModel.DataAnnotations;

namespace KYS.EFCore.Library.DBContext
{
    /// <summary>
    /// Represents a single audit record written by <see cref="ApplicationDbContext.SaveChangesWithAuditAsync"/>.
    /// <br /><br />
    /// Mapped to the <c>ActionLog</c> table, or <c>action_log</c> (snake_case columns) when the provider is PostgreSQL (Npgsql).
    /// </summary>
    public partial class ActionLog
    {
        /// <summary>
        /// Gets or sets the primary key of the audit record.
        /// </summary>
        [Key]
        public long Id { get; set; }
        /// <summary>
        /// Gets or sets the primary key value of the audited entity, stored as a string.
        /// <br /><br />
        /// For entities with a composite key, only the first key value is recorded.
        /// </summary>
        public string ReferenceId { get; set; }
        /// <summary>
        /// Gets or sets the name of the audited entity.
        /// <br /><br />
        /// This is the CLR type name of the entity (not the mapped database table name),
        /// optionally converted with the <see cref="KYS.Library.Helpers.FormattingHelper.Formatting"/> passed to
        /// <see cref="ApplicationDbContext.SaveChangesWithAuditAsync"/>.
        /// </summary>
        public string ReferenceTable { get; set; }
        /// <summary>
        /// Gets or sets the kind of change that was recorded. Persisted as a string (maximum 10 characters).
        /// </summary>
        public AuditAction ActionType { get; set; }
        /// <summary>
        /// Gets or sets the name of the property that was changed.
        /// <br /><br />
        /// Only populated when <see cref="ActionType"/> is <see cref="AuditAction.Update"/>.
        /// </summary>
        public string ColumnName { get; set; }
        /// <summary>
        /// Gets or sets the value of <see cref="ColumnName"/> before the update, converted to a string.
        /// <br /><br />
        /// Only populated when <see cref="ActionType"/> is <see cref="AuditAction.Update"/>.
        /// </summary>
        public string ColumnOldValue { get; set; }
        /// <summary>
        /// Gets or sets the value of <see cref="ColumnName"/> after the update, converted to a string.
        /// <br /><br />
        /// Only populated when <see cref="ActionType"/> is <see cref="AuditAction.Update"/>.
        /// </summary>
        public string ColumnNewValue { get; set; }
        /// <summary>
        /// Gets or sets a JSON snapshot of the audited entity.
        /// <br /><br />
        /// For <see cref="AuditAction.Insert"/> it contains the key and the inserted values;
        /// for <see cref="AuditAction.Delete"/> it contains the key and the values before deletion;
        /// for <see cref="AuditAction.Update"/> it is <see langword="null"/>.
        /// Stored as a <c>json</c> column on PostgreSQL.
        /// </summary>
        public string RecordValue { get; set; }
        /// <summary>
        /// Gets or sets the date and time (UTC) at which the audit record was created.
        /// </summary>
        public DateTime CreatedDate { get; set; }
        /// <summary>
        /// Gets or sets the identifier of the user who made the change,
        /// or <see langword="null"/> if no user was supplied.
        /// </summary>
        public Guid? CreatedBy { get; set; }
    }


    /// <summary>
    /// Specifies the kind of change recorded in an <see cref="ActionLog"/>.
    /// </summary>
    public enum AuditAction
    {
        /// <summary>The entity was added.</summary>
        Insert,
        /// <summary>The entity was modified.</summary>
        Update,
        /// <summary>The entity was deleted.</summary>
        Delete
    }
}
