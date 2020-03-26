﻿using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Wexflow.DotnetCore.Tests
{
    [TestClass]
    public class FilesExist
    {
        private static readonly string TempFolder = @"C:\Wexflow-dotnet-core\Temp\42";

        private static readonly string ExpectedResult =
            "<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n" +
            "<Root>\r\n" +
            "  <Files>\r\n" +
            "    <File path=\"C:\\WexflowTesting\\file1.txt\" name=\"file1.txt\" exists=\"true\" />\r\n" +
            "    <File path=\"C:\\WexflowTesting\\file2.txt\" name=\"file2.txt\" exists=\"true\" />\r\n" +
            "    <File path=\"C:\\WexflowTesting\\file3.txt\" name=\"file3.txt\" exists=\"true\" />\r\n" +
            "    <File path=\"C:\\WexflowTesting\\file41.txt\" name=\"file41.txt\" exists=\"false\" />\r\n" +
            "  </Files>\r\n" +
            "  <Folders>\r\n" +
            "    <Folder path=\"C:\\WexflowTesting\\Watchfolder1\" name=\"Watchfolder1\" exists=\"true\" />\r\n" +
            "    <Folder path=\"C:\\WexflowTesting\\Watchfolder2\" name=\"Watchfolder2\" exists=\"true\" />\r\n" +
            "    <Folder path=\"C:\\WexflowTesting\\Watchfolder41\" name=\"Watchfolder41\" exists=\"false\" />\r\n" +
            "  </Folders>\r\n" +
            "</Root>";

        [TestInitialize]
        public void TestInitialize()
        {
            if (!Directory.Exists(TempFolder)) Directory.CreateDirectory(TempFolder);
            Helper.DeleteFilesAndFolders(TempFolder);
        }

        [TestCleanup]
        public void TestCleanup()
        {
        }

        [TestMethod]
        public void FilesExistTest()
        {
            Helper.StartWorkflow(42);

            // Check the workflow result
            string[] files = Directory.GetFiles(TempFolder, "FilesExist*.xml", SearchOption.AllDirectories);
            Assert.AreEqual(files.Length, 1);

            string content = File.ReadAllText(files[0]);
            Assert.AreEqual(ExpectedResult, content);
        }
    }
}