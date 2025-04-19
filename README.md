# serilog-wrap

## Usage

### Setup Logger
Configure PostgreSQL as Default Logger and Store 
```
//Top of Program.cs
var builder = WebApplication.CreateBuilder(args);
ConfigurationManager configuration = builder.Configuration;
builder.Host.UseSerilog(SeriLogger.Configure);
```
Add PostgreSQL Conn in appsettings.json
```
  "ConnectionStrings": {
    "LogConnectionString": "connectionstring to postgresql"
  }
```

### Register Dependency to use ISeriLogWrapper
```
//Rrogram.cs
builder.Services.AddSerilogWrapper();
```

### Logs destination table
Default table name is **logs**. It can be overrided in SeriLogger.cs => Configure
