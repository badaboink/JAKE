using System;
using System.IO;
using FlaUI.Core;
namespace UI_tests
{
    public class JakeClientHelper
    {
        public static Application LaunchJakeClient()
        {
            string jakeClientPath = GetJakeClientPath();
            var msApplication = Application.Launch(jakeClientPath);
            return msApplication;
        }
        private static string GetJakeClientPath()
        {
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string relativePathToClient = @"Client\bin\Debug\net6.0-windows\Jake.client.exe";

            while (!baseDirectory.EndsWith("JAKE", StringComparison.OrdinalIgnoreCase))
            {
                baseDirectory = Directory.GetParent(baseDirectory).FullName;
            }

            return Path.Combine(baseDirectory, relativePathToClient);
        }
    }
}