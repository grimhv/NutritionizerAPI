using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Honeypox.GLog;

namespace Honeypox.Nutritionizer.Classes
{
    public class ParseIni
    {
        /// <summary>
        /// Gets a Dictionary from all key=value pairs it finds in a particular [section] of an ini file.
        /// </summary>
        /// <param name="filePath">Path of the ini file.</param>
        /// <param name="section">Section to look for.</param>
        /// <returns>Dictionary with strings for both key and value.</returns>
        public static Dictionary<string, string> GetSection(string filePath, string section)
        {
            string[] lines = System.IO.File.ReadAllLines(filePath);
            var results = new Dictionary<string, string>();
            bool skip = true;

            Logger logger = new Logger()
            {
                LogPath = @"c:\dev\api_cs_new\n_api.log"
            };

            // Ignoring commented and blank lines
            foreach (string line in lines.Where(x =>
                !(x.Trim().StartsWith("#") || x.Trim().StartsWith(";") || x.Trim() == "")))
            {
                // Getting rid of whitespace
                string trimmed = line.Trim();

                if (skip)
                {
                    // Comparing the line to the section while ignoring case
                    if (line.Equals($"[{section}]", StringComparison.OrdinalIgnoreCase))
                    {
                        // Since this is the section we want, we'll stop skipping
                        skip = false;
                    }
                }
                else
                { 
                    // This indicates a new section
                    if (trimmed.StartsWith("["))
                    {
                        // So we'll start skipping again
                        skip = true;
                    }
                    else
                    {
                        // Parsing the key/value pair from the line and adding it to our results dictionary
                        var keyAndValue = GetKeyAndValue(trimmed);

                        if (results.ContainsKey(keyAndValue.Item1))
                        {
                            // If the .ini file has duplicate keys under the same section, it's not well
                            logger.Error($"Error while parsing {filePath}: Key `{keyAndValue.Item1}` already exists. " +
                                $"Recommend checking the ini file for correctness.");
                        }
                        else
                        {
                            results.Add(
                                key: keyAndValue.Item1,
                                value: keyAndValue.Item2);
                        }
                    }
                }
            }
            return results;
        }

        /// <summary>Parses a string using regex and returns a tuple as (key, value)</summary>
        public static (string, string) GetKeyAndValue(string line)
        {
            Match match;
            string key = "";
            string value = "";

            Logger logger = new Logger()
            {
                LogPath = @"c:\dev\api_cs_new\n_api.log"
            };

            /* This accepts various combinations of `key=value` pairs, as follows:
             * In all cases:           match.Groups[2] = key
             *                         match.Groups[3] = (everything after the `=` without spaces, including quotes)
             *
             * `key='value'`
             *    or `key = 'value'`   match.Groups[4] = value (without single quotes)
             * `key="value"`
             *    or `key = "value"`   match.Groups[5] = value (without double quotes)
             * `key=value`
             *    or `key = value`     match.Groups[6] = value (same as [3])
             *    
             * The `key` can only be alphanumeric and underscore characters
             * The `value` can be anything
             */
            match = Regex.Match(line, "(([A-Za-z0-9_]+) {0,1}= {0,1})(\"(.+)\"|\'(.+)\'|(.+))");

            /* Since only one group between 4-6 will be populated depending on which format the key/value pair is in,
             * we just have to find out which one is not null, and that's our value
             */
            for (int x = 6; x > 3; x--)
            {
                if (string.IsNullOrEmpty(match.Groups[x].Value))
                {
                    continue;
                }
                else
                {
                    value = match.Groups[x].Value;
                }
            }

            // Ensuring the key is populated
            if (string.IsNullOrEmpty(match.Groups[2].Value))
            {
                logger.Error("Error while parsing Database Configuration file. Key not found.");
                return ("", "");
            }
            else
            {
                key = match.Groups[2].Value;
            }

            // Ensuring the value is populated
            if (string.IsNullOrEmpty(value))
            {
                logger.Error($"Error while parsing Database Config.  Value not found for key {key}.");
                return ("", "");
            }
            else
            {
                return (key, value);
            }
        }
    }
}
