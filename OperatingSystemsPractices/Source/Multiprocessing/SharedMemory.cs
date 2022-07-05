using System.IO;
using System.IO.MemoryMappedFiles;
using System.Security.AccessControl;
using System.Security.Principal;

namespace OperatingSystemsPractices.Source.Multiprocessing
{
    public static class SharedMemory
    {
        static public string Name { get; private set; } = @"Global\SharedMemory";
        static public MemoryMappedFileSecurity Security
        {
            get
            {
                MemoryMappedFileSecurity security = new MemoryMappedFileSecurity();
                security.AddAccessRule(new AccessRule<MemoryMappedFileRights>(new SecurityIdentifier(WellKnownSidType.WorldSid, null), MemoryMappedFileRights.FullControl, AccessControlType.Allow));
                return security;
            }
        }

        public static bool CreateOrOpen(string name, int capacity, out MemoryMappedFile memoryMappedFile)
        {
            try { memoryMappedFile = MemoryMappedFile.CreateOrOpen(name, capacity, MemoryMappedFileAccess.ReadWrite, MemoryMappedFileOptions.None, Security, HandleInheritability.Inheritable); return true; }
            catch { memoryMappedFile = null; return false; }
        }

        public static bool OpenExisting(string name, out MemoryMappedFile memoryMappedFile)
        {
            try { memoryMappedFile = MemoryMappedFile.OpenExisting(name); return true; }
            catch { memoryMappedFile = null; return false; }
        }

        public static bool Write(MemoryMappedFile memoryMappedFile, int offset, int value)
        {
            try
            {
                using (MemoryMappedViewAccessor writer = memoryMappedFile.CreateViewAccessor(offset, sizeof(int)))
                {
                    writer.Write(0, value);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool Write(MemoryMappedFile memoryMappedFile, int offset, bool value)
        {
            try
            {
                using (MemoryMappedViewAccessor writer = memoryMappedFile.CreateViewAccessor(offset, sizeof(bool)))
                {
                    writer.Write(0, value);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool Read(MemoryMappedFile memoryMappedFile, int offset, out int value)
        {
            try
            {
                using (MemoryMappedViewAccessor writer = memoryMappedFile.CreateViewAccessor(offset, sizeof(int)))
                {
                    value = writer.ReadInt32(0);
                }
                return true;
            }
            catch
            {
                value = 0;
                return false;
            }
        }

        public static bool Read(MemoryMappedFile memoryMappedFile, int offset, out bool value)
        {
            try
            {
                using (MemoryMappedViewAccessor writer = memoryMappedFile.CreateViewAccessor(offset, sizeof(int)))
                {
                    value = writer.ReadBoolean(0);
                }
                return true;
            }
            catch
            {
                value = false;
                return false;
            }
        }
    }
}
