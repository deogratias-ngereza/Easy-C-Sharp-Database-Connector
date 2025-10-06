﻿using GMTDBHelper.App.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace GMTDBHelper.App
{
    public sealed class DBUtility
    {
        private static MySqlConnection MYSQL_CONNECTION;
        private static OleDbConnection ACCESS_CONNECTION;
        private static SQLiteConnection SQLITE_CONNECTION;

        public static string DB_TYPE = ""; // Can be MYSQL, ACCESS, or SQLITE

        public static DBFileConfig db_file_config;

        public DBUtility(string db_type, string connStr)
        {
            DB_TYPE = db_type;

            if (db_type == "MYSQL")
            {
                MYSQL_CONNECTION = new MySqlConnection(connStr);
            }
            else if (db_type == "ACCESS")
            {
                ACCESS_CONNECTION = new OleDbConnection(connStr);
            }
            else if (db_type == "SQLITE")
            {
                SQLITE_CONNECTION = new SQLiteConnection(connStr);
            }
        }

        public static void init(string db_type, string connStr)
        {
            DB_TYPE = db_type;

            if (db_type == "MYSQL" && MYSQL_CONNECTION == null)
            {
                MYSQL_CONNECTION = new MySqlConnection(connStr);
            }
            else if (db_type == "ACCESS" && ACCESS_CONNECTION == null)
            {
                ACCESS_CONNECTION = new OleDbConnection(connStr);
            }
            else if (db_type == "SQLITE" && SQLITE_CONNECTION == null)
            {
                SQLITE_CONNECTION = new SQLiteConnection(connStr);
            }
        }

        public static void checkOrSetConnection() {
            var connStr = "";
            if (db_file_config == null)
            {
                //db_file_config = DBUtility.set_main_db_file_config();
                db_file_config = DBUtility.set_main_db_file_config();
                DBUtility.DB_TYPE = db_file_config.db_type;
                connStr = DBUtility.getConnectionString(db_file_config);
            }
            //
            var db_type = db_file_config.db_type;

            if (db_type == "MYSQL" && MYSQL_CONNECTION == null)
            {
                MYSQL_CONNECTION = new MySqlConnection(connStr);
            }
            else if (db_type == "ACCESS" && ACCESS_CONNECTION == null)
            {
                ACCESS_CONNECTION = new OleDbConnection(connStr);
            }
            else if (db_type == "SQLITE" && SQLITE_CONNECTION == null)
            {
                SQLITE_CONNECTION = new SQLiteConnection(connStr);
            }

        }

        //its general method
        public static object getDataFromQuery<T>(string QUERY, string DATA_TYPE)
        {
            //check if db is connected
            DBUtility.checkOrSetConnection();

            if (DB_TYPE == "MYSQL")
            {
                return getDataFromMysqlQuery<T>(QUERY, DATA_TYPE);
            }
            else if (DB_TYPE == "ACCESS")
            {
                return getDataFromAccessQuery<T>(QUERY, DATA_TYPE);
            }
            else if (DB_TYPE == "SQLITE")
            {
                return getDataFromSQLiteQuery<T>(QUERY, DATA_TYPE);
            }
            else
            {
                return null;
            }
        }

        public static int runBasicQuery(string query)
        {
            //check if db is connected
            DBUtility.checkOrSetConnection();

            if (DB_TYPE == "MYSQL")
            {
                return runBasicMysqlQuery(query);
            }
            else if (DB_TYPE == "ACCESS")
            {
                return runBasicAccessQuery(query);
            }
            else if (DB_TYPE == "SQLITE")
            {
                return runBasicSQLiteQuery(query);
            }
            else
            {
                return -2;
            }
        }

        // MySQL Implementation
        private static Object getDataFromMysqlQuery<T>(string QUERY, string DATA_TYPE)
        {
            try
            {
                if (MYSQL_CONNECTION.State != ConnectionState.Open)
                {
                    MYSQL_CONNECTION.Open();
                }

                MySqlDataAdapter adapter = new MySqlDataAdapter(QUERY, MYSQL_CONNECTION);
                adapter.SelectCommand.CommandType = CommandType.Text;

                DataTable dt = new DataTable();
                adapter.Fill(dt);

                switch (DATA_TYPE)
                {
                    case "DT": return dt;
                    case "AD": return adapter;
                    case "LIST": return DBUtility.DataTable2ObjList<T>(dt);
                    default: return DBUtility.DataTable2ObjList<T>(dt);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
            finally { MYSQL_CONNECTION.Close(); };
        }

        private static int runBasicMysqlQuery(string query)
        {
            try
            {
                if (MYSQL_CONNECTION.State != ConnectionState.Open)
                {
                    MYSQL_CONNECTION.Open();
                }

                MySqlCommand command = new MySqlCommand(query, MYSQL_CONNECTION);
                command.ExecuteNonQuery();
                return 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                //MsgBox.
                return 1;
            }
            finally { MYSQL_CONNECTION.Close(); };
        }

        // MS Access Implementation
        private static object getDataFromAccessQuery<T>(string query, string dataType)
        {
            try
            {
                if (ACCESS_CONNECTION.State != ConnectionState.Open)
                {
                    ACCESS_CONNECTION.Open();
                }

                OleDbDataAdapter adapter = new OleDbDataAdapter(query, ACCESS_CONNECTION);
                adapter.SelectCommand.CommandType = CommandType.Text;
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                switch (dataType)
                {
                    case "DT": return dt;
                    case "AD": return adapter;
                    case "LIST": return DBUtility.DataTable2ObjList<T>(dt);
                    default: return DBUtility.DataTable2ObjList<T>(dt);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                MessageBox.Show(ex.Message);
                return null;
            }
            finally { ACCESS_CONNECTION.Close(); };
        }

        private static int runBasicAccessQuery(string query)
        {
            try
            {
                if (ACCESS_CONNECTION.State != ConnectionState.Open)
                {
                    ACCESS_CONNECTION.Open();
                }

                OleDbCommand command = new OleDbCommand(query, ACCESS_CONNECTION);
                command.ExecuteNonQuery();
                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                MessageBox.Show(ex.Message);
                return 1;
            }
            finally { ACCESS_CONNECTION.Close(); };
        }

        // SQLite Implementation
        private static object getDataFromSQLiteQuery<T>(string query, string dataType)
        {
            try
            {
                if (SQLITE_CONNECTION.State != ConnectionState.Open)
                {
                    SQLITE_CONNECTION.Open();
                }

                SQLiteDataAdapter adapter = new SQLiteDataAdapter(query, SQLITE_CONNECTION);
                adapter.SelectCommand.CommandType = CommandType.Text;
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                switch (dataType)
                {
                    case "DT": return dt;
                    case "AD": return adapter;
                    case "LIST": return DBUtility.DataTable2ObjList<T>(dt);
                    default: return DBUtility.DataTable2ObjList<T>(dt);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                MessageBox.Show(ex.Message);
                return null;
            }
            finally { SQLITE_CONNECTION.Close(); };
        }

        private static int runBasicSQLiteQuery(string query)
        {
            try
            {
                if (SQLITE_CONNECTION.State != ConnectionState.Open)
                {
                    SQLITE_CONNECTION.Open();
                }

                SQLiteCommand command = new SQLiteCommand(query, SQLITE_CONNECTION);
                command.ExecuteNonQuery();
                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return 1;
            }
            finally { SQLITE_CONNECTION.Close(); };
        }


        public static List<T> DataTable2ObjList<T>(DataTable table)
        {
            string json = DBUtility.DataTable2Json(table);
            List<T> list = Newtonsoft.Json.JsonConvert.DeserializeObject<List<T>>(json);
            return list;
        }
        public static string DataTable2Json(DataTable table)
        {
            var JSONString = new StringBuilder();
            if (table.Rows.Count > 0)
            {
                JSONString.Append("[");
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    JSONString.Append("{");
                    for (int j = 0; j < table.Columns.Count; j++)
                    {
                        if (j < table.Columns.Count - 1)
                        {
                            JSONString.Append("\"" + table.Columns[j].ColumnName.ToString() + "\":" + "\"" + table.Rows[i][j].ToString() + "\",");
                        }
                        else if (j == table.Columns.Count - 1)
                        {
                            JSONString.Append("\"" + table.Columns[j].ColumnName.ToString() + "\":" + "\"" + table.Rows[i][j].ToString() + "\"");
                        }
                    }
                    if (i == table.Rows.Count - 1)
                    {
                        JSONString.Append("}");
                    }
                    else
                    {
                        JSONString.Append("},");
                    }
                }
                JSONString.Append("]");
            }
            return JSONString.ToString();
        }


        /// <summary>
        /// RETURN CURRENT EXECUTING PATH
        /// </summary>
        /// <returns></returns>
        private static string get_my_path()
        {
            try
            {
                var executing_path = System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath);
                string targetDir = string.Format(executing_path + @"\");
                return targetDir;
            }
            catch (Exception ex)
            {
                Console.WriteLine("--- LOADING PATH ERROR : --- " + ex.Message);
                return "";
            }
        }

        /// <summary>
        /// Get DB CONFIG FILE
        /// </summary>
        /// <returns></returns>
        private static DBFileConfig set_main_db_file_config(string input_db_path = "")
        {
            try
            {
                //AppConfigConstantModel config = getAppConfigs();
                //open the file
                input_db_path = input_db_path == "" ? get_my_path() + @"config\db.json" : input_db_path;
                using (StreamReader r = new StreamReader(input_db_path))
                {
                    string json = r.ReadToEnd();
                    DBFileConfig configObject = Newtonsoft.Json.JsonConvert.DeserializeObject<DBFileConfig>(json);

                    db_file_config = configObject;

                    //
                    //configObject.server_ip = aesEncryptor.DecryptString(configObject.server_ip, config.AES_ENC_PASS);
                    //configObject.user = aesEncryptor.DecryptString(configObject.user, config.AES_ENC_PASS);
                    //configObject.password = aesEncryptor.DecryptString(configObject.password, config.AES_ENC_PASS);
                    return configObject;
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("DBFILE-LOADING-ERROR: " + ex.Message);
                return null;
            }
        }

        /// <summary>
        /// //GET CONNECTION STRING
        /// </summary>
        /// <param name="dbConfig"></param>
        /// <returns></returns>
        private static string getConnectionString(DBFileConfig dbConfig)
        {
            var executing_path = System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath);

            if (dbConfig.db_type == "MYSQL")
            {
                return @"Server=" + dbConfig.db_host +
                       ";Port=" + dbConfig.db_port +
                       ";Database=" + dbConfig.db_name +
                       ";Uid=" + dbConfig.db_username +
                       ";Password=" + dbConfig.db_password +
                       ";Allow Zero Datetime=True;Convert Zero Datetime=True;Persist Security Info=True;";
            }
            else if (dbConfig.db_type == "ACCESS")
            {
                return @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source='" + executing_path + @"\db\" + dbConfig.db_name + "';Jet OLEDB:Database Password=" + dbConfig.db_password + ";";
            }
            else if (dbConfig.db_type == "SQLITE")
            {
                return @"Data Source=" + executing_path + @"\db\" + dbConfig.db_name + ";Version=3;";
            }
            else
            {
                throw new Exception("Unsupported database type: " + dbConfig.db_type);
            }
        }
    }
}
