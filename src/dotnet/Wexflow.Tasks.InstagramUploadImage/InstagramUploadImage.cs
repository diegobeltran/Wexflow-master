﻿using InstagramApiSharp.API.Builder;
using InstagramApiSharp.Classes;
using InstagramApiSharp.Logger;
using System;
using System.IO;
using System.Threading;
using System.Xml.Linq;
using System.Xml.XPath;
using InstagramApiSharp.API;
using InstagramApiSharp.Classes.Models;
using Wexflow.Core;

namespace Wexflow.Tasks.InstagramUploadImage
{
    public class InstagramUploadImage:Task
    {
        public string Username { get; }
        public string Password { get; }

        public InstagramUploadImage(XElement xe, Workflow wf) : base(xe, wf)
        {
            Username = GetSetting("username");
            Password = GetSetting("password");
        }

        public override TaskStatus Run()
        {
            Info("Uploading images...");

            bool succeeded = true;
            bool atLeastOneSucceed = false;

            try
            {

                var authTask = Authenticate();
                authTask.Wait();

                if (authTask.Result == null)
                {
                    Error("An error occured while authenticating.");
                    return new TaskStatus(Status.Error);
                }
                else
                {
                    Info("Authentication succeeded.");
                }

                var files = SelectFiles();

                foreach (var file in files)
                {
                    try
                    {
                        XDocument xdoc = XDocument.Load(file.Path);

                        foreach (var xvideo in xdoc.XPathSelectElements("/Images/Image"))
                        {
                            string filePath = xvideo.Element("FilePath").Value;
                            string caption = xvideo.Element("Caption").Value;

                            var uploadImageTask = UploadImage(authTask.Result, filePath, caption);
                            uploadImageTask.Wait();
                            succeeded &= uploadImageTask.Result;

                            if (succeeded && !atLeastOneSucceed) atLeastOneSucceed = true;
                        }
                    }
                    catch (ThreadAbortException)
                    {
                        throw;
                    }
                    catch (Exception e)
                    {
                        ErrorFormat("An error occured while uploading the image {0}: {1}", file.Path, e.Message);
                        succeeded = false;
                    }
                }

            }
            catch (ThreadAbortException)
            {
                throw;
            }
            catch (Exception e)
            {
                ErrorFormat("An error occured while uploading images: {0}", e.Message);
                return new TaskStatus(Status.Error);
            }

            var status = Status.Success;

            if (!succeeded && atLeastOneSucceed)
            {
                status = Status.Warning;
            }
            else if (!succeeded)
            {
                status = Status.Error;
            }

            Info("Task finished.");
            return new TaskStatus(status);
        }

        private async System.Threading.Tasks.Task<IInstaApi> Authenticate()
        {
            var userSession = new UserSessionData
            {
                UserName = Username,
                Password = Password
            };

            var instaApi = InstaApiBuilder.CreateBuilder()
                .SetUser(userSession)
                .UseLogger(new DebugLogger(LogLevel.Exceptions))
                .Build();
            const string stateFile = "state.bin";
            try
            {
                // load session file if exists
                if (File.Exists(stateFile))
                {
                    using (var fs = File.OpenRead(stateFile))
                    {
                        instaApi.LoadStateDataFromStream(fs);
                        // in .net core or uwp apps don't use LoadStateDataFromStream
                        // use this one:
                        // _instaApi.LoadStateDataFromString(new StreamReader(fs).ReadToEnd());
                        // you should pass json string as parameter to this function.
                    }
                }
            }
            catch (Exception e)
            {
                ErrorFormat("An error occured while authenticating: {0}", e);
                return null;
            }

            if (!instaApi.IsUserAuthenticated)
            {
                // login
                var logInResult = await instaApi.LoginAsync();
                if (!logInResult.Succeeded)
                {
                    ErrorFormat("Unable to login: {0}", logInResult.Info.Message);
                    return null;
                }
            }
            // save session in file
            var state = instaApi.GetStateDataAsStream();
            // in .net core or uwp apps don't use GetStateDataAsStream.
            // use this one:
            // var state = _instaApi.GetStateDataAsString();
            // this returns you session as json string.
            using (var fileStream = File.Create(stateFile))
            {
                state.Seek(0, SeekOrigin.Begin);
                state.CopyTo(fileStream);
            }

            return instaApi;
        }

        public async System.Threading.Tasks.Task<bool> UploadImage(IInstaApi instaApi, string filePath, string caption)
        {
            try
            {
                var mediaImage = new InstaImageUpload
                {
                    // leave zero, if you don't know how height and width is it.
                    Height = 0,
                    Width = 0,
                    Uri = filePath
                };

                var result = await instaApi.MediaProcessor.UploadPhotoAsync(mediaImage, caption);

                if (!result.Succeeded)
                {
                    InfoFormat("Unable to upload image: {0}", result.Info.Message);
                    return false;
                    
                }

                InfoFormat("Media created: {0}, {1}", result.Value.Pk, result.Value.Caption.Text);
                return true;
            }
            catch (Exception e)
            {
                ErrorFormat("An error occured while uploading the image: {0}", e, filePath);
                return false;
            }
            
        }

    }
}
