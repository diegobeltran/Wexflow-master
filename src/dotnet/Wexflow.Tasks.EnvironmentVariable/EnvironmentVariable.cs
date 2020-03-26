﻿using System;
using System.Threading;
using System.Xml.Linq;
using Wexflow.Core;

namespace Wexflow.Tasks.EnvironmentVariable
{
    public class EnvironmentVariable : Task
    {
        public string VariableName { get; private set; }

        public EnvironmentVariable(XElement xe, Workflow wf) : base(xe, wf)
        {
            VariableName = GetSetting("name");
        }

        public override TaskStatus Run()
        {
            Info("Getting environment variable...");

            string value = string.Empty;
            bool succeeded = false;
            try
            {
                value = Environment.GetEnvironmentVariable(VariableName);
                InfoFormat("The value of the environment variable '{0}' is: {1}", VariableName, value);
                succeeded = true;
            }
            catch (ThreadAbortException)
            {
                throw;
            }
            catch (Exception e)
            {
                ErrorFormat("An error occured while retrieving environment variable. Error: {0}", e.Message);
            }

            Info("Task finished.");
            return new TaskStatus(succeeded ? Status.Success : Status.Error, value);
        }
    }
}
