﻿using System;
using Wexflow.Core;
using System.Xml.Linq;
using System.Threading;
using System.IO;

namespace Wexflow.Tasks.FilesRenamer
{
    public class FilesRenamer:Task
    {
        public bool Overwrite { get; private set; }

        public FilesRenamer(XElement xe, Workflow wf)
            : base(xe, wf)
        {
            Overwrite = bool.Parse(GetSetting("overwrite", "false"));
        }

        public override TaskStatus Run()
        {
            Info("Renaming files...");

            bool success = true;
            bool atLeastOneSucceed = false;

            foreach (var file in SelectFiles())
            {
                try
                {
                    if (!string.IsNullOrEmpty(file.RenameTo))
                    {
                        var dirName = Path.GetDirectoryName(file.Path);
                        if(dirName == null) throw new Exception("File directory is null");
                        var destPath = Path.Combine(dirName, file.RenameTo);

                        if (File.Exists(destPath))
                        {
                            if (Overwrite)
                            {
                                if (file.Path != destPath)
                                {
                                    File.Delete(destPath);
                                }
                                else
                                {
                                    InfoFormat("The file {0} and its new name {1} are the same.", file.Path, file.RenameTo);
                                    continue;
                                }
                            }
                            else
                            {
                                ErrorFormat("The destination file {0} already exists.", destPath);
                                success = false;
                                continue;
                            }
                        }

                        File.Move(file.Path, destPath);
                        InfoFormat("File {0} renamed to {1}", file.Path, file.RenameTo);
                        file.Path = destPath;
                        file.RenameTo = string.Empty;
                        if (!atLeastOneSucceed) atLeastOneSucceed = true;
                        Files.Add(new FileInf(destPath, file.TaskId));
                    }
                }
                catch (ThreadAbortException)
                {
                    throw;
                }
                catch (Exception e)
                {
                    ErrorFormat("An error occured while renaming the file {0} to {1}. Error: {2}", file.Path, file.RenameTo, e.Message);
                    success = false;
                }
            }

            var status = Status.Success;

            if (!success && atLeastOneSucceed)
            {
                status = Status.Warning;
            }
            else if (!success)
            {
                status = Status.Error;
            }

            Info("Task finished.");
            return new TaskStatus(status, false);
        }
    }
}