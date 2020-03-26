﻿using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Xml.Linq;
using Wexflow.Core;

namespace Wexflow.Tasks.ExecCs
{
    public class ExecCs : Task
    {
        public ExecCs(XElement xe, Workflow wf): base(xe, wf)
        {
        }

        public override TaskStatus Run()
        {
            Info("Executing cs scripts...");

            var status = Status.Success;
            var success = true;
            var atLeastOneSuccess = false;
            var csFiles = SelectFiles();

            foreach (var csFile in csFiles)
            {
                try
                {
                    var exePath = Path.Combine(Workflow.WorkflowTempFolder, Path.GetFileNameWithoutExtension(csFile.FileName) + ".exe");
                    exec(csFile.Path, exePath);
                    Files.Add(new FileInf(exePath, Id));
                    InfoFormat("The script {0} has been executed -> {1}", csFile.Path, exePath);
                    if (!atLeastOneSuccess) atLeastOneSuccess = true;
                }
                catch (ThreadAbortException)
                {
                    throw;
                }
                catch (Exception e)
                {
                    ErrorFormat("An error occured while executing the script {0}: {1}", csFile.Path, e.Message);
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

        private void exec(string csPath, string exePath)
        {
            var exeGenerated = CompileExecutable(csPath, exePath);
            StartProcess(exePath, string.Empty, false);
        }

        private bool CompileExecutable(string sourceName, string exeName)
        {
            FileInfo sourceFile = new FileInfo(sourceName);
            CodeDomProvider provider = CodeDomProvider.CreateProvider("CSharp");
            bool compileOk = false;

            CompilerParameters cp = new CompilerParameters();

            // Generate an executable instead of 
            // a class library.
            cp.GenerateExecutable = true;

            // Specify the assembly file name to generate.
            cp.OutputAssembly = exeName;

            // Save the assembly as a physical file.
            cp.GenerateInMemory = false;

            // Set whether to treat all warnings as errors.
            cp.TreatWarningsAsErrors = false;

            // Invoke compilation of the source file.
            CompilerResults cr = provider.CompileAssemblyFromFile(cp, sourceName);

            if (cr.Errors.Count > 0)
            {
                // Display compilation errors.
                ErrorFormat("Errors building {0} into {1}", sourceName, cr.PathToAssembly);
                foreach (CompilerError ce in cr.Errors)
                {
                    ErrorFormat("  {0}", ce.ToString());
                }
            }
            else
            {
                // Display a successful compilation message.
                InfoFormat("Source {0} built into {1} successfully.", sourceName, cr.PathToAssembly);
            }

            // Return the results of the compilation.
            if (cr.Errors.Count > 0)
            {
                compileOk = false;
            }
            else
            {
                compileOk = true;
            }
            
            return compileOk;
        }

        private void StartProcess(string processPath, string processCmd, bool hideGui)
        {
            var startInfo = new ProcessStartInfo(processPath, processCmd)
            {
                CreateNoWindow = hideGui,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            var process = new Process { StartInfo = startInfo };
            process.OutputDataReceived += OutputHandler;
            process.ErrorDataReceived += ErrorHandler;
            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            process.WaitForExit();
        }

        private void OutputHandler(object sendingProcess, DataReceivedEventArgs outLine)
        {
            InfoFormat("{0}", outLine.Data);
        }

        private void ErrorHandler(object sendingProcess, DataReceivedEventArgs outLine)
        {
            ErrorFormat("{0}", outLine.Data);
        }

    }
}
