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
            public Types Type;
            public string Text;
        }

        private static readonly StringBuilder log = new StringBuilder();
        public static readonly List<LogLine> Lines = new List<LogLine>();

        public static bool PrintToConsole = true;

        public static void Message(string message)
        {
            Line("INFO", ConsoleColor.White, message);
            Lines.Add(new LogLine { Type = Types.Message, Text = message });
        }

        public static void Warning(string warning)
        {
            Line("WARN", ConsoleColor.Yellow, warning);
            Lines.Add(new LogLine { Type = Types.Warning, Text = warning });
        }

        public static void Error(Exception exception) => Error(exception.ToString());
        public static void Error(string error)
        {
            Line("FAIL", ConsoleColor.Red, error);
            Lines.Add(new LogLine { Type = Types.Error, Text = error });
        }

        public static void AppendToFile(string title, string file)
        {
            var directory = Path.GetDirectoryName(file);
            if (directory != null && !Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            var builder = new StringBuilder();
            builder.AppendLine($"{title} ERROR LOG");
            builder.AppendLine(DateTime.Now.ToString());
            builder.AppendLine(log.ToString());
            builder.AppendLine();

            if (File.Exists(file))
                builder.Append(File.ReadAllText(file));

            File.WriteAllText(file, builder.ToString());
        }

        private static void Line(string subtitle, ConsoleColor subtitleFg, string message)
        {
            Append("FOSTER", ConsoleColor.DarkCyan, true);

            Append(":", subtitleFg, true);
            Append(subtitle, subtitleFg);

            Append(": ", ConsoleColor.DarkGray);
            Append(message);
            AppendLine();
        }

        private static void Append(string text, ConsoleColor fg = ConsoleColor.White, bool consoleOnly = false)
        {
            if (PrintToConsole)
            {
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
