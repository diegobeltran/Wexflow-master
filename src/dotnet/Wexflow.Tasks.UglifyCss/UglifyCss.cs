﻿using NUglify;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Xml.Linq;
using Wexflow.Core;

namespace Wexflow.Tasks.UglifyCss
{
    public class UglifyCss:Task
    {
        public UglifyCss(XElement xe, Workflow wf) : base(xe, wf)
        {
        }

        public override TaskStatus Run()
        {
            Info("Uglifying CSS files...");

            var status = Status.Success;
            var success = true;
            var atLeastOneSuccess = false;
            var cssFiles = SelectFiles();

            foreach (var cssFile in cssFiles)
            {
                try
                {
                    var source = File.ReadAllText(cssFile.Path);
                    var result = Uglify.Css(source);
                    if (result.HasErrors)
                    {
                        ErrorFormat("An error occured while uglifying the CSS file {0}: {1}", cssFile.Path, string.Concat(result.Errors.Select(e => e.Message + "\n").ToArray()));
                        success = false;
                        continue;
                    }

                    var destPath = Path.Combine(Workflow.WorkflowTempFolder, Path.GetFileNameWithoutExtension(cssFile.FileName) + ".min.css");
                    File.WriteAllText(destPath, result.Code);
                    Files.Add(new FileInf(destPath, Id));
                    InfoFormat("The CSS file {0} has been uglified -> {1}", cssFile.Path, destPath);
                    if (!atLeastOneSuccess) atLeastOneSuccess = true;
                }
                catch (ThreadAbortException)
                {
                    throw;
                }
                catch (Exception e)
                {
                    ErrorFormat("An error occured while uglifying the CSS file {0}: {1}", cssFile.Path, e.Message);
                    success = false;
                }
            }

            if (!success && atLeastOneSuccess)
            {
                status = Status.Warning;
            }
            else if (!success)
            {
                status = Status.Error;
            }

            Info("Task finished.");
            return new TaskStatus(status);
        }
    }
}
