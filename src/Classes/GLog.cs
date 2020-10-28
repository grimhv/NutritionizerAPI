using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Honeypox.GLog
{
    class Logger
    {
        /// <summary> Used to customize the text for the event level of an entry </summary>
        public class LogLevel
        {
            // References parent class Glog
            private Logger _logger;
            public LogLevel(Logger logger)
            {
                _logger = logger;
            }

            public string Warning { get; set; } = "WARN";
            public string Error { get; set; } = "ERRO";
            public string Critical { get; set; } = "CRIT";
            public string Debug { get; set; } = "DEBU";
            public string Custom { get; set; } = "CUST";
            public string Internal { get; } = "INTR";
        }

        // Allows reference to nested class LogLevel
        public LogLevel Level { get; set; }
        public Logger()
        {
            this.Level = new LogLevel(this);
        }

        /// <summary> Filepath where the log should be written </summary>
        public string LogPath { get; set; } = "GLog.log";

        /// <summary> Whether log entries should be timestamped </summary>
        public bool TimestampEntries { get;  set; } = true;

        /// <summary> Whether log entries should have lines after the first indented </summary>
        public bool IndentEntries { get; set; } = false;

        /// <summary> Whether to add a tab after the indent </summary>
        public bool TabulateEntries { get; set; } = true;

        /// <summary> Format used by DateTime for log entries </summary>
        public string DateFormat { get;  set; } = "MM/dd/yyyy hh:mm:ss tt";

        /// <summary> Flag for the path having previously thrown an exception </summary>
        private bool InvalidPath { get; set; } = false;

        /// <summary> The path that threw the exception </summary>
        private string InvalidPathValue { get; set; }

        /// <summary>
        /// This is used if an error occurred while trying to write the log entry
        /// overrides the EventLevel given by user, typically accompanied by a custom error
        /// </summary>
        private bool InternalError { get; set; } = false;

        /// <summary> Ensures the path exists.  This will flag `InvalidPath` until the path is changed </summary>
        private bool CheckPath()
        {
            // If the `InvalidPath` flag is set and `LogPath` hasn't changed, there's no reason to throw more exceptions
            if (InvalidPath)
            {
                if (LogPath == InvalidPathValue)
                {
                    return false;
                }
            }

            try
            {
                // I'm sure there's a cooler way to validate a filepath, but this works for now
                File.GetAttributes(LogPath);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error while accessing file: {LogPath}");
                Console.WriteLine(e);
                InvalidPath = true;
                InvalidPathValue = LogPath;
                return false;
            }

            return true;
        }
        /// <summary> Writes a warning message to the log file set by `GLog.LogPath` </summary>
        public void Warning(dynamic contents)
        {
            if (CheckPath())
            {
                List<string> formattedContents = ConvertContentsToList(contents);
                CommitEntry(formattedContents, Level.Warning);
            }
            else
            {
                Console.WriteLine($"Attempted to open path failed: {LogPath}");
            }
        }

        /// <summary> Writes an error message to the log file set by `GLog.LogPath` </summary>
        public void Error(dynamic contents)
        {
            if (CheckPath())
            {
                List<string> formattedContents = ConvertContentsToList(contents);
                CommitEntry(formattedContents, Level.Error);
            }
            else
            {
                Console.WriteLine($"Attempted to open path failed: {LogPath}");
            }
        }

        /// <summary> Writes a critical message to the log file set by `GLog.LogPath` </summary>
        public void Critical(dynamic contents)
        {
            if (CheckPath())
            {
                List<string> formattedContents = ConvertContentsToList(contents);
                CommitEntry(formattedContents, Level.Critical);
            }
            else
            {
                Console.WriteLine($"Attempted to open path failed: {LogPath}");
            }
        }

        /// <summary> Writes a warning message to the log file set by `GLog.LogPath` </summary>
        public void Debug(dynamic contents)
        {
            if (CheckPath())
            {
                List<string> formattedContents = ConvertContentsToList(contents);
                CommitEntry(formattedContents, Level.Debug);
            }
            else
            {
                Console.WriteLine($"Attempted to open path failed: {LogPath}");
            }
        }

        /// <summary> Writes a custom message to the log file set by `GLog.LogPath` </summary>
        public void Custom(dynamic contents)
        {
            if (CheckPath())
            {
                List<string> formattedContents = ConvertContentsToList(contents);
                CommitEntry(formattedContents, Level.Custom);
            }
            else
            {
                Console.WriteLine($"Attempted to open path failed: {LogPath}");
            }
        }

        /// <summary> Converts string[] to List<string> </summary>
        private List<string> ConvertContentsToList(string[] contents)
        {
            return RemoveWhitespace(contents.ToList());
        }

        /// <summary> Passes the List<string> along </summary>
        private List<string> ConvertContentsToList(List<string> contents)
        {
            return RemoveWhitespace(contents);
        }

        /// <summary> Converts the exception to a string, then to a List<string> </summary>
        private List<string> ConvertContentsToList(Exception contents)
        {
            return RemoveWhitespace(ConvertMultilineStringToList(contents.ToString()));
        }

        /// <summary> Converts the string to a list </summary>
        private List<string> ConvertContentsToList(string contents)
        {
            return RemoveWhitespace(ConvertMultilineStringToList(contents));
        }

        /// <summary> Converts an object to a List<string> </summary>
        private List<string> ConvertContentsToList(object contents)
        {
            // Attempt to convert the object to a list.  If an exception is thrown, set `InternalError` to 'true' and
            // change the contents to an internal error entry string
            try
            {
                return RemoveWhitespace(ConvertMultilineStringToList(contents.ToString()));
            }
            catch
            {
                InternalError = true;
                return new List<string> { "Unable to parse object as string." };
            }
        }

        private static List<string> ConvertMultilineStringToList(string contents)
        {
            // If the string has newlines, convert it to a list by lines first
            // Use "\r\n", "\n", "\r" over `System.Environment.NewLine` because it's more precise
            if (Regex.Match(contents, @"\n|\r", RegexOptions.IgnoreCase).Success)
            {
                return new List<string>(
                    contents.Split(
                        new[] { "\r\n", "\n", "\r" }, StringSplitOptions.None).ToList()
                );
            }
            else
            {
                return new List<string>() { contents };
            }
        }

        /// <summary> Removes empty strings or those with just whitespace from the end of a list </summary>
        private List<string> RemoveWhitespace(List<string> contents)
        {
            /* Keeps removing empty or null lines if they're last in the array
             * This is in case the contents passed to Write end in a newline
             *     i.e. "Hello World!\r\n"
             * In the above example, Split() will add an empty string into the list
             *     i.e. lines[0] = "Hello World!"
             *          lines[1] = ""
             * This also prevents erroneous, trailing whitespace from filling the log
             *
             * If, at any point within the loop, the `lines` list only has one element, it
             * more-than-likely means that the entire list was empty, which is an error
             * in itself, so we will log that instead
             */
            while (String.IsNullOrWhiteSpace(contents.Last()))
            {
                if (contents.Count == 1)
                {
                    InternalError = true;
                    contents[0] = "Attempted to write to log, but string was empty.";
                    break;
                }
                else
                {
                    contents = contents.Take(contents.Count() - 1).ToList();
                }
            }

            return contents;
        }

        /// <summary> Commits a log entry to file </summary>
        private void CommitEntry(List<string> lines, string eventLevel)
        {
            // Determine formatting options
            
            // INTERNAL ERROR
            if (InternalError)
            {
                eventLevel = Level.Internal + " ";
            }
            else
            {
                eventLevel += " ";
            }

            // TABULATION
            string tab = "";
            if (TabulateEntries)
            {
                // Use 4 spaces instead of \t
                tab = "    ";
            }

            // DATE
            string date = "";
            if (TimestampEntries)
            {
                // Inside a try/catch in case user gave a DateFormat that breaks it
                try
                {
                    date = DateTime.Now.ToString(DateFormat);
                }
                catch
                {
                    date = DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss tt");
                }
            }

            // INDENT
            string indentOffset = "";
            if (IndentEntries)
            {
                // Uses the length of the date and event level, + 2 for the ": " between date and contents
                // to line up vertically where the log entry message starts
                indentOffset = new string(' ', (date.Length + eventLevel.Length + 2));
            }

            /* Open file and write the multiline log entry -- by default, it will look like: 
             * 
             * CRIT 10/15/2020 06:00:08 PM: Exception was thrown:
             *     System.ArgumentException: Empty path name is not legal.
             *          at System.IO.StreamWriter.ValidateArgsAndOpenPath(String path, Boolean append, Encoding encoding, Int32 bufferSize)
             *          at System.IO.StreamWriter..ctor(String path, Boolean append)
             *          at System.IO.File.AppendText(String path)
             */
            try
            {
                using StreamWriter file = File.AppendText(this.LogPath);
                {
                    // Format each line depending on if it's the first line or subsequent lines
                    for (int i = 0; i < lines.Count; i++)
                    {
                        string line;
                        if (i <= 0)
                        {
                            line = $"{eventLevel}{date}: " + lines[i];
                        }
                        else
                        {
                            line = $"{indentOffset}{tab}" + Regex.Replace(lines[i], @"\n|\r", "");
                        }
                        file.WriteLine($"{line}");
                    }
                }
            }
            catch (DirectoryNotFoundException e)
            {
                Console.WriteLine($"Error while writing to file: {LogPath}");
                Console.WriteLine(e);
                InvalidPath = true;
                InvalidPathValue = LogPath;
            }
            
            // Reset internal error after entry is written
            InternalError = false;
        }

        /// <summary> Empties log file </summary>
        public void ClearLog()
        {
            if (CheckPath())
            {
                File.WriteAllText(LogPath, string.Empty);
            }
            else
            {
                Console.WriteLine($"Attempt to clear log file failed at path: {LogPath}");
            }
        }
    }
}
