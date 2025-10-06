# Easy-C-Sharp-Database-Connector (GMTDBHelper)

A lightweight utility library to quickly connect to MySQL, MS Access, or SQLite and run simple queries with minimal setup. Designed to be embedded as a reusable library (DLL) in other .NET projects.

This README is the canonical documentation for developers and AI agents that need to integrate, reuse, and automate tasks with this library.

- Assembly: GMTDBHelper.dll
- Namespaces:
  - GMTDBHelper.App
  - GMTDBHelper.App.Models
- Core types:
  - DBUtility (primary API)
  - DBFileConfig (configuration model)
  - DBHelper (connection string helper)

Supported databases:
- MySQL
- Microsoft Access (.accdb)
- SQLite


## Features

- Zero-ORM, simple query execution
- Auto-config through a JSON file next to your app executable
- Unified API for MySQL, Access, and SQLite
- Read results as:
  - List<T> mapped by column names
  - DataTable
  - DataAdapter
- Execute non-query SQL (INSERT/UPDATE/DELETE) via a single helper

Note: This library uses System.Windows.Forms APIs internally (for path resolution and error messages). It is best suited for desktop apps (WinForms/WPF) or environments where Application.ExecutablePath exists. It targets .NET Framework style environments.


## Installation

1) Add a reference to GMTDBHelper.dll in your project.
2) Ensure the following dependencies are present (Copy Local = true or distributed with your app):
   - MySql.Data.dll (for MySQL)
   - System.Data.SQLite.dll (+ related EF6/artifacts if used)
   - Newtonsoft.Json.dll
3) Create a configuration file at:
   - [YourExecutableDirectory]/config/db.json
4) For Access/SQLite, also place your database file in:
   - [YourExecutableDirectory]/db/

Tip: In Visual Studio, mark your config and db assets as “Copy to Output Directory”.


## Configuration (config/db.json)

Place a db.json file next to the executable under config/db.json. The library will automatically load it on first use.

DBFileConfig fields:
- db_type: "MYSQL" | "ACCESS" | "SQLITE"
- db_username: string
- db_password: string
- db_name: string (for Access/SQLite, the file name, e.g. "db.accdb" or "app.db")
- db_host: string (MySQL only)
- db_port: number (MySQL only)
- conn_str_extras: string (reserved for future use)

Examples:

MySQL:
{
  "db_type": "MYSQL",
  "db_host": "127.0.0.1",
  "db_port": 3306,
  "db_name": "testdb",
  "db_username": "root",
  "db_password": "password",
  "conn_str_extras": ""
}

Microsoft Access:
{
  "db_type": "ACCESS",
  "db_name": "db.accdb",
  "db_password": "password",
  "db_username": "",
  "db_host": "",
  "db_port": 0,
  "conn_str_extras": ""
}

SQLite:
{
  "db_type": "SQLITE",
  "db_name": "app.db",
  "db_password": "",
  "db_username": "",
  "db_host": "",
  "db_port": 0,
  "conn_str_extras": ""
}

Paths resolved by the library:
- Access: [Application.ExecutablePath]/db/[db_name]
- SQLite: [Application.ExecutablePath]/db/[db_name]
- Config: [Application.ExecutablePath]/config/db.json


## Quick Start

- Add references and dependencies.
- Create config/db.json.
- (If Access/SQLite) Place your .accdb/.db under db/.
- Use DBUtility methods in your code.

Example POCO model (column-name matching):
using Newtonsoft.Json;

public class User
{
    // Property names must match column names (case-insensitive for JSON),
    // or decorate with JsonProperty to map different names.
    [JsonProperty("id")] public int Id { get; set; }
    [JsonProperty("name")] public string Name { get; set; }
    [JsonProperty("email")] public string Email { get; set; }
}


## Reading entries

The simplest and most common usage is to read rows into a strongly typed list.

- As List<T> (recommended for app logic):
List<User> users = (List<User>) DBUtility.getDataFromQuery<User>("SELECT id, name, email FROM users", "LIST");

- Single record:
User user = ((List<User>) DBUtility.getDataFromQuery<User>(
    "SELECT id, name, email FROM users WHERE id = 1",
    "LIST"
)).FirstOrDefault();

