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


    public interface IClassifier {
        void ClassifySelection();
        void ClassifyAll();
        void DeleteCCIUDAFromSelection();
        void DeleteCCIUDAFromAll();
    }


    internal class Classifier {

        const string UDA_ClassCode = "CCIClassCode";
        const string UDA_TypeName = "CCITypeName";
        const string UDA_TopNode = "CCITopNode";
        const string UDA_TypeID = "CCITypeID";

        private readonly Dictionary<string, List<string>> _database;
        private readonly Dictionary<string, List<string>> _mapping;

        const int RunningNumberDigits = 3;
        const int StartNumber = 1;

        private bool ignoreDuplicateClassWarning = false;

        public Classifier(string mappingFilePath, string databaseFilePath) {
            _database = ReadDictionaryFromCSV(Program.ClassificationForm.DatabasefilePath);
            _mapping = ReadDictionaryFromCSV(Program.ClassificationForm.MappingfilePath);
        }


        private Dictionary<string, List<string>> ReadDictionaryFromCSV(string filePath) {
            var dataDict = new Dictionary<string, List<string>>();
            using (StreamReader reader = new StreamReader(filePath)) {
                string line;
                while ((line = reader.ReadLine()) != null) {
                    string[] parts = line.Split(',','"').Select(s => s.Trim()).ToArray();
                    string key = parts[0];
                    var values = new List<string>();
                    for (int i = 1; i < parts.Length; i++)
                        values.Add(parts[i]);

                    if (dataDict.Keys.Contains(key)) {
                        if (ignoreDuplicateClassWarning) break;
                        var result = MessageBox.Show($"Database contains multiple definitions for class: {key}.\nThe first definition will be used and subsequent are ignored.\n\nClose with Cancel/Annuller to ignore similar messages.","Duplicate class definition",MessageBoxButtons.OKCancel);
                        if (result == DialogResult.Cancel) {
                            ignoreDuplicateClassWarning = true;
                        }
                    }
                    else
                        dataDict.Add(key, values);
                }
            }
            return dataDict;

        }

        public void Classify(List<Part> parts) {
            int i = 1;
            foreach (Part part in parts) {
                if (i % 50 == 0 || i == parts.Count())
                    Output.Status($"Classifying {i} of {parts.Count()} parts.");
                
                // Go to next part if there is already a class code
                string cciClassCodeUDA = string.Empty;
                part.GetUserProperty(UDA_ClassCode, ref cciClassCodeUDA);
                if (cciClassCodeUDA != string.Empty)
                    continue;
                
                Classify(part);
                i++;
            }
            Output.Status($"Finished classifying {parts.Count()} parts.");
        }


        public void DeleteClassificationUDAValues(List<Part> parts) {
            int i = 1;
            foreach (ModelObject part in parts) {
                if (i % 10 == 0)
                    Output.Status($"Removing CCI UDAs in {i} of {parts.Count()} parts.");
                part.SetUserProperty(UDA_ClassCode, "");
                part.SetUserProperty(UDA_TypeName, "");
                part.SetUserProperty(UDA_TopNode, "");
                part.SetUserProperty(UDA_TypeID, "");
                i++;
            }
            Output.Status($"Finished removing CCI UDAs on {parts.Count()} parts.");
        }

        private string GenerateDataBaseId(string partName, string partProfile) {
            return partName + "_" + partProfile;
        }

        private string GenerateClassificationClass(string partName) {
            if (!_mapping.ContainsKey(partName)) {
                Output.Log("No mapping for name: " + partName);
                return "N/A";
            }
            return _mapping[partName][1];
        }

        private int GenerateClassificationTypeNumber(string ClassificationClass, string dataBaseID) {

            // Check if database already contains CCICode
            var matchingCCIClasses = _database.Keys.Where(key => key.StartsWith(ClassificationClass));
            if (matchingCCIClasses.Count() == 0)
                return StartNumber;


            // Check if the matching CCIcodes also match databaseID
            foreach (var matchingCCIClass in matchingCCIClasses) {
                if (_database[matchingCCIClass][0] == dataBaseID)
                    return int.Parse(_database[matchingCCIClass][3]);
            }

            // Create next number
            var lastMatchingClassCode = _database.Keys.Where(key => key.StartsWith(ClassificationClass)).OrderBy(key => key).Last();
            int lastMatchingNumber = int.Parse(lastMatchingClassCode.Split('.').Last());
            return lastMatchingNumber + 1;
        }


        private string GenerateCCITypeName(string partName, string partProfile) {
            if (!_mapping.ContainsKey(partName)) {
                Output.Log("No mapping for name: " + partName);
                return "";
            }
            return _mapping[partName][0] + " " + partProfile;
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
                return sizes[0];
            }
            return profile;
        }


        public void Classify(Part part) {
            
            string name = GetPartName(part);
            string profile = GetPartProfile(part);

            // Create databaseID eg. BE_KB90/110
            string dataBaseID = GenerateDataBaseId(name, profile);

            // Create CCI type ID name eg. ULF.002
            string CCIClassCode = GenerateClassificationClass(name);
            string CCITypeNumber = GenerateClassificationTypeNumber(CCIClassCode, dataBaseID).ToString("D" + RunningNumberDigits.ToString());
            string CCITypeID = CCIClassCode + "." + CCITypeNumber;

            string TypeName = GenerateCCITypeName(name, profile);

            // Set properties and update database
            part.SetUserProperty(UDA_ClassCode, "[L]%" + CCIClassCode);
            part.SetUserProperty(UDA_TopNode, "[L] Bygningsdel");
            part.SetUserProperty(UDA_TypeID, "[L]%"+CCITypeID);
            part.SetUserProperty(UDA_TypeName, TypeName);

            // Update database
            if (!_database.ContainsKey(CCITypeID) && CCIClassCode != "N/A") {
                _database.Add(CCITypeID, new List<string>() { dataBaseID, name, profile, CCITypeNumber });
                AddLineToDatabaseFile($"{CCITypeID}, {dataBaseID}, {name}, {profile}, {CCITypeNumber}");
            }
        }

        private void AddLineToDatabaseFile(string line) {
            using (StreamWriter writer = new StreamWriter(Program.ClassificationForm.DatabasefilePath, true)) {
                writer.WriteLine(line);
            }
        }

        private Hashtable GetReportProperties(ModelObject mo) {
            ArrayList sNames = new ArrayList() {
                "NAME", "PROFILE", "OBJECT_TYPE", "MATERIAL_TYPE",
                "PROFILE_TYPE", "TYPENAME", "CONTENTTYPE",
                "TYPE", "TYPE1", "TYPE2", "TYPE3", "TYPE4",
            };
            ArrayList dNames = new ArrayList() {
                "SUBTYPE"
            };
            ArrayList iNames = new ArrayList() {
                "TYPENUMBER", "GROUP_TYPE", "DATE"
            };
            Hashtable values = new Hashtable(sNames.Count + dNames.Count + iNames.Count);
            mo.GetAllReportProperties(sNames, dNames, iNames, ref values);
            
            return values;
        }
    }
}
