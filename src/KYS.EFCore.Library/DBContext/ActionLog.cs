using System;
using System.ComponentModel.DataAnnotations;

namespace KYS.EFCore.Library.DBContext
{
    public partial class ActionLog
    {
        [Key]
        public long Id { get; set; }
        public string ReferenceId { get; set; }
        public string ReferenceTable { get; set; }
        public AuditAction ActionType { get; set; }
        public string ColumnName { get; set; }
        public string ColumnOldValue { get; set; }
        public string ColumnNewValue { get; set; }
        public string RecordValue { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? CreatedBy { get; set; }
    }


    public enum AuditAction
    {
        Insert,
        Update,
        Delete
    }
}
