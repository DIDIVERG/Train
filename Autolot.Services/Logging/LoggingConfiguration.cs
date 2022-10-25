using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NpgsqlTypes;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.PostgreSQL;

namespace Services.Logging;

public static class LoggingConfiguration
{
    private static readonly string OutputTemplate =
        @"[{Timestamp:yy-MM-dd HH:mm:ss} {Level}]{ApplicationName}:
{SourceContext}{NewLine} Message:{Message}{NewLine}in method
{MemberName} at {FilePath}:{LineNumber}{NewLine}{Exception}{NewLine}";

    private static IDictionary<string, ColumnWriterBase> _columnWriter = new Dictionary<string, ColumnWriterBase>
    {
        { "application_name", new SinglePropertyColumnWriter("Application name") },
        { "machine_name", new SinglePropertyColumnWriter("Machine name") },
        { "member_name", new SinglePropertyColumnWriter("Member name") },
        { "file_path", new SinglePropertyColumnWriter("File path") },
        {
            "line_number", new SinglePropertyColumnWriter(
                "Line number",
                PropertyWriteMethod.ToString,
                NpgsqlDbType.Integer)
        },
        { "source_context", new SinglePropertyColumnWriter("Source context") },
        { "request_path", new SinglePropertyColumnWriter("Request path") },
        { "action_name", new SinglePropertyColumnWriter("Action name") },
    };

    public static IHostBuilder ConfigureSerilog(this IHostBuilder builder)
    {
        builder.ConfigureLogging((context, logging) => { logging.ClearProviders(); })
            .UseSerilog((hostingContext, loggerConfiguration) =>
            {
                var config = hostingContext.Configuration;
                var connectionString = config.GetConnectionString("Default");
                var tableName = config["Logging:PostgreSQL:tableName"];
                var schema = config["Logging:PostgreSQL:schema"];
                var restrictedToMinimumLevel = config["Logging:PostgreSQL:restrictedToMinimumLevel"];
                if (!Enum.TryParse<LogEventLevel>(restrictedToMinimumLevel,
                        out var logLevel))
                {
                    logLevel = LogEventLevel.Debug;
                }
                LogEventLevel level = (LogEventLevel)Enum.Parse(typeof(LogEventLevel),
                    restrictedToMinimumLevel);
                loggerConfiguration.Enrich.FromLogContext()
                    .Enrich.WithMachineName()
                    .WriteTo.File(
                        path: "ErrorLog.txt",
                        rollingInterval: RollingInterval.Day,
                        restrictedToMinimumLevel: logLevel,
                        outputTemplate: OutputTemplate
                    ).WriteTo.Console(restrictedToMinimumLevel: logLevel)
                    .WriteTo.PostgreSQL(
                        tableName: tableName,
                        schemaName: schema,
                        needAutoCreateTable: true,
                        connectionString: connectionString,
                        restrictedToMinimumLevel: level,
                        columnOptions: _columnWriter
                        ); }
            );
        return builder;
    }
}