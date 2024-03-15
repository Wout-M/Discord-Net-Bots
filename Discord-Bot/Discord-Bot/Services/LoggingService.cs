﻿using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord_Bot.Services;
public class LoggingService
{
    private string _logDirectory { get; }
    private string _logFile => Path.Combine(_logDirectory, $"{DateTime.UtcNow.ToString("yyyy-MM-dd")}.txt");

    public LoggingService(DiscordSocketClient client, InteractionService command)
    {
        _logDirectory = "../logs";

        client.Log += LogAsync;
        command.Log += LogAsync;
    }

    private Task LogAsync(LogMessage message)
    {
        string text = message.Exception is CommandException cmdException
            ? $"[Command/{message.Severity}] {cmdException.Command.Aliases.First()} failed to execute in {cmdException.Context.Channel}: {cmdException}"
            : $"[General/{message.Severity}] {message}";

        if (message.Severity == LogSeverity.Error || message.Severity == LogSeverity.Critical)
        {
            if (!Directory.Exists(_logDirectory))
                Directory.CreateDirectory(_logDirectory);

            if (!File.Exists(_logFile))
                File.Create(_logFile).Dispose();

            File.AppendAllText(_logFile, $"{text}\n");
        }

        return Console.Out.WriteLineAsync(text);
    }
}