- As DataTable (e.g., for UI grids or dynamic processing):
System.Data.DataTable dt =
    (System.Data.DataTable) DBUtility.getDataFromQuery<object>("SELECT * FROM users", "DT");

- As DataAdapter (advanced scenarios):
var adapter = (System.Data.Common.DataAdapter) DBUtility.getDataFromQuery<object>("SELECT * FROM users", "AD");

Notes on mapping:
- Internally, the library converts the DataTable to JSON, then deserializes to List<T> using Newtonsoft.Json.
- Column names must align to your POCO property names.
- If database column names differ from your C# properties, use [JsonProperty("column_name")] attributes.


## Updating entries (INSERT, UPDATE, DELETE)

Use runBasicQuery for non-query SQL. Return codes:
- 0 = success
- 1 = error/exception occurred
- -2 = unsupported DB type (if not configured)

Examples:
- Insert:
int result = DBUtility.runBasicQuery(
    "INSERT INTO users (name, email) VALUES ('John', 'john@example.com')"
);
// result == 0 on success

- Update:
string safeName = name.Replace(\"'\", \"''\"); // BASIC escaping (see security note below)
int result = DBUtility.runBasicQuery(
    $"UPDATE users SET name = '{safeName}' WHERE id = {id}"
);

- Delete:
int result = DBUtility.runBasicQuery("DELETE FROM users WHERE id = 5");

Security note:
- The helper executes raw text SQL and does not provide parameterized commands.
- ALWAYS sanitize/escape user input or, preferably, perform updates through stored procedures or validated server-side logic. For public-facing apps, do not interpolate untrusted values directly into SQL.


## Selecting which database to use

By default, the library auto-loads config from config/db.json on first use of DBUtility APIs.

Two usage modes:

A) Config-file driven (recommended)
- Just create config/db.json.
- Do not call DBUtility.init manually.
- The library will read db.json, set DB_TYPE, and open the appropriate connection as needed.

B) Manual initialization (no db.json)
- Provide DBFileConfig in code and initialize the connection yourself:
using GMTDBHelper.App;
using GMTDBHelper.App.Models;

var cfg = new DBFileConfig
{
    db_type = "MYSQL",
    db_host = "127.0.0.1",
    db_port = 3306,
    db_name = "testdb",
    db_username = "root",
    db_password = "password"
};

// Set the config to skip reading the file:
DBUtility.db_file_config = cfg;

// Build a connection string using DBHelper:
string connStr = DBHelper.getConnectionString(cfg);

// Initialize the static connection once:
DBUtility.init(cfg.db_type, connStr);

// Now call queries:
List<User> users = (List<User>) DBUtility.getDataFromQuery<User>("SELECT * FROM users", "LIST");

Important: If you use manual init, set DBUtility.db_file_config before calling any query methods so that DBUtility.checkOrSetConnection does not try to read config/db.json and override DB_TYPE values.


## Type reference

DBUtility (GMTDBHelper.App)
- Static fields:
  - DBUtility.DB_TYPE: string ("MYSQL" | "ACCESS" | "SQLITE")
  - DBUtility.db_file_config: DBFileConfig (nullable)
- Initialization:
  - new DBUtility(string db_type, string connStr)  // rarely needed directly; sets DB_TYPE and creates connection
  - DBUtility.init(string db_type, string connStr) // preferred if doing manual initialization
- Query APIs:
  - object DBUtility.getDataFromQuery<T>(string QUERY, string DATA_TYPE)
    - DATA_TYPE:
      - "LIST" => returns List<T>
      - "DT"   => returns DataTable
      - "AD"   => returns DataAdapter
    - T is used only when DATA_TYPE = "LIST"
  - int DBUtility.runBasicQuery(string query)
    - returns 0/1/-2 as noted
- Helpers:
  - List<T> DBUtility.DataTable2ObjList<T>(DataTable table)
  - string  DBUtility.DataTable2Json(DataTable table)

