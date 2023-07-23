using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;

namespace NorskaLib.Spreadsheets
{
    public class SpreadsheetImporter
    {
        public const string URLFormat = @"https://docs.google.com/spreadsheets/d/{0}/gviz/tq?tqx=out:csv&sheet={1}";

        private readonly object targetObject;
        private readonly FieldInfo[] targetListsFields;
        private readonly string documentID;

        public event Action onComplete;

        public bool abort;

        private string output;
        public string Output
        {
            get => output;

            private set
            {
                output = value;
                onOutputChanged.Invoke();
            }
        }
        public event Action onOutputChanged;

        public float progress;
        public float Progress
        {
            get => progress;

            private set
            {
                progress = Mathf.Clamp01(value);
                onProgressChanged.Invoke();
            }
        }
        public event Action onProgressChanged;

        private float ProgressElementDelta
            => 1f / targetListsFields.Length;

        public SpreadsheetImporter(object targetObject, FieldInfo[] targetListsFields, string documentID)
        {
            this.targetObject = targetObject;
            this.targetListsFields = targetListsFields;
            this.documentID = documentID;
        }

        public async void Run()
        {
            abort = false;
            var webClient = new WebClient();

            for (int i = 0; i < targetListsFields.Length && !abort; i++)
                await PopulateList(targetObject, targetListsFields[i], webClient);

            webClient.Dispose();

            onComplete?.Invoke();
        }

        private async Task PopulateList(object targetObject, FieldInfo targetListField, WebClient webClient)
        {
            var contentType = targetListField.FieldType.GetGenericArguments().SingleOrDefault();
            if (contentType is null)
                throw new Exception($"Could not identify type of defs stored in {targetListField.Name}");

            #region Downloading page

            var pageAttribute = (SpreadsheetPageAttribute)Attribute.GetCustomAttribute(targetListField, typeof(SpreadsheetPageAttribute));
            var pageName = pageAttribute.name;

            Output = $"Downloading page '{pageName}'...";

            var url = string.Format(URLFormat, documentID, pageName);
            var request = default(Task<string>);

            try
            {
                request = webClient.DownloadStringTaskAsync(url);
            }
            catch (WebException)
            {
                abort = true;
                throw new Exception($"Bad URL '{url}'");
            }

            while (!request.IsCompleted)
                await Task.Delay(100);

            var rawTable = Regex.Split(request.Result, "\r\n|\r|\n");
            request.Dispose();

            Progress += 1 / 3f * ProgressElementDelta;

            #endregion

            #region Analyzing and splitting raw text

            Output = $"Analysing headers...";

            var headersRaw = Utilities.Split(rawTable[0]);

            var idHeaderIdx = -1;
            var headers = new List<string>();
            var emptyHeadersIdxs = new List<int>();
            for (int i = 0; i < headersRaw.Length; i++)
            {
                if (string.IsNullOrEmpty(headersRaw[i]))
                {
                    emptyHeadersIdxs.Add(i);
                    continue;
                }

                if (idHeaderIdx == -1 && headersRaw[i].ToLower() == "id")
                    idHeaderIdx = i;

                headers.Add(headersRaw[i]);
            }

            var rows = new List<string[]>();
            for (int i = 1; i < rawTable.Length; i++)
            {
                var substrings = Utilities.Split(rawTable[i]);
                if (idHeaderIdx != -1 && string.IsNullOrEmpty(substrings[idHeaderIdx]))
                    continue;

                rows.Add(substrings.Where((val, index) => !emptyHeadersIdxs.Contains(index)).ToArray());
            }

            Progress += 1 / 3f * ProgressElementDelta;

            #endregion

            #region Parsing and populating list of defs 

            Output = $"Populating list of defs '{targetListField.Name}'<{contentType.Name}>...";

            var headersToFields = new Dictionary<string, FieldInfo>();
            foreach (var header in headers)
            {
                // TO DO:
                // Add support of fields with names other than the header names via an attribute
                var bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
                var fieldInfo = contentType.GetField(header, bindingFlags);
                if (fieldInfo is null)
                {
                    Debug.LogWarning($"Header '{header}' match no field in {contentType.Name} type");
                    continue;
                }
                headersToFields.Add(header, fieldInfo);
            }

            var list = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(contentType));
            foreach (var row in rows)
            {
                var item = Activator.CreateInstance(contentType);

                for (int i = 0; i < headers.Count; i++)
                    if (headersToFields.TryGetValue(headers[i], out var field))
                        field.SetValue(item, Utilities.Parse(row[i], field.FieldType));

                list.Add(item);
            }

            targetListField.SetValue(targetObject, list);

            Progress += 1 / 3f * ProgressElementDelta;

            #endregion
        }
    }
}
