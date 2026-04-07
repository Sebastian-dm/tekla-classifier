using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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

        const string ConstantValue = ""; //"[L] Bygningsdel"
        const string ClassCodePrefix = ""; //"[L]%"
        const string TypeCodePrefix = ""; //""
        const string ClassTypeCodePrefix = ""; //"[L]%"
        const int RunningNumberDigits = 2;
        const int StartNumber = 1;
        const string ClassTypeSeparator = "_";


        public Classifier(string mappingFilePath, string databaseFilePath) {
            _mapping = CsvReader.ReadDictionaryFromCSV(mappingFilePath);
            _database = CsvReader.ReadDictionaryFromCSV(databaseFilePath);
            _databaseHeaders = CsvReader.ReadHeadersFromCSV(databaseFilePath);
        }
        

        public void Classify(List<Part> parts) {
            int i = 1;
            foreach (Part part in parts) {
                if (i % 50 == 0 || i == parts.Count())
                    Output.Status($"Classifying {i} of {parts.Count()} parts.");
                
                // Go to next part if there is already a class code
                string classCodeUDA = string.Empty;
                part.GetUserProperty(UDA_ClassCode, ref classCodeUDA);
                if (classCodeUDA == string.Empty)
                    continue;
                
                ClassifyPart(part);
                i++;
            }
            CsvReader.SortCsvFile(_databaseFilePath);
            Output.Status($"Finished classifying {parts.Count()} parts.");
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

        public void ClassifyPart(Part part) {

            string matchString = GenerateMatchString(part, udas: GetClassificationUdas(part));
            string classcode = GenerateClassCode(part);
            string typecode = GenerateTypeCode(classcode, matchString, typePrefix:GetPartName(part)); 
            string classTypecode = classcode + ClassTypeSeparator + typecode;
            string typeDescription = GenerateTypeDescription(part);

            // Set properties and update database
            if (UDA_Constant != "") part.SetUserProperty(UDA_Constant, ConstantValue);
            if (UDA_ClassCode != "") part.SetUserProperty(UDA_ClassCode, ClassCodePrefix + classcode);
            if (UDA_TypeCode != "") part.SetUserProperty(UDA_TypeCode, TypeCodePrefix + typecode);
            if (UDA_ClassTypeCode != "") part.SetUserProperty(UDA_ClassTypeCode, ClassTypeCodePrefix + classTypecode);
            if (UDA_TypeDescription != "") part.SetUserProperty(UDA_TypeDescription, typeDescription);

            // Update database
            if (!_database.ContainsKey(classTypecode) && classcode != "N/A") {
                var newEntry = new Dictionary<string, string> {
                        { "ClassTypeCode", classTypecode},
                        { "MatchString", matchString },
                        { "Description", typeDescription },
                    };
                _database.Add(classTypecode, newEntry);
                CsvReader.AddEntryToDatabaseFile(_databaseFilePath, classTypecode, _databaseHeaders, newEntry);
            }
        }

        private List<string> GetClassificationUdas(Part part) {
            if (!_mapping.ContainsKey(part.Name))
                return new List<string>();
            List<string> udas = new List<string>();
            foreach (var key in _mapping[part.Name].Keys) {
                if (key.StartsWith("UDA:"))
                    udas.Add(_mapping[part.Name][key].Replace("UDA:",""));
            }
            return udas;
        }

        private string GenerateMatchString(Part part, List<string> udas = null) {

            StringBuilder matchCode = new StringBuilder(GetPartName(part));

            foreach (string uda in udas) {
                string udaValue = string.Empty;
                part.GetUserProperty(uda, ref udaValue);

                if (udaValue != string.Empty)
                    matchCode.Append("_" + udaValue);
            }
            matchCode.Append("_" + GetPartProfile(part));

            return matchCode.ToString();
        }

        private string GenerateClassCode(Part part) {
            string partName = GetPartName(part);
            if (!_mapping.ContainsKey(partName)) {
                Output.Log("No mapping for name: " + partName);
                return "N/A";
            }
            return _mapping[partName]["ClassCode"];
        }

        private string GenerateTypeCode(string classCode, string matchString, string typePrefix = "") {
            int typeNumber = GenerateTypeNumber(classCode, matchString, typePrefix);
            return typePrefix + typeNumber.ToString("D" + RunningNumberDigits.ToString());
        }


        private int GenerateTypeNumber(string classCode, string matchString, string typePrefix = "") {

            // Check if database already contains Class
            var matchingDatabaseEntries = _database.Keys.Where(key => key.StartsWith(classCode));
            if (matchingDatabaseEntries.Count() == 0)
                return StartNumber;

            // Check if the matching Classes also match Type, and if so return the same number
            foreach (var entry in matchingDatabaseEntries) {
                if (_database[entry]["MatchString"] == matchString)
                        return int.Parse(entry.Substring(entry.Length - RunningNumberDigits));
            }

            // Create next number
            var lastMatchingClassCode = _database.Keys.Where(key => key.StartsWith(classCode)).OrderBy(key => key).Last();
            int lastMatchingNumber = int.Parse(lastMatchingClassCode.Substring(lastMatchingClassCode.Length - RunningNumberDigits));
            return lastMatchingNumber + 1;
        }


        private string GenerateTypeDescription(Part part) {
            string partName = GetPartName(part);
            string partProfile = GetPartProfile(part);

            if (!_mapping.ContainsKey(partName)) {
                Output.Log("No mapping for name: " + partName);
                return "";
            }
            if (_database.Values.Any(entry => entry["TeklaMatchString"] == partName + "_" + partProfile)) {
                var matchingEntry = _database.Values.Where(entry => entry["TeklaMatchString"] == partName + "_" + partProfile).First();
                return matchingEntry["Description"];
            }
            return _mapping[partName]["TypeDescription"] + " " + partProfile;
        }


        private bool UseThicknessAsProfile(Part part) {

            bool ProfileCanBeSplit = (part.Profile.ProfileString.Split('*').Length > 1);
            if (!ProfileCanBeSplit)
                return false;
            
            if ((part as Beam) != null && (part as Beam).Type == BeamTypeEnum.PANEL)
                    return true;
            if (part as PolyBeam != null && (part as PolyBeam).Type == PolyBeam.PolyBeamTypeEnum.PANEL)
                    return true;
            if ((part as ContourPlate) != null)
                    return true;
            return false;
        }


        private string GetPartName(Part part) {
            try {
                return part.Name;
            }
            catch (Exception) {
                Output.Log($"Failed to get name of part with guid: {part.Identifier.GUID}");
                return string.Empty;
            }
        }

        private string GetPartProfile(Part part) {
            // Extract part profile
            string profile = part.Profile.ProfileString;

            // Reduce if necessary
            if (UseThicknessAsProfile(part)) {
                string[] sizes = profile.Split('*');
                Array.Sort(sizes);
                return "t"+sizes[0];
            }
            return profile;
        }
    }
}
