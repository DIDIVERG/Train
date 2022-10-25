using Microsoft.Extensions.Configuration;
using Serilog.Context;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;

namespace Services.Logging;

public class AppLogging<T>: IAppLogging<T>
{
    private readonly ILogger _logger;
    private readonly IConfiguration _configuration;
    private readonly string _applicationName; 
    
    public AppLogging(ILogger logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
        _applicationName = configuration.GetValue<string>("ApplicationName");

    }

    private List<IDisposable> PushProperties(string memberName, string sourceFilePath, int sourceLineNumber)
    {
        List<IDisposable> list = new List<IDisposable>()
        {
            LogContext.PushProperty("MemberName", memberName),
            LogContext.PushProperty("FilePath", sourceFilePath),
            LogContext.PushProperty("LineNumber", sourceLineNumber),
            LogContext.PushProperty("ApplicationName", _applicationName)
        };
        
        return list;
    }

    private static void Dispose(IEnumerable<IDisposable> disposables)
    {
        foreach (var item in disposables)
        {
            item.Dispose();
        }
    }
    public void LogAppError(Exception ex, 
        string message, 
        [CallerMemberName] string memberName = "", 
        [CallerFilePath] string sourceFilePath = "",
        [CallerLineNumber] int sourceLineNumber = 0)
    {
        var list = PushProperties(memberName, sourceFilePath, sourceLineNumber);
        _logger.LogError(ex, message);
       Dispose(list);
    }

    public void LogAppError(string message, 
        [CallerMemberName]  string memberName = "", 
        [CallerFilePath]  string sourceFilePath = "", 
        [CallerLineNumber] int sourceLineNumber = 0)
    {
        var list = PushProperties(memberName, sourceFilePath, sourceLineNumber);
        _logger.LogError(message);
        Dispose(list);
    }

    public void LogAppCritical(string message, 
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string sourceFilePath = "", 
        [CallerLineNumber] int sourceLineNumber = 0)
    {
        var list = PushProperties(memberName, sourceFilePath, sourceLineNumber);
        _logger.LogCritical(message);
        Dispose(list);
    }

    public void LogAppCritical(Exception ex, 
        string message, 
        [CallerMemberName]  string memberName = "",
        [CallerFilePath] string sourceFilePath = "",
        [CallerLineNumber] int sourceLineNumber = 0)
    {
        var list = PushProperties(memberName, sourceFilePath, sourceLineNumber);
        _logger.LogCritical(ex, message);
        Dispose(list);
    }

    public void LogAppInformation(string message, 
        [CallerMemberName] string memberName = "", 
        [CallerFilePath] string sourceFilePath = "", 
        [CallerLineNumber] int sourceLineNumber = 0)
    {
        var list = PushProperties(memberName, sourceFilePath, sourceLineNumber);
        _logger.LogInformation(message);
        Dispose(list);
    }

    public void LogAppDebug(string message, 
        [CallerMemberName] string memberName = "", 
        [CallerFilePath] string sourceFilePath = "", 
        [CallerLineNumber] int sourceLineNumber = 0)
    {
        var list = PushProperties(memberName, sourceFilePath, sourceLineNumber);
        _logger.LogDebug(message);
        Dispose(list);
    }

    public void LogAppTrace(string message, 
        [CallerMemberName] string memberName = "", 
        [CallerFilePath] string sourceFilePath = "", 
        [CallerLineNumber] int sourceLineNumber = 0)
    {
        var list = PushProperties(memberName, sourceFilePath, sourceLineNumber);
        _logger.LogTrace(message);
        Dispose(list);    }

    public void LogAppWarning(string message, 
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string sourceFilePath = "",
        [CallerLineNumber] int sourceLineNumber = 0)
    {
        var list = PushProperties(memberName, sourceFilePath, sourceLineNumber);
        _logger.LogWarning(message);
        Dispose(list);
    }
}