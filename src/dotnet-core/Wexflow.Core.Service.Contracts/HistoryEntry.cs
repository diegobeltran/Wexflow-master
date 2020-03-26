﻿using System.Runtime.Serialization;

namespace Wexflow.Core.Service.Contracts
{
    [DataContract]
    public class HistoryEntry
    {
        [DataMember]
        public string Id { get; set; }
        [DataMember]
        public int WorkflowId { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public LaunchType LaunchType { get; set; }
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public Status Status { get; set; }
        [DataMember]
        public string StatusDate { get; set; }
    }
}
