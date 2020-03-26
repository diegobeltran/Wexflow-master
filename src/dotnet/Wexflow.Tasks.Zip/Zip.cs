﻿using System;
using Wexflow.Core;
using System.Xml.Linq;
using ICSharpCode.SharpZipLib.Zip;
using System.IO;
using System.Threading;

namespace Wexflow.Tasks.Zip
{
    public class Zip : Task
    {
        public string ZipFileName { get; private set; }

        public Zip(XElement xe, Workflow wf)
            : base(xe, wf)
        {
            ZipFileName = GetSetting("zipFileName");
        }

        public override TaskStatus Run()
        {
            Info("Zipping files...");

            bool success = true;

            var files = SelectFiles();
            if (files.Length > 0)
            {
                var zipPath = Path.Combine(Workflow.WorkflowTempFolder, ZipFileName);

                try
                {
                    using (var s = new ZipOutputStream(File.Create(zipPath)))
                    {
                        s.SetLevel(9); // 0 - store only to 9 - means best compression

                        var buffer = new byte[4096];

                        foreach (FileInf file in files)
                        {
                            // Using GetFileName makes the result compatible with XP
                            // as the resulting path is not absolute.
                            var entry = new ZipEntry(Path.GetFileName(file.Path)) { DateTime = DateTime.Now };

                            // Setup the entry data as required.

                            // Crc and size are handled by the library for seakable streams
                            // so no need to do them here.

                            // Could also use the last write time or similar for the file.
                            s.PutNextEntry(entry);

                            using (FileStream fs = File.OpenRead(file.Path))
                            {
                                // Using a fixed size buffer here makes no noticeable difference for output
                                // but keeps a lid on memory usage.
                                int sourceBytes;
                                do
                                {
                                    sourceBytes = fs.Read(buffer, 0, buffer.Length);
                                    s.Write(buffer, 0, sourceBytes);
                                } while (sourceBytes > 0);
                            }
                        }

                        // Finish/Close arent needed strictly as the using statement does this automatically

                        // Finish is important to ensure trailing information for a Zip file is appended.  Without this
                        // the created file would be invalid.
                        s.Finish();

                        // Close is important to wrap things up and unlock the file.
                        s.Close();

                        InfoFormat("Zip {0} created.", zipPath);
                        Files.Add(new FileInf(zipPath, Id));
                    }
                }
                catch (ThreadAbortException)
                {
                    throw;
                }
                catch (Exception e)
                {
                    ErrorFormat("An error occured while creating the Zip {0}", e, zipPath);
                    success = false;
                }
            }

            var status = Status.Success;

            if (!success)
            {
                status = Status.Error;
            }

            Info("Task finished.");
            return new TaskStatus(status);
        }
    }
}
