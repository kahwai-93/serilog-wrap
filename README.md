# serilog-wrap

### Usage
```
//Top of Program.cs
var builder = WebApplication.CreateBuilder(args);
ConfigurationManager configuration = builder.Configuration;
builder.Host.UseSerilog(SeriLogger.Configure);

--- Code ---

//Register Dependency
builder.Services.AddSerilogWrapper();
```
