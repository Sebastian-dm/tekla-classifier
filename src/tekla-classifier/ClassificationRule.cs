using System;
using System.Collections.Generic;
using System.IO;

public sealed class ClassificationRule {
    // File order = priority
    public int Order { get; private set; }

    // All match conditions, e.g. Name, UDA:Layer, UDA:Zone
    public IDictionary<string, string> MatchValues { get; private set; }

    // Output values
    public string ClassCode { get; private set; }
    public string TypeDescription { get; private set; }

    public ClassificationRule(
        int order,
        IDictionary<string, string> matchValues,
        string classCode,
        string typeDescription) {
        Order = order;
        MatchValues = matchValues;
        ClassCode = classCode;
        TypeDescription = typeDescription;
    }

    public bool Matches(IDictionary<string, string> actualValues) {
        foreach (KeyValuePair<string, string> pair in MatchValues) {
            string actualValue;
            if (!actualValues.TryGetValue(pair.Key, out actualValue))
                return false;

            if (!string.Equals(pair.Value, actualValue, StringComparison.OrdinalIgnoreCase))
                return false;
        }

        return true;
    }
}

public static class ClassificationRuleLoader {
    public static IList<ClassificationRule> LoadRules(string csvPath) {
        if (string.IsNullOrWhiteSpace(csvPath))
            throw new ArgumentException("CSV path is required.", nameof(csvPath));

        if (!File.Exists(csvPath))
            throw new FileNotFoundException("Rule file not found.", csvPath);

        string[] lines = File.ReadAllLines(csvPath);

        if (lines.Length == 0)
            throw new InvalidDataException("Rule file is empty.");

        int headerLineIndex = FindFirstNonEmptyLine(lines);
        if (headerLineIndex < 0)
            throw new InvalidDataException("Rule file contains no data.");

        string[] headers = SplitAndTrim(lines[headerLineIndex]);

        int classCodeIndex = FindColumnIndex(headers, "ClassCode", true);
        int typeDescriptionIndex = FindColumnIndex(headers, "TypeDescription", true);

        List<int> matchColumnIndexes = new List<int>();
        for (int i = 0; i < headers.Length; i++) {
            string header = headers[i];

            if (string.Equals(header, "ClassCode", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(header, "TypeDescription", StringComparison.OrdinalIgnoreCase)) {
                continue;
            }

            if (string.Equals(header, "Name", StringComparison.OrdinalIgnoreCase) ||
                header.StartsWith("UDA:", StringComparison.OrdinalIgnoreCase)) {
                matchColumnIndexes.Add(i);
            }
        }

        if (matchColumnIndexes.Count == 0)
            throw new InvalidDataException("At least one match column is required: Name and/or UDA:*");

        List<ClassificationRule> rules = new List<ClassificationRule>();
        int order = 0;

        for (int lineIndex = headerLineIndex + 1; lineIndex < lines.Length; lineIndex++) {
            string line = lines[lineIndex];

            if (string.IsNullOrWhiteSpace(line))
                continue;

            string[] cells = SplitAndTrim(line);

            string classCode = Normalize(GetValue(cells, classCodeIndex));
            string TypeDescription = Normalize(GetValue(cells, typeDescriptionIndex));

            if (classCode == null)
                throw new InvalidDataException("Row " + (lineIndex + 1) + ": ClassCode is required.");

            if (TypeDescription == null)
                throw new InvalidDataException("Row " + (lineIndex + 1) + ": TypeDescription is required.");

            Dictionary<string, string> matchValues =
                new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            for (int i = 0; i < matchColumnIndexes.Count; i++) {
                int columnIndex = matchColumnIndexes[i];
                string header = headers[columnIndex];
                string value = Normalize(GetValue(cells, columnIndex));

                if (value != null) {
                    matchValues[header] = value;
                }
            }

            if (matchValues.Count == 0)
                throw new InvalidDataException(
                    "Row " + (lineIndex + 1) + ": At least one match value must be set.");

            rules.Add(new ClassificationRule(order, matchValues, classCode, TypeDescription));
            order++;
        }

        return rules;
    }

    private static int FindFirstNonEmptyLine(string[] lines) {
        for (int i = 0; i < lines.Length; i++) {
            if (!string.IsNullOrWhiteSpace(lines[i]))
                return i;
        }

        return -1;
    }

    private static string[] SplitAndTrim(string line) {
        string[] parts = line.Split(',');

        for (int i = 0; i < parts.Length; i++) {
            parts[i] = parts[i].Trim();
        }

        return parts;
    }

    private static int FindColumnIndex(string[] headers, string columnName, bool required) {
        for (int i = 0; i < headers.Length; i++) {
            if (string.Equals(headers[i], columnName, StringComparison.OrdinalIgnoreCase))
                return i;
        }

        if (required)
            throw new InvalidDataException("Missing required column: " + columnName);

        return -1;
    }

    private static string GetValue(string[] cells, int index) {
        if (index < 0 || index >= cells.Length)
            return null;

        return cells[index];
    }

    private static string Normalize(string value) {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        return value.Trim();
    }
}