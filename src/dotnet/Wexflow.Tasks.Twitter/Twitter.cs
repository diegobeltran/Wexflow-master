﻿using System;
using Wexflow.Core;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Threading;
using Tweetinvi;

namespace Wexflow.Tasks.Twitter
{
    public class Twitter : Task
    {
        public string ConsumerKey { get; }
        public string ConsumerSecret { get; }
        public string AccessToken { get; }
        public string AccessTokenSecret { get; }

        public Twitter(XElement xe, Workflow wf) : base(xe, wf)
        {
            ConsumerKey = GetSetting("consumerKey");
            ConsumerSecret = GetSetting("consumerSecret");
            AccessToken = GetSetting("accessToken");
            AccessTokenSecret = GetSetting("accessTokenSecret");
        }

        public override TaskStatus Run()
        {
            Info("Sending tweets...");

            bool success = true;
            bool atLeastOneSucceed = false;

            var files = SelectFiles();

            if (files.Length > 0)
            {
                try
                {
                    TweetinviConfig.ApplicationSettings.HttpRequestTimeout = 20000;
                    TweetinviConfig.CurrentThreadSettings.InitialiseFrom(TweetinviConfig.ApplicationSettings);

                    Auth.SetUserCredentials(ConsumerKey, ConsumerSecret, AccessToken, AccessTokenSecret);
                    var authenticatedUser = User.GetAuthenticatedUser();

                    if (authenticatedUser == null) // Something went wrong but we don't know what
                    {
                        // We can get the latest exception received by Tweetinvi
                        var latestException = ExceptionHandler.GetLastException();
                        ErrorFormat("The following error occured : '{0}'", latestException.TwitterDescription);
                        Error("Authentication failed.");
                        return new TaskStatus(Status.Error);
                    }
                    Info("Authentication succeeded.");
                }
                catch (ThreadAbortException)
                {
                    throw;
                }
                catch (Exception e)
                {
                    ErrorFormat("Authentication failed: {0}", e.Message);
                    return new TaskStatus(Status.Error);
                }

                foreach (FileInf file in files)
                {
                    try
                    {
                        var xdoc = XDocument.Load(file.Path);
                        foreach (XElement xTweet in xdoc.XPathSelectElements("Tweets/Tweet"))
                        {
                            var status = xTweet.Value;
                            var tweet = Tweet.PublishTweet(status);
                            if (tweet != null)
                            {
                                InfoFormat("Tweet '{0}' sent. Id: {1}", status, tweet.Id);

                                if (!atLeastOneSucceed) atLeastOneSucceed = true;
                            }
                            else
                            {
                                ErrorFormat("An error occured while sending the tweet '{0}'", status);
                                success = false;
                            }
                        }

                    }
                    catch (ThreadAbortException)
                    {
                        throw;
                    }
                    catch (Exception e)
                    {
                        ErrorFormat("An error occured while sending the tweets of the file {0}.", e, file.Path);
                        success = false;
                    }
                }
            }

            var tstatus = Status.Success;

            if (!success && atLeastOneSucceed)
            {
                tstatus = Status.Warning;
            }
            else if (!success)
            {
                tstatus = Status.Error;
            }

            Info("Task finished.");
            return new TaskStatus(tstatus);
        }
    }
}