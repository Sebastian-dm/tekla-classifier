using RenderData;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Reflection;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Windows.Forms;
using Tekla.Structures.Model;
using Tekla.Structures.ModelInternal;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ExplorerBar;
using static Tekla.Structures.Filtering.Categories.ComponentFilterExpressions;
using static Tekla.Structures.Filtering.Categories.PartFilterExpressions;
using static Tekla.Structures.Model.Beam;


namespace TeklaClassifier {


    public interface IClassifier {
        void ClassifySelection();
        void ClassifyAll();
        void DeleteCCIUDAFromSelection();
        void DeleteCCIUDAFromAll();
    }


    internal class Classifier {

        const string UDA_Constant = ""; //""
        const string UDA_ClassCode = "DDA-lag"; //"DDA-lag"
        const string UDA_TypeCode = "DDA-id"; //"DDA-id"
        const string UDA_TypeDescription = "DDA-desc"; //"DDA-desc"
        const string UDA_ClassTypeCode = ""; //"DDA-desc"

        private readonly string _databaseFilePath;
        private readonly Dictionary<string, Dictionary<string, string>> _mapping;
        private readonly Dictionary<string, Dictionary<string, string>> _database;
        private readonly List<string> _databaseHeaders;

        IList<ClassificationRule> _rules;

        const string ConstantValue = ""; //"[L] Bygningsdel"
        const string ClassCodePrefix = ""; //"[L]%"
        const string TypeCodePrefix = ""; //""
        const string ClassTypeCodePrefix = ""; //"[L]%"
        const int RunningNumberDigits = 2;
        const int StartNumber = 1;
        const string ClassTypeSeparator = "_";


        public Classifier(string mappingFilePath, string databaseFilePath) {
            _databaseFilePath = databaseFilePath;
            _mapping = CsvReader.ReadDictionaryFromCSV(mappingFilePath);
            _database = CsvReader.ReadDictionaryFromCSV(databaseFilePath);
            _databaseHeaders = CsvReader.ReadHeadersFromCSV(databaseFilePath);

            _rules = ClassificationRuleLoader.LoadRules(mappingFilePath);
        }


        public void DeleteClassificationUDAValues(List<Part> parts) {
            int i = 1;
            foreach (ModelObject part in parts) {
                if (i % 10 == 0)
                    Output.Status($"Removing classification UDAs in {i} of {parts.Count()} parts.");
                part.SetUserProperty(UDA_Constant, "");
                part.SetUserProperty(UDA_ClassCode, "");
                part.SetUserProperty(UDA_TypeCode, "");
                part.SetUserProperty(UDA_TypeDescription, "");
                part.SetUserProperty(UDA_ClassTypeCode, "");
                i++;
            }
            Output.Status($"Finished removing classification UDAs on {parts.Count()} parts.");
        }
        

        public void Classify(List<Part> parts) {
            int i = 1;
            foreach (Part part in parts) {
                if (i % 50 == 0 || i == parts.Count())
                    Output.Status($"Classifying {i} of {parts.Count()} parts.");

                // Parts already has classification data, so skip
                string classCodeUDA = string.Empty;
                part.GetUserProperty(UDA_ClassCode, ref classCodeUDA);
                if (classCodeUDA != string.Empty)
                    continue;

                var partData = BuildPartData(part);

                if (_database.ContainsKey(partData["DatabaseKey"]))
                    partData = GetClassificationFromDatabase(partData);
                else 
                    partData = GenerateClassificationFromMapping(partData);

                SetPartClassificationUdas(part, partData);
                UpdateDatabase(partData);

                i++;
            }
            CsvReader.SortCsvFile(_databaseFilePath);
            Output.Status($"Finished classifying {parts.Count()} parts.");
        }


        public Dictionary<string, string> BuildPartData(Part part) {
            string partName = part.Name;
            string partProfile = GetPartProfile(part);

            string databaseKey = partName;
            var partData = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) {
                { "Name", partName },
                { "Profile", partProfile }
            };
            foreach (var uda in GetClassificationUdas(part)) {
                partData.Add(uda.Key, uda.Value);
                databaseKey += "_" + uda.Value;
            }
            databaseKey += "_" + partProfile;

            partData.Add("DatabaseKey", databaseKey);

            return partData;
        }


        public Dictionary<string, string> GetClassificationFromDatabase(Dictionary<string, string> partData) {
            var databaseEntry = _database[partData["DatabaseKey"]];
            partData["ClassCode"] = databaseEntry["ClassCode"];
            partData["TypeCode"] = databaseEntry["TypeCode"];
            partData["ClassTypeCode"] = partData["ClassCode"] + ClassTypeSeparator + partData["TypeCode"];
            partData["TypeDescription"] = databaseEntry["TypeDescription"];
            return partData;
        }


