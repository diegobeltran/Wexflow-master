﻿using System.Collections.Generic;
using Wexflow.Core;
using Renci.SshNet;
using System.IO;
using Renci.SshNet.Sftp;

namespace Wexflow.Tasks.Ftp
{
    public class PluginSftp:PluginBase
    {
        public string PrivateKeyPath { get; set; }
        public string Passphrase { get; set; }

        public PluginSftp(Task task, string server, int port, string user, string password, string path, string privateKeyPath, string passphrase)
            :base(task, server, port, user, password, path)
        {
            PrivateKeyPath = privateKeyPath;
            Passphrase = passphrase;
        }

        private ConnectionInfo GetConnectionInfo()
        {
            // Setup Credentials and Server Information
            ConnectionInfo connInfo;

            if (!string.IsNullOrEmpty(PrivateKeyPath) && !string.IsNullOrEmpty(Passphrase))
            {
                connInfo = new ConnectionInfo(Server, Port, User, new PasswordAuthenticationMethod(User, Password), new PrivateKeyAuthenticationMethod(User, new PrivateKeyFile(PrivateKeyPath, Passphrase)));
            }
            else
            {
                connInfo = new ConnectionInfo(Server, Port, User, new PasswordAuthenticationMethod(User, Password));
            }

            return connInfo;
        }

        public override FileInf[] List()
        {
            var files = new List<FileInf>();

            using (var client = new SftpClient(GetConnectionInfo()))
            {
                client.Connect();
                client.ChangeDirectory(Path);

                var sftpFiles = client.ListDirectory(".");
                foreach (SftpFile file in sftpFiles)
                {
                    if (file.IsRegularFile)
                    {
                        files.Add(new FileInf(file.FullName, Task.Id));
                        Task.InfoFormat("[PluginSFTP] file {0} found on {1}.", file.FullName, Server);
                    }
                }

                client.Disconnect();
            }

            return files.ToArray();
        }

        public override void Upload(FileInf file)
        {
            using (var client = new SftpClient(GetConnectionInfo()))
            {
                client.Connect();
                client.ChangeDirectory(Path);

                using (FileStream fileStream = File.OpenRead(file.Path))
                {
                    client.UploadFile(fileStream, file.RenameToOrName, true);
                }
                Task.InfoFormat("[PluginSFTP] file {0} sent to {1}.", file.Path, Server);

                client.Disconnect();
            }
        }

        public override void Download(FileInf file)
        {
            using (var client = new SftpClient(GetConnectionInfo()))
            {
                client.Connect();
                client.ChangeDirectory(Path);

                var destFileName = System.IO.Path.Combine(Task.Workflow.WorkflowTempFolder, file.FileName);
                using (FileStream ostream = File.Create(destFileName))
                {
                    client.DownloadFile(file.Path, ostream);
                    Task.Files.Add(new FileInf(destFileName, Task.Id));
                }
                Task.InfoFormat("[PluginSFTP] file {0} downloaded from {1}.", file.Path, Server);

                client.Disconnect();
            }
        }

        public override void Delete(FileInf file)
        {
            using (var client = new SftpClient(GetConnectionInfo()))
            {
                client.Connect();
                client.ChangeDirectory(Path);

                client.DeleteFile(file.Path);
                Task.InfoFormat("[PluginSFTP] file {0} deleted from {1}.", file.Path, Server);

                client.Disconnect();
            }
        }
    }
}