DBFileConfig (GMTDBHelper.App.Models)
- Properties:
  - string db_type
  - string conn_str_extras  // currently unused
  - string db_username
  - string db_password
  - string db_name
  - string db_host
  - int    db_port

DBHelper (GMTDBHelper.App)
- Connection string helper:
  - static string getConnectionString(DBFileConfig dbConfig)
  - static string conn_str() // legacy example for Access in a fixed location


## Examples by database

MySQL:
- config/db.json:
{
  "db_type": "MYSQL",
  "db_host": "127.0.0.1",
  "db_port": 3306,
  "db_name": "testdb",
  "db_username": "root",
  "db_password": "password",
  "conn_str_extras": ""
}
- Code:
var users = (List<User>) DBUtility.getDataFromQuery<User>("SELECT * FROM users", "LIST");
int ok = DBUtility.runBasicQuery("UPDATE users SET name='Jane' WHERE id=1");

Access:
- Files:
  - db/db.accdb (your Access DB)
  - config/db.json:
{
  "db_type": "ACCESS",
  "db_name": "db.accdb",
  "db_password": "password",
  "db_username": "",
  "db_host": "",
  "db_port": 0,
  "conn_str_extras": ""
}
- Code:
var users = (List<User>) DBUtility.getDataFromQuery<User>("SELECT * FROM users", "LIST");

SQLite:
- Files:
  - db/app.db (your SQLite DB)
  - config/db.json:
{
  "db_type": "SQLITE",
  "db_name": "app.db",
  "db_password": "",
  "db_username": "",
  "db_host": "",
  "db_port": 0,
  "conn_str_extras": ""
}
- Code:
var users = (List<User>) DBUtility.getDataFromQuery<User>("SELECT * FROM users", "LIST");


## Common gotchas and tips

- Column to property mapping:
  - The library uses JSON mapping to convert rows to List<T>. Your POCO property names should match the column names, or use [JsonProperty("column_name")].
- Null handling:
  - All values are read from DataTable as strings before being serialized to JSON. Make sure your POCOs can handle string-to-type conversions. Consider using nullable types (int?).
- Error handling:
  - Errors may show via MessageBox (WinForms). DBUtility.runBasicQuery returns status codes instead of throwing.
- Thread safety:
  - Static connection objects are not guaranteed to be thread-safe. Prefer using on the UI thread or implement your own synchronization if needed.
- Parameterized queries:
  - Not provided by this helper. If you need parameterization, create your own command logic or wrap DBUtility with safer routines. Never pass untrusted input directly into SQL.
- App location:
  - All paths are relative to Application.ExecutablePath. In services or web apps, this path may differ from your project directory; ensure config/db.json and db/* reside next to the executable.


## Minimal end-to-end sample

- Model:
public class User
{
    public int id { get; set; }    // match DB column names
    public string name { get; set; }
    public string email { get; set; }
}

- Read list:
List<User> users = (List<User>) DBUtility.getDataFromQuery<User>("SELECT id, name, email FROM users", "LIST");

- Read single:
User u = ((List<User>) DBUtility.getDataFromQuery<User>(
    "SELECT id, name, email FROM users WHERE id = 42",
    "LIST"
)).FirstOrDefault();

- Update:
int status = DBUtility.runBasicQuery("UPDATE users SET email='new@ex.com' WHERE id=42");
// status == 0 => success

- Insert:
int s2 = DBUtility.runBasicQuery("INSERT INTO users (name,email) VALUES ('John','john@ex.com')");

- Delete:
int s3 = DBUtility.runBasicQuery("DELETE FROM users WHERE id=42");


## API one-liners you can copy

- List<User> users = (List<User>) DBUtility.getDataFromQuery<User>("SELECT * FROM users", "LIST");
- var dt = (System.Data.DataTable) DBUtility.getDataFromQuery<object>("SELECT * FROM users", "DT");
- int ok = DBUtility.runBasicQuery("UPDATE users SET name='Jane' WHERE id=1");


## Versioning and contributions

This repo is intended as a simple connector. If you extend it (e.g., add parameterized queries, logging, async APIs), please document your changes and consider contributing back via PR.


## License

MIT (or the license specified in your repository). Update this section as appropriate.
