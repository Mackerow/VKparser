using System;
using OperatingSystemsPractices.Source.Vk;

namespace OperatingSystemsPractices.Source.Resources
{
    public static class Files
    {
        public static FileInfo VkNews { get { return new FileInfo(Folders.ProgramDocuments, "VkNews", "json"); } }
        public static FileInfo Text { get { return new FileInfo(Folders.ProgramDocuments, "1", "json"); } }
        public static FileInfo Photos { get { return new FileInfo(Folders.ProgramDocuments, "2", "json"); } }
        public static FileInfo Hrefs { get { return new FileInfo(Folders.ProgramDocuments, "3", "json"); } }
    }
}
