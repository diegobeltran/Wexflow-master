﻿using System;
using Wexflow.Core;
using System.Xml.Linq;
using System.IO;
using System.Threading;

namespace Wexflow.Tasks.FilesMover
{
    public class FilesMover:Task
    {
        public string DestFolder { get; private set; }
        public bool Overwrite { get; private set; }
        public string PreserveFolderStructFrom { get; private set; }
        public bool AllowCreateDirectory { get; private set; }

        public FilesMover(XElement xe, Workflow wf)
            : base(xe, wf)
        {
            DestFolder = GetSetting("destFolder");
            Overwrite = bool.Parse(GetSetting("overwrite", "false"));
            PreserveFolderStructFrom = GetSetting("preserveFolderStructFrom");
            AllowCreateDirectory = GetSettingBool("allowCreateDirectory", true);
        }

        public override TaskStatus Run()
        {
            Info("Moving files...");

            var success = true;
            var atLeastOneSucceed = false;

            var files = SelectFiles();
            for (var i = files.Length - 1; i > -1; i--) 
            {
                var file = files[i];
                var fileName = Path.GetFileName(file.Path);
                string destFilePath;
                if (!string.IsNullOrEmpty(fileName))
                {
                    if (!string.IsNullOrWhiteSpace(PreserveFolderStructFrom) &&
                        file.Path.StartsWith(PreserveFolderStructFrom, StringComparison.InvariantCultureIgnoreCase))
                    {
                        var preservedFolderStruct = Path.GetDirectoryName(file.Path);
                        preservedFolderStruct = preservedFolderStruct.Length > PreserveFolderStructFrom.Length
                            ? preservedFolderStruct.Remove(0, PreserveFolderStructFrom.Length)
                            : string.Empty;

                        if (preservedFolderStruct.StartsWith(Path.DirectorySeparatorChar.ToString()))
                        {
                            preservedFolderStruct = preservedFolderStruct.Remove(0, 1);
                        }

                        destFilePath = Path.Combine(DestFolder, preservedFolderStruct, file.FileName);
                    }
                    else
                    {
                        destFilePath = Path.Combine(DestFolder, fileName);
                    }
                }
                else
                {
                    ErrorFormat("File name of {0} is empty.", file);
                    continue;
                }

                try
                {
                    if (AllowCreateDirectory && !Directory.Exists(Path.GetDirectoryName(destFilePath)))
                    {
                        InfoFormat("Creating directory: {0}", Path.GetDirectoryName(destFilePath));
                        Directory.CreateDirectory(Path.GetDirectoryName(destFilePath));
                    }

                    if (File.Exists(destFilePath))
                    {
                        if (Overwrite)
                        {
                            File.Delete(destFilePath);
                        }
                        else
                        {
                            ErrorFormat("Destination file {0} already exists.", destFilePath);
                            success = false;
                            continue;
                        }
                    }

                    File.Move(file.Path, destFilePath);
                    var fi = new FileInf(destFilePath, Id);
                    Files.Add(fi);
                    Workflow.FilesPerTask[file.TaskId].Remove(file);
                    InfoFormat("File moved: {0} -> {1}", file.Path, destFilePath);
                    if (!atLeastOneSucceed) atLeastOneSucceed = true;
                }
                catch (ThreadAbortException)
                {
                    throw;
                }
                catch (Exception e)
                { 
                    ErrorFormat("An error occured while moving the file {0} to {1}", e, file.Path, destFilePath);
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
