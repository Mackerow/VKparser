using System;

namespace OperatingSystemsPractices.Source.Resources
{
    public static class Folders
    {
        public static string ChromeUserData { get { return Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Google\Chrome\User Data"; } }
        public static string ProgramDocumentsOld { get { return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\OperatingSystemsPractices"; } }
        public static string ProgramDocuments { get { return @"C:\Users\vladk\Documents\OperatingSystemsPractices"; } }
    }
}