        public Dictionary<string, string> GenerateClassificationFromMapping(Dictionary<string, string> partData) {
            ClassificationRule match = null;
            for (int i = 0; i < _rules.Count; i++) {
                if (_rules[i].Matches(partData)) {
                    match = _rules[i];
                    break;
                }
            }
            if (match != null) {
                partData["ClassCode"] = match.ClassCode;
                partData["TypeDescription"] = match.TypeDescription + " " + partData["Profile"];
                partData["TypeCode"] = partData["Name"] + GenerateTypeNumber(partData).ToString("D" + RunningNumberDigits);
                partData["ClassTypeCode"] = partData["ClassCode"] + ClassTypeSeparator + partData["TypeCode"];
            }
            return partData;
        }


        public void SetPartClassificationUdas(Part part, Dictionary<string, string> partData) {
            if (UDA_Constant != "") part.SetUserProperty(UDA_Constant, ConstantValue);
            if (UDA_ClassCode != "") part.SetUserProperty(UDA_ClassCode, ClassCodePrefix + partData["ClassCode"]);
            if (UDA_TypeCode != "") part.SetUserProperty(UDA_TypeCode, TypeCodePrefix + partData["TypeCode"]);
            if (UDA_ClassTypeCode != "") part.SetUserProperty(UDA_ClassTypeCode, ClassTypeCodePrefix + partData["ClassTypeCode"]);
            if (UDA_TypeDescription != "") part.SetUserProperty(UDA_TypeDescription, partData["TypeDescription"]);
        }


        public void UpdateDatabase(Dictionary<string, string> partData) {
            if (!_database.ContainsKey(partData["DatabaseKey"]) && partData["ClassCode"] != "N/A") {
                var newEntry = new Dictionary<string, string> {
                                { "ClassCode", partData["ClassCode"]},
                                { "TypeCode", partData["TypeCode"] },
                                { "TypeDescription", partData["TypeDescription"] },
                            };
                _database.Add(partData["DatabaseKey"], newEntry);
                CsvReader.AddEntryToDatabaseFile(_databaseFilePath, partData["DatabaseKey"], _databaseHeaders, newEntry);
            }
        }


        private Dictionary<string, string> GetClassificationUdas(Part part) {
            var udas = new Dictionary<string, string>();

            if (!_mapping.ContainsKey(part.Name))
                return udas;
            foreach (var key in _mapping[part.Name].Keys) {
                if (key.StartsWith("UDA:")) {
                    string udaKey = key.Replace("UDA:", "");
                    string udaValue = string.Empty;
                    part.GetUserProperty(udaKey, ref udaValue);
                    udas.Add(key, udaValue);
                }
            }
            return udas;
        }


        private int GenerateTypeNumber(Dictionary<string, string> partData) {

            // Check if database already contains Class
            var databaseEntriesMatchingClass = _database.Where(kvp => kvp.Value["ClassCode"] == partData["ClassCode"]).ToList();
            if (databaseEntriesMatchingClass.Count() == 0)
                return StartNumber;

            // Create next number
            var lastMatchingClassCode = databaseEntriesMatchingClass.OrderBy(kvp => kvp.Value["TypeCode"]).Last().Value["TypeCode"];
            int lastMatchingNumber = int.Parse(lastMatchingClassCode.Substring(lastMatchingClassCode.Length - RunningNumberDigits));
            return lastMatchingNumber + 1;
        }


        private string GetPartProfile(Part part) {
            // Extract part profile
            string profile = part.Profile.ProfileString;

            // Case: Only a single number, e.g. "300"
            int intProfile = 0;
            int.TryParse(profile, out intProfile);
            if (intProfile != 0)
                return "t" + intProfile.ToString();

            // Reduce if necessary
            if (UseThicknessAsProfile(part)) {
                string[] sizes = profile.Split('*').OrderBy(s => int.Parse(s)).ToArray();
                return "t" + sizes[0];
            }
            return profile;
        }

        private bool UseThicknessAsProfile(Part part) {
            List<string> profileExceptions = new List<string>() { "VI", "VE"};

            bool ProfileCanBeSplit = (part.Profile.ProfileString.Split('*').Length > 1);
            if (!ProfileCanBeSplit)
                return false;

            if ((part as Beam) != null && (part as Beam).Type == BeamTypeEnum.PANEL)
                return true;
            if (part as PolyBeam != null && (part as PolyBeam).Type == PolyBeam.PolyBeamTypeEnum.PANEL)
                return true;
            if ((part as ContourPlate) != null)
                return true;
            if (profileExceptions.Contains(part.Name))
                return true;
            return false;
        }
    }
}
