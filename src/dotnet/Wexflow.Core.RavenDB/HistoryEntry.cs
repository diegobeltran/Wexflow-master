﻿namespace Wexflow.Core.RavenDB
{
    public class HistoryEntry : Core.Db.HistoryEntry
    {
        public string Id { get; set; }

        public override string GetDbId()
        {
            return Id;
        }
    }
}
