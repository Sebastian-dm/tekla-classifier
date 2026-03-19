using Tekla.Structures;
using System.IO;
using System.Diagnostics;
using System.Linq;

namespace Tekla.Technology.Akit.UserScript
{
    public class Script
    {
        static string exePath = @"__EXE_PATH__";
        
        public static void Run(IScript akit)
        {
            LaunchExternalApplication.StartExtension(exePath);
        }
    }

    public static class LaunchExternalApplication
    {
        public static void StartExtension(string filePath)
        {
            var externalApplication = new Process { StartInfo = { FileName = filePath } };
            if (externalApplication.Start()) Tekla.Structures.Model.Operations.Operation.DisplayPrompt(filePath + " successfully started.");
            else Tekla.Structures.Model.Operations.Operation.DisplayPrompt(filePath + " unable to be started.");
        }
    }
}