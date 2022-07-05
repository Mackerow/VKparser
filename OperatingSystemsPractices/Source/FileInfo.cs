namespace OperatingSystemsPractices.Source
{
    public class FileInfo
    {
        public string Folder { get; private set; }
        public string Name { get; private set; }
        public string Format { get; private set; }
        public string Path { get { return Folder + @"\" + Name + "." + Format; } }
        public FileInfo(string folder, string name, string format)
        {
            Folder = folder;
            Name = name;
            Format = format;
        }
    }
}
