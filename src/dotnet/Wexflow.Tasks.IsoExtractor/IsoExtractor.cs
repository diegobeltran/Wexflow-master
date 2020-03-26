﻿using DiscUtils.Iso9660;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml.Linq;
using Wexflow.Core;

namespace Wexflow.Tasks.IsoExtractor
{
    public class IsoExtractor : Task
    {
        public string DestDir { get; set; }

        public IsoExtractor(XElement xe, Workflow wf)
            : base(xe, wf)
        {
            DestDir = GetSetting("destDir");
        }

        public override TaskStatus Run()
        {
            Info("Extracting ISO files...");

            bool success = true;
            bool atLeastOneSucceed = false;

            var isos = SelectFiles();

            if (isos.Length > 0)
            {
                foreach (FileInf iso in isos)
                {
                    try
                    {
                        string destFolder = Path.Combine(DestDir
                            , Path.GetFileNameWithoutExtension(iso.Path) + "_" + string.Format("{0:yyyy-MM-dd-HH-mm-ss-fff}", DateTime.Now));
                        Directory.CreateDirectory(destFolder);
                        
                        ExtractIso(iso.Path, destFolder);

                        foreach (var file in Directory.GetFiles(destFolder, "*.*", SearchOption.AllDirectories))
                        {
                            Files.Add(new FileInf(file, Id));
                        }

                        InfoFormat("ISO {0} extracted to {1}", iso.Path, destFolder);

                        if (!atLeastOneSucceed) atLeastOneSucceed = true;
                    }
                    catch (ThreadAbortException)
                    {
                        throw;
                    }
                    catch (Exception e)
                    {
                        ErrorFormat("An error occured while extracting of the ISO {0}", e, iso.Path);
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

        private void ExtractIso(string isoPath, string destDir)
        {
            using (FileStream isoStream = File.Open(isoPath, FileMode.Open))
            {
                CDReader cd = new CDReader(isoStream, true);
                var files = cd.GetFiles("\\", "*.*", SearchOption.AllDirectories);

                foreach (var file in files)
                {
                    using (var stream = cd.OpenFile(file, FileMode.Open))
                    {
                        var destFile = destDir.TrimEnd('\\') + "\\" + Regex.Replace(file.Replace("/", "\\"), @";\d*$", "").TrimStart('\\');

                        // Create directories
                        var destFolder = Path.GetDirectoryName(destFile);
                        destFolder = destFolder.Equals(destDir) ? string.Empty : destFolder;

                        var finalDestFolder = destDir;
                        if (!string.IsNullOrEmpty(destFolder))
                        {
                            finalDestFolder = Path.Combine(destDir, destFolder);
                        }

                        if (!Directory.Exists(finalDestFolder))
                        {
                            Directory.CreateDirectory(finalDestFolder);
                        }

                        // Create the file
                        using (FileStream sw = new FileStream(destFile, FileMode.CreateNew))
                        {
                            byte[] buffer = new byte[2048];
                            int bytesRead;
                            while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
                            {
                                sw.Write(buffer, 0, bytesRead);
                            }
                        }
                    }

                }

            }
        }

    }
}
