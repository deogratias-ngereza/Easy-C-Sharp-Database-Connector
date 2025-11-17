using GMTDBHelper.App.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace GMTDBHelper.App
{
    public class DBHelper
    {

        public static string conn_str()
        {
            var executing_path = System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath);
            var connStr = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source='" + executing_path + @"\db\db.accdb';Jet OLEDB:Database Password=password;";
            return connStr;
        }
        public static string getConnectionString(DBFileConfig dbConfig)
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
                if (dbConfig.db_path == "" || dbConfig.db_path == null)
                {
                    return @"Data Source=" + executing_path + @"\db\" + dbConfig.db_name + ";Version=3;";
                }
                else
                {
                    return @"Data Source='" + dbConfig.db_path + "';Version=3;";
                }
                
            }
            else
            {
                throw new Exception("Unsupported database type: " + dbConfig.db_type);
            }
        }



    }
}
