﻿using System;
using System.IO;
using System.Threading;
using System.Xml.Linq;
using Wexflow.Core;

namespace Wexflow.Tasks.FilesEqual
{
    public class FilesEqual : Task
    {
        public string File1 { get; set; }
        public string File2 { get; set; }

        public FilesEqual(XElement xe, Workflow wf)
            : base(xe, wf)
        {
            File1 = GetSetting("file1");
            File2 = GetSetting("file2");
        }

        public override TaskStatus Run()
        {
            Info("Checking...");

            bool success = true;

            try
            {
                if (!File.Exists(File1))
                {
                    Logger.ErrorFormat("The file {0} does not exist.", File1);
                    return new TaskStatus(Status.Error, false);
                }

                if (!File.Exists(File2))
                {
                    Logger.ErrorFormat("The file {0} does not exist.", File2);
                    return new TaskStatus(Status.Error, false);
                }

                var xmlPath = Path.Combine(Workflow.WorkflowTempFolder,
                       string.Format("FilesEqual_{0:yyyy-MM-dd-HH-mm-ss-fff}.xml", DateTime.Now));
                var xdoc = new XDocument(new XElement("Root"));
                var xFiles = new XElement("Files");

                xFiles.Add(new XElement("File",
                    new XAttribute("path", File1),
                    new XAttribute("name", Path.GetFileName(File1))));

                xFiles.Add(new XElement("File",
                    new XAttribute("path", File2),
                    new XAttribute("name", Path.GetFileName(File2))));
                
                if (xdoc.Root != null)
                {
                    xdoc.Root.Add(xFiles);

                    var res = FileEquals(File1, File2);
                    xdoc.Root.Add(new XElement("Result", res.ToString().ToLower()));
                }

                xdoc.Save(xmlPath);
                Files.Add(new FileInf(xmlPath, Id));
                InfoFormat("The result has been written in: {0}", xmlPath);
            }
            catch (ThreadAbortException)
            {
                throw;
            }
            catch (Exception e)
            {
                ErrorFormat("An error occured while checking files. Error: {0}", e.Message);
                success = false;
            }

            var status = Status.Success;

            if (!success)
            {
                status = Status.Error;
            }

            Info("Task finished.");
            return new TaskStatus(status, false);
        }

        private bool FileEquals(string path1, string path2)
        {
            byte[] file1 = File.ReadAllBytes(path1);
            byte[] file2 = File.ReadAllBytes(path2);
            if (file1.Length == file2.Length)
            {
                for (int i = 0; i < file1.Length; i++)
                {
                    if (file1[i] != file2[i])
                    {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }
    }
}
