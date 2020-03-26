[![Downloads](https://img.shields.io/github/downloads/aelassas/Wexflow/total.svg)](https://wexflow.github.io/stats) [![Release](https://img.shields.io/github/release/aelassas/Wexflow)](https://github.com/aelassas/Wexflow/releases/latest) [![Nuget](https://img.shields.io/nuget/v/Wexflow)](https://www.nuget.org/packages/Wexflow) [![Wiki](https://img.shields.io/badge/github-wiki-181717.svg?maxAge=60)](https://github.com/aelassas/Wexflow/wiki) [![License](https://img.shields.io/github/license/aelassas/Wexflow)](https://github.com/aelassas/Wexflow/blob/master/LICENSE.txt) [![Gitter](https://badges.gitter.im/Join%20Chat.svg)](https://gitter.im/Wexflow/Lobby) [![Tweet](https://img.shields.io/twitter/url/http/shields.io.svg?style=social)](https://twitter.com/intent/tweet?text=Wexflow%20-%20Open-source%20and%20cross-platform%20workflow%20engine&url=https://wexflow.github.io&via=aelassas_dev) [![Follow](https://img.shields.io/twitter/url/https/twitter.com/fold_left.svg?style=social&label=Follow)](https://twitter.com/aelassas_dev)

# What's New?

Take a look at the [version 5.2](https://github.com/aelassas/Wexflow/releases/tag/v5.2). This version includes an upgrade to .NET Core 3.1 LTS, support for [PostgreSQL](https://github.com/aelassas/Wexflow/wiki/PostgreSQL), [SQL Server](https://github.com/aelassas/Wexflow/wiki/SQL-Server), [MySQL](https://github.com/aelassas/Wexflow/wiki/MySQL) and [SQLite](https://github.com/aelassas/Wexflow/wiki/SQLite), JSON editor, display of server logs in the backend, parallel jobs of the same workflow definition, [date interpolation](https://github.com/aelassas/Wexflow/issues/166), bug fixes, performance enhancements and under the hood updates.

This version has been depthly tested on Windows, Linux, macOS and the cloud, and runs 100% on Windows, Linux, macOS and the cloud.


# Continuous Integration

|  Server | Platform |Project| Status |
----------|--------|----------|-------|
|[Azure Pipelines](https://azure.microsoft.com/en-us/services/devops/pipelines/) (.NET and .NET Core)| Windows | Wexflow.sln |[![Build Status](https://aelassas.visualstudio.com/Wexflow/_apis/build/status/aelassas.Wexflow?branchName=master)](https://aelassas.visualstudio.com/Wexflow/_build/latest?definitionId=1&branchName=master)|
|[AppVeyor](https://www.appveyor.com/) (.NET and .NET Core)| Windows | Wexflow.sln |[![Build Status](https://ci.appveyor.com/api/projects/status/github/aelassas/Wexflow?svg=true)](https://ci.appveyor.com/project/aelassas/wexflow)|
|[GitHub Actions](https://github.com/aelassas/Wexflow/actions) (.NET Core)| Linux | Wexflow.Server.csproj |[![Actions Status](https://github.com/aelassas/Wexflow/workflows/.NET%20Core/badge.svg)](https://github.com/aelassas/Wexflow/actions)|
|[Travis](https://travis-ci.org/) (Android)| Linux | src/android |[![Build Status](https://travis-ci.org/aelassas/Wexflow.svg?branch=master)](https://travis-ci.org/aelassas/Wexflow) | 
|[Bitrise](https://www.bitrise.io/) (Android)|Linux| src/android | [![Build Status](https://app.bitrise.io/app/0fb832132f6afa6d/status.svg?token=j49g0Gx7rNWkl4s41xM_kA)](https://app.bitrise.io/app/0fb832132f6afa6d)|
|[CircleCI](https://circleci.com/) (Android)|Linux| src/android | [![CircleCI](https://circleci.com/gh/aelassas/Wexflow.svg?style=shield)](https://circleci.com/gh/aelassas/Wexflow)|
|[Travis](https://travis-ci.org/) (iOS)| macOS | src/ios |[![Build Status](https://travis-ci.org/aelassas/Wexflow.svg?branch=master)](https://travis-ci.org/aelassas/Wexflow) | 
|[Bitrise](https://www.bitrise.io/) (iOS)|macOS| src/ios | [![Build Status](https://app.bitrise.io/app/f8006552bdd4ee80/status.svg?token=Yd_71TrG-cqFvEC1oV5teQ)](https://app.bitrise.io/app/f8006552bdd4ee80)|
|[CircleCI](https://circleci.com/) (iOS)|macOS| src/ios | [![CircleCI](https://circleci.com/gh/aelassas/Wexflow.svg?style=shield)](https://circleci.com/gh/aelassas/Wexflow)|
|[FOSSA](https://fossa.com/) (All projects)| Linux | All projects | [![FOSSA Status](https://app.fossa.com/api/projects/git%2Bgithub.com%2Faelassas%2FWexflow.svg?type=shield)](https://app.fossa.com/projects/git%2Bgithub.com%2Faelassas%2FWexflow?ref=badge_shield)|
<!--|[Codecov](https://codecov.io/) (.NET and .NET Core)|Windows| Wexflow.sln |![codecov](https://codecov.io/gh/aelassas/Wexflow/branch/master/graph/badge.svg)|-->
<!--|[Coveralls](https://coveralls.io/) (.NET and .NET Core)|Linux| Wexflow.sln |[![Coverage Status](https://badgen.net/coveralls/c/github/aelassas/Wexflow)](https://coveralls.io/github/aelassas/Wexflow?branch=master)|-->

# Wexflow

Wexflow is a high-performance, extensible, modular and cross-platform workflow engine. The goal of Wexflow is to automate recurring tasks. With the help of Wexflow, building automation and workflow processes become easy. Wexflow also helps in making the long-running processes straightforward. The communication between systems or applications becomes easy through this powerful workflow engine.

Wexflow makes use of [.NET Core](https://www.microsoft.com/net/download), a cross-platform version of .NET for building websites, services, and console apps. Thus, Wexflow provides a cross-platform workflow server and a cross-platform backend for managing, designing and tracking workflows with ease and flexibility. Wexflow server and its backend run on Windows, Linux and macOS.

Wexflow also makes use of [Quartz.NET](https://www.quartz-scheduler.net/) open source job scheduling system that is used in large scale enterprise systems. Thus, Wexflow offers flexibility in planning workflow jobs such as [cron workflows](https://github.com/aelassas/Wexflow/wiki/Cron-scheduling).

Since workflows are typically long running processes, they will need to be persisted to storage between tasks. There are several persistence providers available. Wexflow provides [LiteDB](http://www.litedb.org/), [MongoDB](https://github.com/aelassas/Wexflow/wiki/MongoDB), [RavenDB](https://github.com/aelassas/Wexflow/wiki/RavenDB), [CosmosDB](https://github.com/aelassas/Wexflow/wiki/CosmosDB), [PostgreSQL](https://github.com/aelassas/Wexflow/wiki/PostgreSQL), [SQL Server](https://github.com/aelassas/Wexflow/wiki/SQL-Server), [MySQL](https://github.com/aelassas/Wexflow/wiki/MySQL) and [SQLite](https://github.com/aelassas/Wexflow/wiki/SQLite) persistence providers which enhance and improve the performance of this workflow engine. The user can choose the persistence provider of his choice at the installation.

Wexflow comes with a backend too, so you can search and filter among all your workflows, have real-time statistics on your workflows, manage your workflows with ease, design your workflows with ease, and track your workflows with ease:

![Dashboard](https://aelassas.github.io/wexflow/images/wbo-dashboard-4.4-2.png)

Just to give you an idea of what Wexflow does, this is a screenshot from the "Designer" page. Using the "Designer" page, we get a nice visual overview of the dependency graph of the workflow. Each node represents a task which has to be run:

![Designer](https://aelassas.github.io/wexflow/images/wbo-designer-4.4-1.png)

Moreover, the "Designer" page allows to edit workflows through its XML/JSON editor or its WYSIWYG form based editor:

![Designer](https://aelassas.github.io/wexflow/images/wbo-designer-4.4-2.png)

# Table Of Contents
- [Why Wexflow?](https://github.com/aelassas/Wexflow#why-wexflow)
- [Real Life Examples](https://github.com/aelassas/Wexflow#real-life-examples)
- [Benefits](https://github.com/aelassas/Wexflow#benefits)
- [Who's Using Wexflow?](https://github.com/aelassas/Wexflow#whos-using-wexflow)
- [Get Started](https://github.com/aelassas/Wexflow#get-started)
- [Building From Source](https://github.com/aelassas/Wexflow#building-from-source)
- [Contribute](https://github.com/aelassas/Wexflow#contribute)
- [Changelog](https://github.com/aelassas/Wexflow#changelog)
- [License](https://github.com/aelassas/Wexflow#license)
- [Credits](https://github.com/aelassas/Wexflow#credits)

# Why Wexflow?

- [x] [Free and open source](https://github.com/aelassas/Wexflow/wiki/Free-and-open-source).
- [x] [Easy to install and effortless configuration](https://github.com/aelassas/Wexflow/wiki/Installation).
- [x] [Straightforward and easy to use](https://github.com/aelassas/Wexflow/wiki/Usage).
- [x] [A cross-platform workflow server](https://github.com/aelassas/Wexflow/wiki/Workflow-server).
- [x] [A cross-platform backend](https://github.com/aelassas/Wexflow/wiki/Usage#backend).
- [x] [An Android app for managing workflows](https://github.com/aelassas/Wexflow/wiki/Usage#android-manager).
- [x] [An iOS app for managing workflows](https://github.com/aelassas/Wexflow/wiki/Usage#ios-manager).
- [x] [Sequential workflows](https://github.com/aelassas/Wexflow/wiki/Samples#sequential-workflows).
- [x] [Flowchart workflows](https://github.com/aelassas/Wexflow/wiki/Samples#flowchart-workflows).
- [x] [Approval workflows](https://github.com/aelassas/Wexflow/wiki/Samples#approval-workflows).
- [x] [100+ built-in tasks](https://github.com/aelassas/Wexflow/wiki/Tasks-documentation).
- [x] [User-driven](https://github.com/aelassas/Wexflow/wiki/User-driven).
- [x] [Cron scheduling](https://github.com/aelassas/Wexflow/wiki/Cron-scheduling).
- [x] [LiteDB, MongoDB, RavenDB and CosmosDB support](https://github.com/aelassas/Wexflow/wiki/Databases).
- [x] [PostgreSQL, SQL Server, MySQL and SQLite support](https://github.com/aelassas/Wexflow/wiki/Databases).
- [x] [Extensive logging and incident reporting](https://github.com/aelassas/Wexflow/wiki/Logging).
- [x] [Real-time stats](https://github.com/aelassas/Wexflow/wiki/Usage#dashboard).
- [x] [RESTful API](https://github.com/aelassas/Wexflow/wiki/RESTful-API).
- [x] [Extensible](https://github.com/aelassas/Wexflow/wiki/Extensible).

Discover more about the features in [details](https://github.com/aelassas/Wexflow/wiki).

<!--
- [x] [Modular](https://github.com/aelassas/Wexflow/wiki/Modular).
- [x] [Well documented](https://github.com/aelassas/Wexflow/wiki/).
- [x] [User driven](https://github.com/aelassas/Wexflow/wiki/User-driven).
- [x] [A cross-platform application for managing workflows](https://github.com/aelassas/Wexflow/wiki/Usage#manager).
- [x] [A cross-platform application for designing workflows](https://github.com/aelassas/Wexflow/wiki/Usage#designer).
- [x] [User management](https://github.com/aelassas/Wexflow/wiki/Usage#users).
- [x] [Workflow events](https://github.com/aelassas/Wexflow/wiki/Samples#workflow-events).
- [x] [Hot reloading](https://github.com/aelassas/Wexflow/wiki/Hot-reloading).
- [x] [Automation](https://github.com/aelassas/Wexflow/wiki/Automation).
- [x] [Monitoring](https://github.com/aelassas/Wexflow/wiki/Monitoring).-
-->

# Real Life Examples

- Orchestration engine.
- [Form submission approval processes](https://github.com/aelassas/Wexflow/wiki/Samples#form-submission-approval-workflow).
- Batch recording live video feeds.
- Batch transcoding audio and video files.
- Batch uploading videos and their metadata to YouTube SFTP dropbox.
- [YouTube integration](https://github.com/aelassas/Wexflow/wiki/Tasks-documentation#audio-and-video-tasks): Automatically upload, search for content and list uploaded videos on YouTube through YouTube Data API.
- [Vimeo integration](https://github.com/aelassas/Wexflow/wiki/Tasks-documentation#audio-and-video-tasks): Automatically upload and list uploaded videos on Vimeo through Vimeo API.
- [Instagram integration](https://github.com/aelassas/Wexflow/wiki/Tasks-documentation#social-media-tasks): Automatically upload images and videos to Instagram through Instagram API.
- [Twitter integration](https://github.com/aelassas/Wexflow/wiki/Tasks-documentation#social-media-tasks): Automatically send tweets through Twitter API.
- [Reddit integration](https://github.com/aelassas/Wexflow/wiki/Tasks-documentation#social-media-tasks): Automatically send posts and links to Reddit through Reddit API.
- [Slack integration](https://github.com/aelassas/Wexflow/wiki/Slack): Automatically send messages to Slack channels.
- [Twilio integration](https://github.com/aelassas/Wexflow/wiki/Twilio): Automatically send SMS messages.
- Batch encrypting and decrypting large files.
- Batch converting, resizing and cropping images.
- Creating and sending reports and invoices by email.
- Connecting systems and applications via watch folders.
- Batch downloading files over FTP/FTPS/SFTP/HTTP/HTTPS/Torrent.
- Batch uploading files over FTP/FTPS/SFTP.
- Database administration and maintenance.
- Synchronizing the content of local or remote directories.
- [Optimizing PDF files](https://blogs.datalogics.com/2018/11/26/wexflow-automating-datalogics-pdf-tools/).

# Benefits

- [x] Gain time by automating repetitive tasks.
- [x] Save money by avoiding re-work and corrections.
- [x] Reduce human error.
- [x] Become more efficient and effective in completing your tasks.
- [x] Become more productive in what you do.
- [x] Become consistent in what you do.

# Who's Using Wexflow?

[![Assurant](https://aelassas.github.io/wexflow/images/user_assurant-logo.png)](https://www.assurant.com)
<br>
<br>
<br>
[![BHV-Automation](https://aelassas.github.io/wexflow/images/user_bhv-logo-neu.jpg)](http://www.bhv-automation.de)
<br>
<br>
<br>
[![Datalogics](https://aelassas.github.io/wexflow/images/user_datalogics_logo.png)](https://www.datalogics.com)
<br>
<br>
<br>
[![eandbsoftware](https://aelassas.github.io/wexflow/images/user_eandbsoftware.png)](https://www.eandbsoftware.org)
<br>
<br>
<br>
[![glinksolutions](https://aelassas.github.io/wexflow/images/user_glinksolutions-logo.png)](https://glinksolutions.vn)
<br>
<br>
<br>
[![Yandex](https://aelassas.github.io/wexflow/images/user_Yandex_logo_en.png)](https://yandex.com)
<br>
<br>
<br>
[![AK](https://aelassas.github.io/wexflow/images/ak-system-software.jpg)](https://ak-system-software.de/?lang=en)

# Get Started

- [Download the latest release](https://github.com/aelassas/Wexflow/releases/latest)
- [Installation guide](https://github.com/aelassas/Wexflow/wiki/Installation)
- [Quick start](https://github.com/aelassas/Wexflow/wiki/Usage)
- [Workflow samples](https://github.com/aelassas/Wexflow/wiki/Samples)
- [Tasks documentation](https://github.com/aelassas/Wexflow/wiki/Tasks-documentation)

# Building From Source

To build from source, [follow these instructions](https://github.com/aelassas/Wexflow/wiki/Debug).

# Contribute

1. Turn off `autocrlf`:
   ```
   git config core.autocrlf false
    ```
2. Hack!
3. Make a pull request.

After your pull request has been reviewed, it can be merged into the repository.

To run unit tests, follow these [guidelines](https://github.com/aelassas/Wexflow/wiki/How-to-run-unit-tests%3F).

<!--
# Bugs and features
  
 If you found any issues with Wexflow, please submit a bug report at the [Issue Tracker](https://github.com/aelassas/Wexflow/issues). Please include the following:
 
  - The version of Wexflow you are using.
  - How to reproduce the issue (a step-by-step description).
  - Expected result.
 
If you'd like to add a feature request please add some details how it is supposed to work.
-->

# Changelog

The changelog is available in the [release history](https://github.com/aelassas/Wexflow/wiki/History).

# License

Wexflow is licensed under the [MIT License](https://github.com/aelassas/Wexflow/blob/master/LICENSE.txt). Wexflow contains other libraries with their individual licenses. More details about these licenses can be found in the [wiki](https://github.com/aelassas/Wexflow/wiki/License).

[![FOSSA Status](https://app.fossa.io/api/projects/git%2Bgithub.com%2Faelassas%2FWexflow.svg?type=large)](https://app.fossa.io/projects/git%2Bgithub.com%2Faelassas%2FWexflow?ref=badge_large)

# Credits

Thanks to [JetBrains](https://www.jetbrains.com) for the free open source licenses.

Improved and optimized using:

<a href="https://www.jetbrains.com/resharper/"><img src="https://aelassas.github.io/wexflow/images/logo_resharper.gif" alt="Resharper" width="100" /></a>
