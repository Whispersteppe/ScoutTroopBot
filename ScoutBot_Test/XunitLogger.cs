using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;

/// <summary>
/// Adds the preovider to log to xUnit test output
/// </summary>
public static class XUnitLoggerExtensions
{
    public static ILoggingBuilder AddXUnitLoggerProvider(this ILoggingBuilder builder, ITestOutputHelper output)
    {
        builder.AddProvider(new XUnitLoggerProvider(output));
        return builder;
    }
}

/// <summary>
/// XUnitLogger provider that creates loggers to log to xUnit test output
/// </summary>
public class XUnitLoggerProvider : ILoggerProvider, ISupportExternalScope
{
    private readonly ITestOutputHelper _output;
    private LoggerExternalScopeProvider? _scopeProvider;

    public XUnitLoggerProvider(ITestOutputHelper output)
    {
        _output = output;
    }

    /// <summary>
    /// Create a single instance of the logger
    /// </summary>
    /// <param name="categoryName"></param>
    /// <returns></returns>
    public ILogger CreateLogger(string categoryName)
    {
        return new XUnitLogger(_output, categoryName, _scopeProvider ?? new LoggerExternalScopeProvider());
    }

    public ILogger<T> CreateLogger<T>()
    {
        return new XUnitLogger<T>(_output, _scopeProvider ?? new LoggerExternalScopeProvider());
    }


    public void Dispose()
    {
    }

    public void SetScopeProvider(IExternalScopeProvider scopeProvider)
    {
        _scopeProvider = scopeProvider as LoggerExternalScopeProvider;
    }
}

/// <summary>
/// Generic logger for class T
/// </summary>
/// <typeparam name="T"></typeparam>
public class XUnitLogger<T> : XUnitLogger, ILogger<T> where T : notnull
{
    public XUnitLogger(ITestOutputHelper output, LoggerExternalScopeProvider scopeProvider)
        : base(output, typeof(T).FullName ?? "Unknown", scopeProvider)
    {
    }
}

/// <summary>
/// Logs to ITestOutputHelper to aid in debugging purposes
/// </summary>
public class XUnitLogger : ILogger
{
    private readonly ITestOutputHelper _output;
    private readonly string _categoryName;
    private readonly LoggerExternalScopeProvider _scopeProvider;

    public XUnitLogger(ITestOutputHelper output, string categoryName, LoggerExternalScopeProvider scopeProvider)
    {
        _categoryName = categoryName;
        _scopeProvider = scopeProvider;
        _output = output;
    }

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
    {
        return _scopeProvider.Push(state);
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return true;
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine($"[LogLevel: {logLevel}]")
            .AppendLine($" Time: {DateTime.Now.ToString("o")}")
            .AppendLine($" CategoryName: {_categoryName}")
            .AppendLine($" EventId: {eventId.Id} - {eventId.Name} ")
            .AppendLine($" Scope: {BuildScopeInformation()} ")
            .AppendLine($" Message: {formatter(state, exception)}")
            ;

        try
        {
            _output.WriteLine(sb.ToString());
        }
        catch (InvalidOperationException)
        {
            // Swallow this exception as the output is disposed
        }
    }
    private string BuildScopeInformation()
    {
        StringBuilder sb = new StringBuilder();
        _scopeProvider.ForEachScope((scope, builder) =>
        {
            builder.Append($" => {scope}");
        }, sb);
        return sb.ToString();
    }
}
