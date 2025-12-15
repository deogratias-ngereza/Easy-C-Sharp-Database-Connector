# GMTDBHelper - Software Requirements

This document outlines all software requirements that must be installed on target machines before using the GMTDBHelper portable library.

## Core Requirements

### 1. .NET Framework 4.5 or Higher
**REQUIRED** - The library targets .NET Framework 4.5
- **Download**: [Microsoft .NET Framework 4.5](https://www.microsoft.com/en-us/download/details.aspx?id=30653)
- **Alternative**: .NET Framework 4.6, 4.7, 4.8 (recommended for better compatibility)
- **Check if installed**: Run `reg query "HKLM\SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full" /v Version` in Command Prompt

### 2. Visual C++ Redistributable for Visual Studio 2015-2022 (x64)
**REQUIRED** - Needed for SQLite native libraries
- **Download**: [Microsoft Visual C++ Redistributable](https://aka.ms/vs/17/release/vc_redist.x64.exe)
- **Note**: The library is compiled for x64 architecture (64-bit)

### 3. Microsoft Access Database Engine 2016 Redistributable (x64)
**REQUIRED ONLY IF USING MICROSOFT ACCESS (.accdb) DATABASES**
- **Download**: [Microsoft Access Database Engine 2016 Redistributable (64-bit)](https://www.microsoft.com/en-us/download/details.aspx?id=54920)
- **Important**: 
  - Install the 64-bit version since this library is compiled for x64
  - If you have 32-bit Office installed, you may need to use the `/passive` flag during installation
  - Installation command: `AccessDatabaseEngine_X64.exe /passive`

## Required DLL Dependencies

The following DLLs must be distributed with your application (should be in the same folder as GMTDBHelper.dll):

### 1. MySql.Data.dll
**REQUIRED FOR MYSQL SUPPORT**
- Purpose: MySQL database connectivity
- Download from: [MySQL Connector/NET](https://dev.mysql.com/downloads/connector/net/)
- Include in bin/Debug or bin/Release folder

### 2. Newtonsoft.Json.dll
**REQUIRED - ALWAYS**
- Purpose: JSON serialization/deserialization for data mapping
- Version: 4.5.0.0 or higher
- Download from: [Json.NET on NuGet](https://www.nuget.org/packages/Newtonsoft.Json/)
- **This library is critical** - used for converting DataTable to List<T>

### 3. System.Data.SQLite.dll (AMD64)
**REQUIRED FOR SQLITE SUPPORT**
- Purpose: SQLite database connectivity
- Version: 1.0.119.0 or compatible
- Architecture: AMD64 (64-bit)
- Download from: [System.Data.SQLite Download Page](https://system.data.sqlite.org/index.html/doc/trunk/www/downloads.wiki)
- **Important**: Download the x64 (64-bit) bundle

### 4. System.Data.SQLite.EF6.dll (Optional)
**RECOMMENDED FOR SQLITE**
- Purpose: Entity Framework 6 support for SQLite
- Version: 1.0.119.0 or compatible
- Include if using advanced queries

### 5. System.Data.SQLite.Linq.dll (Optional)
**RECOMMENDED FOR SQLITE**
- Purpose: LINQ support for SQLite
- Version: 1.0.119.0 or compatible
- Include if using LINQ queries

## Database-Specific Requirements

### For MySQL
- MySQL Connector/NET installed OR MySql.Data.dll distributed with your app
- MySQL Server accessible (local or remote)

### For Microsoft Access (.accdb files)
- Microsoft Access Database Engine 2016 Redistributable (x64) **MUST BE INSTALLED**
- .accdb database file placed in `[YourApp]/db/` folder

### For SQLite
- System.Data.SQLite.dll (x64) distributed with your app
- .db database file placed in `[YourApp]/db/` folder

## Architecture Requirements

- **OS Architecture**: 64-bit Windows (x64)
- **Platform Target**: This library is compiled for x64 (64-bit)
- **Will NOT work on 32-bit systems** without recompilation

## File Structure Requirements

Your application must maintain this folder structure:

```
YourApp.exe
├── GMTDBHelper.dll
├── MySql.Data.dll (if using MySQL)
├── Newtonsoft.Json.dll (ALWAYS REQUIRED)
├── System.Data.SQLite.dll (if using SQLite)
├── System.Data.SQLite.EF6.dll (optional)
├── System.Data.SQLite.Linq.dll (optional)
├── config/
│   └── db.json (database configuration)
└── db/
    ├── db.accdb (if using Access)
    └── app.db (if using SQLite)
```

## Configuration File (db.json)

Create `config/db.json` with appropriate settings for your database:

**MySQL Example:**
```json
{
  "db_type": "MYSQL",
  "db_host": "127.0.0.1",
  "db_port": 3306,
  "db_name": "testdb",
  "db_username": "root",
  "db_password": "password",
  "conn_str_extras": ""
}
```

**Microsoft Access Example:**
```json
{
  "db_type": "ACCESS",
  "db_name": "db.accdb",
  "db_password": "",
  "db_username": "",
  "db_host": "",
  "db_port": 0,
  "conn_str_extras": ""
}
```

**SQLite Example:**
```json
{
  "db_type": "SQLITE",
  "db_name": "app.db",
  "db_password": "",
  "db_username": "",
  "db_host": "",
  "db_port": 0,
  "conn_str_extras": ""
}
```

## Quick Installation Checklist

Before deploying to a new machine, ensure:

- [ ] .NET Framework 4.5+ installed
- [ ] Visual C++ Redistributable (x64) installed
- [ ] (If using Access) Microsoft Access Database Engine 2016 (x64) installed
- [ ] GMTDBHelper.dll included in application folder
- [ ] Newtonsoft.Json.dll included (REQUIRED)
- [ ] MySql.Data.dll included (if using MySQL)
- [ ] System.Data.SQLite.dll (x64) included (if using SQLite)
- [ ] config/db.json created with correct settings
- [ ] Database files placed in db/ folder (for Access/SQLite)
- [ ] Target system is 64-bit Windows

## Common Installation Errors

### Error: "Could not load file or assembly 'Newtonsoft.Json'"
**Solution**: Copy Newtonsoft.Json.dll to the same folder as your application executable

### Error: "Could not load file or assembly 'MySql.Data'"
**Solution**: Copy MySql.Data.dll to the same folder as your application executable

### Error: "The 'Microsoft.ACE.OLEDB.12.0' provider is not registered"
**Solution**: Install Microsoft Access Database Engine 2016 Redistributable (x64)

### Error: "Unable to load DLL 'SQLite.Interop.dll'"
**Solution**: 
1. Ensure System.Data.SQLite.dll is the x64 version
2. Install Visual C++ Redistributable (x64)
3. Check that SQLite.Interop.dll is in the correct subfolder (x64/)

### Error: "BadImageFormatException" or "Platform mismatch"
**Solution**: 
1. Ensure all DLLs are 64-bit versions
2. Target system must be 64-bit Windows
3. Do not mix 32-bit and 64-bit components

## Download Links Summary

1. **.NET Framework 4.5+**: https://dotnet.microsoft.com/download/dotnet-framework
2. **Visual C++ Redistributable (x64)**: https://aka.ms/vs/17/release/vc_redist.x64.exe
3. **Access Database Engine (x64)**: https://www.microsoft.com/en-us/download/details.aspx?id=54920
4. **MySQL Connector/NET**: https://dev.mysql.com/downloads/connector/net/
5. **System.Data.SQLite (x64)**: https://system.data.sqlite.org/downloads/
6. **Newtonsoft.Json**: https://www.nuget.org/packages/Newtonsoft.Json/

## Support

For issues related to this library, refer to the main README.md or the project repository.
