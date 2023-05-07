using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace NorskaLib.Spreadsheets
{
    // TO DO:
    // Switch to NorskaLib.Core parsing utilities
    public static class Utilities
    {
        public static string[] Split(string line)
        {
            bool isInsideQuotes = false;
            List<string> result = new List<string>();

            string temp = string.Empty;
            for (int i = 0; i < line.Length; i++)
                if (line[i] == '"')
                {
                    isInsideQuotes = !isInsideQuotes;

                    if (i == line.Length - 1)
                        result.Add(temp);
                }
                else
                {
                    if (!isInsideQuotes && line[i] == ',')
                    {
                        result.Add(temp);
                        temp = string.Empty;
                    }
                    else
                        temp += line[i];
                }

            return result.ToArray();
        }

        public static object Parse(string s, Type type)
        {
            return Parse(s, type, out var error);
        }
        public static object Parse(string s, Type type, out bool error)
        {
            error = false;

            if (type == typeof(string))
                return s;

            if (type == typeof(int))
                return ParseInt(s, out error);

            if (type == typeof(float))
                return ParseFloat(s, out error);

            if (type == typeof(bool))
                return ParseBool(s, out error);

            if (type.IsEnum)
            {
                object result;
                try
                {
                    result = Enum.Parse(type, s, true);
                }
                catch (ArgumentException)
                {
                    result = default(object);
                    error = false;
                }
                return result;
            }

            return default(object);
        }

        public static int ParseInt(string s, out bool error)
        {
            error = !int.TryParse(s, out var result);

            if (error)
                Debug.LogWarning($"Error at parsing '{s}' to Integer");

            return error
                ? 0
                : result;
        }

        public static readonly string[] TrueOptions
            = new string[]
            {
                "true", "yes"
            };
        public static readonly string[] FalseOptions
            = new string[]
            {
                "false", "no"
            };
        public static bool ParseBool(string s, out bool error)
        {
            s = s.ToLower();
            error = false;

            for (int i = 0; i < TrueOptions.Length; i++)
            {
                if (s == TrueOptions[i])
                    return true;
                if (s == FalseOptions[i])
                    return false;
            }

            error = true;
            return false;
        }

        public static float ParseFloat(string s, out bool error)
        {
            error = !float.TryParse(
                s.Replace(',', '.'),
                NumberStyles.Float,
                CultureInfo.InvariantCulture,
                out var result);
            return result;
        }
    }
}
