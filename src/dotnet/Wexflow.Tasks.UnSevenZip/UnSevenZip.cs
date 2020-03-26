﻿using SharpCompress.Archives;
using SharpCompress.Archives.SevenZip;
using SharpCompress.Common;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Xml.Linq;
using Wexflow.Core;

namespace Wexflow.Tasks.UnSevenZip
{
    public class UnSevenZip : Task
    {
        public string DestDir { get; private set; }

        public UnSevenZip(XElement xe, Workflow wf)
            : base(xe, wf)
        {
            DestDir = GetSetting("destDir");
        }

        public override TaskStatus Run()
        {
            Info("Extracting 7Z archives...");

            bool success = true;
            bool atLeastOneSucceed = false;

            var rars = SelectFiles();

            if (rars.Length > 0)
            {
                foreach (FileInf rar in rars)
                {
                    try
                    {
                        string destFolder = Path.Combine(DestDir
                            , Path.GetFileNameWithoutExtension(rar.Path) + "_" + string.Format("{0:yyyy-MM-dd-HH-mm-ss-fff}", DateTime.Now));
                        Directory.CreateDirectory(destFolder);

                        Extract7Z(rar.Path, destFolder);

                        foreach (var file in Directory.GetFiles(destFolder, "*.*", SearchOption.AllDirectories))
                        {
                            Files.Add(new FileInf(file, Id));
                        }

                        InfoFormat("7Z {0} extracted to {1}", rar.Path, destFolder);

                        if (!atLeastOneSucceed) atLeastOneSucceed = true;
                    }
                    catch (ThreadAbortException)
                    {
                        throw;
                    }
                    catch (Exception e)
                    {
                        ErrorFormat("An error occured while extracting of the 7Z {0}: {1}", rar.Path, e);
                        success = false;
                    }
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

        public void Extract7Z(string rarFileName, string targetDir)
        {
            using (var archive = SevenZipArchive.Open(rarFileName))
            {
                foreach (var entry in archive.Entries.Where(entry => !entry.IsDirectory))
                {
                    entry.WriteToDirectory(targetDir, new ExtractionOptions()
                    {
                        ExtractFullPath = true,
                        Overwrite = true
                    });
                }
            }
        }
    }
}
