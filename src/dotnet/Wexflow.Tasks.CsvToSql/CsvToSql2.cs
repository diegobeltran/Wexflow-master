using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Xml.Linq;
using Wexflow.Core;

namespace Wexflow.Tasks.CsvToSql
{
    public class CsvToSql2 : Task
    {
        public string TableName { get; set; }
        public string Separator { get; set; }

        public CsvToSql2(XElement xe, Workflow wf)
           : base(xe, wf)
        {
            TableName = GetSetting("tableName");
            Separator = GetSetting("separator");
        }

        public override TaskStatus Run()
        {
            Info("Converting CSV to SQL...");

            bool succeeded = true;
            bool atLeastOneSucceed = false;

            try
            {
                var csvFiles = SelectFiles();

                foreach (var csvFile in csvFiles)
                {
                    string sqlPath = Path.Combine(Workflow.WorkflowTempFolder,
                        string.Format("{0}_{1:yyyy-MM-dd-HH-mm-ss-fff}", Path.GetFileNameWithoutExtension(csvFile.FileName), DateTime.Now));
                    succeeded &= ConvertCsvToSql(csvFile.Path, sqlPath, TableName, Separator);
                    if (succeeded && !atLeastOneSucceed) atLeastOneSucceed = true;
                }

            }
            catch (ThreadAbortException)
            {
                throw;
            }
            catch (Exception e)
            {
                ErrorFormat("An error occured while converting CSV files to SQL: {0}", e.Message);
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

        private bool ConvertCsvToSql(string csvPath, string sqlPath, string tableName, string separator)
        {
            try
            {
                using (StreamReader sr = new StreamReader(csvPath))
                using (StreamWriter sw = new StreamWriter(sqlPath + ".sql"))
                {
                    string columnsLine = sr.ReadLine(); // First line contains columns
                    string line;
                    int counter = 0;
                    
                    string FileExtension = ".txt";
                    string FileDelimiter = separator;                    
                    string ColumnsDataType = "VARCHAR(500)";
                    string SchemaName = "dbo";                   
                    string ColumnList = "";
                    while (!string.IsNullOrEmpty(line = sr.ReadLine()))
                    {

                        if (counter == 0)
                        {
                            //Read the header and prepare Create Table Statement
                            ColumnList = "[" + columnsLine.Replace(FileDelimiter, "],[") + "]";
                            TableName = tableName;
                            string CreateTableStatement = "IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[" + SchemaName + "].";
                            CreateTableStatement += "[" + TableName + "]')";
                            CreateTableStatement += " AND type in (N'U'))DROP TABLE [" + SchemaName + "].";
                            CreateTableStatement += "[" + TableName + "]  Create Table " + SchemaName + ".[" + TableName + "]";
                            CreateTableStatement += "([" + columnsLine.Replace(FileDelimiter, "] " + ColumnsDataType + ",[") + "] " + ColumnsDataType + ")";

                            File.WriteAllText( sqlPath + ".create", CreateTableStatement);

                            Files.Add(new FileInf(sqlPath + ".create", Id));

                        }
                        counter++;

                        string values2 = line.Replace(separator, "");

                        if (values2.Trim() != "") 
                        { 

                        var values = line.Split(new string[] { separator }, StringSplitOptions.None);

                        sw.Write("INSERT INTO " + tableName + "(" + columnsLine.Replace(separator, ",").TrimEnd(',') + ")" + " VALUES ");
                        sw.Write("(");
                        int longitud= values.Length-1;
                            int contador = 0;
                            foreach (var value1 in values)
                        {
                            int i;
                            double d;
                            float f;
                            string value = value1;
                               
                            if (value== string.Empty)
                            {
                                    value="-";
                            }
                            

                            if (int.TryParse(value, out i))
                            {
                                sw.Write(i);
                            }
                            else if (double.TryParse(value, out d))
                            {
                                sw.Write(d);
                            }
                            else if (float.TryParse(value, out f))
                            {
                                sw.Write(f);
                            }
                            else
                            {
                                sw.Write("'" + value + "'");
                            }

                                if (!values.Last().Equals(value) && contador < longitud)
                            {
                                sw.Write(", ");
                            }
                                contador++;
                        }
                        sw.Write(");\r\n");
                    }
                    }
                    Files.Add(new FileInf(sqlPath + ".sql", Id));
                    InfoFormat("SQL script {0} created from {1} with success.", sqlPath, csvPath);

                    return true;
                }
            }
            catch (Exception e)
            {
                ErrorFormat("An error occured while converting the CSV {0} to SQL: {1}", csvPath, e.Message);
                return false;
            }
        }

    }
}
