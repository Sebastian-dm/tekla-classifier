using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Reflection;
using System.Runtime.InteropServices.ComTypes;
using System.Windows.Forms;
using Tekla.Structures.Model;
using Tekla.Structures.ModelInternal;
using static Tekla.Structures.Filtering.Categories.ComponentFilterExpressions;
using static Tekla.Structures.Filtering.Categories.PartFilterExpressions;
using static Tekla.Structures.Model.Beam;


namespace TeklaClassifier {

    public static class CsvReader {

        public static List<string> ReadHeadersFromCSV(string filePath) {
            using (var reader = new StreamReader(filePath)) {
                var headerLine = reader.ReadLine();
                if (headerLine == null)
                    return new List<string>();
                return headerLine
                    .Split(',')
                    .Select(h => h.Trim())
                    .ToList();
            }
        }

        public static Dictionary<string, Dictionary<string, string>> ReadDictionaryFromCSV(string filePath, bool ignoreDuplicateClassWarning = false) {
            var result = new Dictionary<string, Dictionary<string, string>>();

            using (var reader = new StreamReader(filePath)) {
                // Read header row
                var headerLine = reader.ReadLine();
                if (headerLine == null)
                    return result;

                var headers = headerLine
                    .Split(',')
                    .Select(h => h.Trim())
                    .ToArray();

                string line;
                while ((line = reader.ReadLine()) != null) {
                    var parts = line
                        .Split(',')
                        .Select(p => p.Trim())
                        .ToArray();

                    if (parts.Length == 0)
                        continue;

                    string outerKey = parts[0];

                    if (result.ContainsKey(outerKey)) {
                        if (ignoreDuplicateClassWarning) continue;

                        var dialog = MessageBox.Show(
                            $"Database contains multiple definitions for class: {outerKey}.\n" +
                            "The first definition will be used and subsequent are ignored.\n\n" +
                            "Close with Cancel to ignore similar messages.",
                            "Duplicate class definition",
                            MessageBoxButtons.OKCancel
                        );

                        if (dialog == DialogResult.Cancel)
                            ignoreDuplicateClassWarning = true;

                        continue;
                    }

                    var innerDict = new Dictionary<string, string>();

                    for (int i = 1; i < headers.Length && i < parts.Length; i++) {
                        innerDict[headers[i]] = parts[i];
                    }

                    result[outerKey] = innerDict;
                }
            }

            return result;
        }

        public static void AddEntryToDatabaseFile(string filePath, string key, List<string> headers, Dictionary<string, string> values) {
            var row = new List<string>() { key };

            for (int i = 1; i < headers.Count; i++) {

                if (values.TryGetValue(headers[i], out var value))
                    row.Add(value);
                else
                    row.Add(""); // missing value
            }

            using (StreamWriter writer = new StreamWriter(filePath, true)) {
                writer.WriteLine(string.Join(",", row));
            }
        }

        public static void SortCsvFile(string filePath, int sortColumnIndex = 0, bool hasHeader = true) {
            if (!File.Exists(filePath))
                return;

            string header = null;
            var lines = File.ReadAllLines(filePath).ToList();

            if (lines.Count == 0)
                return;

            if (hasHeader) {
                header = lines[0];
                lines = lines.Skip(1).ToList();
            }

            var sorted = lines
                .Select(l => l.Split(','))
                .OrderBy(cols => cols[sortColumnIndex])
                .Select(cols => string.Join(",", cols))
                .ToList();

            if (hasHeader)
                sorted.Insert(0, header);

            File.WriteAllLines(filePath, sorted);
        }
    }
}
