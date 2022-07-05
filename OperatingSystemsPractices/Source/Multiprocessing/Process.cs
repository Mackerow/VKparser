using System.Linq;
using System.ServiceProcess;

namespace OperatingSystemsPractices.Source.Multiprocessing
{
    public static class Process
    {
        public static bool IsRunningAndReady(string name)
        {
            if (!ProcessIsRunning(name)) return false;
            if (!ProcessIsReady(name)) return false;
            return true;
        }

        private static bool ProcessIsRunning(string name)
        {
            try { if (System.Diagnostics.Process.GetProcessesByName(name).Count() != 0) return true; } catch { }
            try { if (new ServiceController(name).Status == ServiceControllerStatus.Running) return true; } catch { }
            try { if (new ServiceController(name).Status == ServiceControllerStatus.StopPending) return true; } catch { }
            return false;
        }

        private static bool ProcessIsReady(string name)
        {
            if (!SharedMemory.OpenExisting(@"Global\" + name, out var memoryMappedFile)) return false;
            if (!SharedMemory.Read(memoryMappedFile, 0, out bool isRunning)) return false;
            return isRunning;
        }
    }
}
