﻿namespace Wexflow.Core.SQLServer
{
    public class StatusCount : Core.Db.StatusCount
    {
        public static readonly string ColumnName_Id = "ID";
        public static readonly string ColumnName_PendingCount = "PENDING_COUNT";
        public static readonly string ColumnName_RunningCount = "RUNNING_COUNT";
        public static readonly string ColumnName_DoneCount = "DONE_COUNT";
        public static readonly string ColumnName_FailedCount = "FAILED_COUNT";
        public static readonly string ColumnName_WarningCount = "WARNING_COUNT";
        public static readonly string ColumnName_DisabledCount = "DISABLED_COUNT";
        public static readonly string ColumnName_StoppedCount = "STOPPED_COUNT";
        public static readonly string ColumnName_DisapprovedCount = "DISAPPROVED_COUNT";

        public static readonly string TableStruct = "(" + ColumnName_Id + " INT IDENTITY(1,1) PRIMARY KEY, " + ColumnName_PendingCount + " INT, " + ColumnName_RunningCount + " INT, " + ColumnName_DoneCount + " INT, " + ColumnName_FailedCount + " INT, " + ColumnName_WarningCount + " INT, " + ColumnName_DisabledCount + " INT, " + ColumnName_StoppedCount + " INT, " + ColumnName_DisapprovedCount + " INT)";

        public int Id { get; set; }
    }
}