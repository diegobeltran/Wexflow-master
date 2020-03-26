﻿using System;

namespace Wexflow.Server.Contracts
{
    public enum LaunchType
    {
        Startup,
        Trigger,
        Periodic,
        Cron
    }

    public class WorkflowInfo:IComparable
    {
        public string DbId { get; set; }

        public int Id { get;  set; }

        public Guid InstanceId { get; set; }

        public string Name { get;  set; }

        public LaunchType LaunchType { get;  set; }

        public bool IsEnabled { get;  set; }

        public bool IsApproval { get; private set; }
        
        public bool IsWaitingForApproval { get; private set; }

        public string Description { get;  set; }

        public bool IsRunning { get; set; }
        
        public bool IsPaused { get; set; }
        
        public string Period { get; set; }
        
        public string CronExpression { get;  set; }
        
        public bool IsExecutionGraphEmpty { get; set; }

        public Variable[] LocalVariables { get; set; }

        public WorkflowInfo(string dbId, int id, Guid instanceId, string name, LaunchType launchType, bool isEnabled, bool isApproval, bool isWaitingForApproval, string desc, bool isRunning, bool isPaused, string period, string cronExpression, bool isExecutionGraphEmpty, Variable[] localVariables)
        {
            DbId = dbId;
            Id = id;
            InstanceId = instanceId;
            Name = name;
            LaunchType = launchType;
            IsEnabled = isEnabled;
            IsApproval = isApproval;
            IsWaitingForApproval = isWaitingForApproval;
            Description = desc;
            IsRunning = isRunning;
            IsPaused = isPaused;
            Period = period;
            CronExpression = cronExpression;
            IsExecutionGraphEmpty = isExecutionGraphEmpty;
            LocalVariables = localVariables;
        }

        public int CompareTo(object obj)
        {
            var wfi = (WorkflowInfo)obj;
            return wfi.Id.CompareTo(Id);
        }
    }
}
