using MySql.Data.MySqlClient;
using Npgsql;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.IO;
using System.Security;
using System.Threading;
using System.Xml.Linq;
using Teradata.Client.Provider;
using Wexflow.Core;
using System.Data.Odbc;
using System.Xml;
using System.Text;
using System.Data;
using Newtonsoft.Json;

namespace Wexflow.Tasks.SqlToJson
{
    public enum Type
    {
        SqlServer,
        Access,
        Oracle,
        MySql,
        Sqlite,
        PostGreSql,
        Teradata,
        Odbc
    }

    


    public class SqlToJson : Wexflow.Core.Task
    {
        public Type DbType { get; set; }
        public string ConnectionString { get; set; }
        public string SqlScript { get; set; }

        public SqlToJson(XElement xe, Workflow wf)
            : base(xe, wf)
        {
            DbType = (Type)Enum.Parse(typeof(Type), GetSetting("type"), true);
            ConnectionString = GetSetting("connectionString");
            SqlScript = GetSetting("sql", string.Empty);
        }

        public override TaskStatus Run()
        {
            Info("Executing SQL scripts...");

            bool success = true;
            bool atLeastOneSucceed = false;

            // Execute SqlScript if necessary
            try
            {
                if (!string.IsNullOrEmpty(SqlScript))
                {
                    ExecuteSql(SqlScript);
                    Info("The script has been executed through the sql option of the task.");
                }
            }
            catch (ThreadAbortException)
            {
                throw;
            }
            catch (Exception e)
            {
                ErrorFormat("An error occured while executing sql script. Error: {0}", e.Message);
                success = false;
            }

            // Execute SQL files scripts
            foreach (FileInf file in SelectFiles())
            {
                try
                {
                    var sql = File.ReadAllText(file.Path);
                    ExecuteSql(sql);
                    InfoFormat("The script {0} has been executed.", file.Path);

                    if (!atLeastOneSucceed) atLeastOneSucceed = true;
                }
                catch (ThreadAbortException)
                {
                    throw;
                }
                catch (Exception e)
                {
                    ErrorFormat("An error occured while executing sql script {0}. Error: {1}", file.Path, e.Message);
                    success = false;
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
            return new Wexflow.Core.TaskStatus(status, false);
        }

        private void ExecuteSql(string sql)
        {
            switch (DbType)
            {
                case Type.SqlServer:
                    using (var connection = new SqlConnection(ConnectionString))
                    using (var command = new SqlCommand(sql, connection))
                    {
                        ConvertToJson(connection, command);
                    }
                    break;
                case Type.Access:
                    using (var conn = new OleDbConnection(ConnectionString))
                    using (var comm = new OleDbCommand(sql, conn))
                    {
                        ConvertToJson(conn, comm);
                    }
                    break;
                case Type.Oracle:
                    using (var connection = new OracleConnection(ConnectionString))
                    using (var command = new OracleCommand(sql, connection))
                    {
                        ConvertToJson(connection, command);
                    }
                    break;
                case Type.MySql:
                    using (var connection = new MySqlConnection(ConnectionString))
                    using (var command = new MySqlCommand(sql, connection))
                    {
                        ConvertToJson(connection, command);
                    }
                    break;
                case Type.Sqlite:
                    using (var connection = new SQLiteConnection(ConnectionString))
                    using (var command = new SQLiteCommand(sql, connection))
                    {
                        ConvertToJson(connection, command);
                    }
                    break;
                case Type.PostGreSql:
                    using (var connection = new NpgsqlConnection(ConnectionString))
                    using (var command = new NpgsqlCommand(sql, connection))
                    {
                        ConvertToJson(connection, command);
                    }
                    break;
                case Type.Teradata:
                    using (var connenction = new TdConnection(ConnectionString))
                    using (var command = new TdCommand(sql, connenction))
                    {
                        ConvertToJson(connenction, command);
                    }
                    break;
                case Type.Odbc:
                    using (var connenction = new OdbcConnection(ConnectionString))
                    using (var command = new OdbcCommand(sql, connenction))
                    {
                        ConvertToJson(connenction, command);
                    }
                    break;
            }
        }

        private void ConvertToJson(DbConnection connection, DbCommand command)
        {
            connection.Open();
            var reader = command.ExecuteReader();

            if (reader.HasRows)
            {
                //var columns = new List<string>();

                //for (int i = 0; i < reader.FieldCount; i++)
                //{
                //    columns.Add(reader.GetName(i));
                //}

                string destPath = Path.Combine(Workflow.WorkflowTempFolder,
                                               string.Format("SqlToJson_{0:yyyy-MM-dd-HH-mm-ss-fff}.Json",
                                               DateTime.Now));
                //var xdoc = new XDocument();
                //var xobjects = new XElement("Records");

                //while (reader.Read())
                //{
                //    var xobject = new XElement("Record");

                //    foreach (var column in columns)
                //    {
                //        xobject.Add(new XElement("Cell"
                //            , new XAttribute("column", SecurityElement.Escape(column))
                //            , new XAttribute("value", SecurityElement.Escape(reader[column].ToString()))));
                //    }
                //    xobjects.Add(xobject);
                //}
                string result = ToJson(reader);
                //xdoc.Add(xobjects);
                WriteToJsonFile(destPath,result);
                Files.Add(new FileInf(destPath, Id));
                InfoFormat("Json file generated: {0}", destPath);
            }
        }

        public static void WriteToJsonFile(string filePath, string contentsToWriteToFile, bool append = false)
        {
            TextWriter writer = null;
            try
            {
                //var contentsToWriteToFile = JsonConvert.SerializeObject(objectToWrite);
                writer = new StreamWriter(filePath, append);
                writer.Write(contentsToWriteToFile);
            }
            finally
            {
                if (writer != null)
                    writer.Close();
            }
        }

        public static String ToJson(DbDataReader rdr)

        {

            StringBuilder sb = new StringBuilder();

            StringWriter sw = new StringWriter(sb);



            using (JsonWriter jsonWriter = new JsonTextWriter(sw))

            {

                jsonWriter.WriteStartArray();



                while (rdr.Read())

                {

                    jsonWriter.WriteStartObject();



                    int fields = rdr.FieldCount;



                    for (int i = 0; i < fields; i++)

                    {

                        jsonWriter.WritePropertyName(rdr.GetName(i));

                        jsonWriter.WriteValue(rdr[i]);

                    }



                    jsonWriter.WriteEndObject();

                }



                jsonWriter.WriteEndArray();

                

                return sw.ToString();

            }

        }


    }
}
