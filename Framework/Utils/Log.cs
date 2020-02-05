using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Foster.Framework
{
    /// <summary>
    /// Foster Log system
    /// </summary>
    public static class Log
    {

        public enum Types
        {
            Message,
            Warning,
            Error
        }

        public struct LogLine
        {
            public string Caller;
            public Types Type;
            public string Text;
        }

        private static readonly StringBuilder log = new StringBuilder();
        public static readonly List<LogLine> Lines = new List<LogLine>();

        public static bool PrintToConsole = true;

        public static void Message(string message) => Message(null, message);
        public static void Message(string? caller, string message)
        {
            Line(caller, "INFO", ConsoleColor.DarkGray, ConsoleColor.White, message);
            Lines.Add(new LogLine { Caller = caller ?? "", Type = Types.Message, Text = message });
        }

        public static void Warning(string warning) => Warning(null, warning);
        public static void Warning(string? caller, string warning)
        {
            Line(caller, "WARN", ConsoleColor.DarkYellow, ConsoleColor.White, warning);
            Lines.Add(new LogLine { Caller = caller ?? "", Type = Types.Warning, Text = warning });
        }

        public static void Error(string error) => Error(null, error);
        public static void Error(Exception exception) => Error(exception.ToString());
        public static void Error(string? caller, string error)
        {
            Line(caller, "FAIL", ConsoleColor.DarkRed, ConsoleColor.White, error);
            Lines.Add(new LogLine { Caller = caller ?? "", Type = Types.Error, Text = error });
        }

        public static void WriteTo(string file)
        {
            using var writer = File.AppendText(file);
            writer.WriteLine("FOSTER ERROR LOG");
            writer.WriteLine(DateTime.Now.ToString());
            writer.Write(log.ToString());
            writer.WriteLine();
        }

        private static void Line(string? caller, string subtitle, ConsoleColor subtitleBg, ConsoleColor subtitleFg, string message)
        {
            Append("FOSTER", ConsoleColor.DarkCyan, ConsoleColor.White, true);

            Append("[", subtitleBg, subtitleFg);
            Append(subtitle, subtitleBg, subtitleFg);
            Append("]", subtitleBg, subtitleFg);

            if (caller != null)
            {
                Append(" ", ConsoleColor.Black, ConsoleColor.DarkGray);
                Append(caller, ConsoleColor.Black, ConsoleColor.DarkGray);
            }

            Append(": ", ConsoleColor.Black, ConsoleColor.DarkGray);
            Append(message);
            AppendLine();
        }

        private static void Append(string text, ConsoleColor bg = ConsoleColor.Black, ConsoleColor fg = ConsoleColor.White, bool consoleOnly = false)
        {
            if (PrintToConsole)
            {
                Console.BackgroundColor = bg;
                Console.ForegroundColor = fg;
                Console.Write(text);
            }

            if (!consoleOnly)
                log.Append(text);
        }

        private static void AppendLine()
        {
            if (PrintToConsole)
                Console.WriteLine();

            log.AppendLine();
        }

    }
}
