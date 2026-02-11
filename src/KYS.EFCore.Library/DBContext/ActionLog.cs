using System;

namespace KYS.EFCore.Library.DBContext
{
    public partial class ActionLog
    {
        public Guid Id { get; set; }
        public string ReferenceId { get; set; }
        public string ReferenceTable { get; set; }
        public string ActionType { get; set; }
        public string ColumnName { get; set; }
        public string ColumnOldValue { get; set; }
        public string ColumnNewValue { get; set; }
        public string RecordValue { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? CreatedUser { get; set; }
    }
}
