using Serilog;
using Serilog.Events;

namespace LoggingShared.Config
{
    public static class LoggingConfiguration
    {
        public static void ConfigureSerilog(string serviceName, string? seqUrl = null)
        {
            seqUrl ??= Environment.GetEnvironmentVariable("SEQ_URL") ?? "http://seq:5341";

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("System", LogEventLevel.Warning)
                .MinimumLevel.Information()
                .Enrich.FromLogContext()
                .Enrich.WithProperty("Microservico", serviceName)
                .Enrich.WithEnvironmentName()
                .Enrich.WithThreadId()
                .Enrich.WithProcessId()
                .WriteTo.Console()
                .WriteTo.Seq(seqUrl)
                .CreateLogger();
        }
    }
}