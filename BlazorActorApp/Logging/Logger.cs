﻿using NLog.Config;
using NLog.Targets;
using NLog;

namespace BlazorActorApp.Logging
{
    public static class Logger
    {
        public static NLog.Logger Log = LogManager.GetCurrentClassLogger();

        public static void Configure()
        {
            LoggingConfiguration config = new LoggingConfiguration();
            string layout = "[${longdate}] [${message}] [ThreadId:${threadid}]";

            // Log to console
            ColoredConsoleTarget consoleTarget = new ColoredConsoleTarget()
            {
                UseDefaultRowHighlightingRules = true,
                Layout = layout
            };
            config.AddRule(minLevel: NLog.LogLevel.Trace, maxLevel: NLog.LogLevel.Fatal, target: consoleTarget);

            // Log to file (minLevel: Info)
            FileTarget infoFileTarget = new FileTarget("info")
            {
                FileName = "${basedir}\\Logging\\${date:format=yyyy-MM-dd}.log",
                Layout = layout
            };
            config.AddRule(minLevel: NLog.LogLevel.Info, maxLevel: NLog.LogLevel.Fatal, target: infoFileTarget);

            // Log to file (minLevel: Trace)
            FileTarget traceFileTarget = new FileTarget("trace")
            {
                FileName = "${basedir}\\Logging\\${date:format=yyyy-MM-dd}.trace.log",
                Layout = layout
            };
            config.AddRule(minLevel: NLog.LogLevel.Trace, maxLevel: NLog.LogLevel.Fatal, target: traceFileTarget);

            // Log to SignalR LoggingHub
            var loggingHubTarget = new LoggingHubTarget("http://code.webnori.com/hubs/logging")
            {
                Layout = layout
            };
            config.AddRule(minLevel: NLog.LogLevel.Trace, maxLevel: NLog.LogLevel.Fatal, target: loggingHubTarget);

            LogManager.Configuration = config;
            Log = LogManager.GetCurrentClassLogger();
        }
    }
}
