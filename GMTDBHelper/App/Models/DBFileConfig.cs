using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GMTDBHelper.App.Models
{
    public class DBFileConfig
    {
        public string db_type { get; set; }
        public string conn_str_extras { get; set; }
        public string db_username { get; set; }
        public string db_password { get; set; }
        public string db_name { get; set; }
        public string db_host { get; set; }
        public int db_port { get; set; }
    }
}
