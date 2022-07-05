using OperatingSystemsPractices.Source.Resources;

namespace OperatingSystemsPractices.Source
{
    public static class Log
    {
        static string Name { get; set; } = "Process1";
        static string Path { get; set; } = Folders.ProgramDocuments + @"\Log.txt";
        static bool AllowWrite { get; set; } = true;
        static bool WriteProcessStarted { get; set; } = true;
        static bool WriteUnableToLocateStart { get; set; } = false;
        static bool WriteStartLocated { get; set; } = false;
        static bool WriteSkip { get; set; } = false;
        static bool WriteWait { get; set; } = false;
        static bool WriteWork { get; set; } = true;
        static bool WriteSetIteration { get; set; } = false;
        static bool WriteWaitingForThreadsToStop { get; set; } = false;
        static bool WriteProcessFinished { get; set; } = true;

        public static void AddLine(string s)
        {
            if (!AllowWrite) return;
            while (true)
            {
                try { System.IO.File.AppendAllText(Path, s + "\n"); }
                catch { continue; };
                break;
            }
        }

        public static void ProcessStarted()
        {
            if (!WriteProcessStarted) return;
            Log.AddLine($"{Name}: started!");
        }

        public static void UnableToLocateStart()
        {
            if (!WriteUnableToLocateStart) return;
            Log.AddLine($"{Name}: unable to locate start...");
        }

        public static void StartLocated(int iterationIndex)
        {
            if (!WriteStartLocated) return;
            Log.AddLine($"{Name}: start located [{iterationIndex}]");
        }

        public static void Skip(int iterationIndex)
        {
            if (!WriteSkip) return;
            Log.AddLine($"Process1: skip [{iterationIndex}], cause process is not active!");
        }

        public static void Wait(int iterationIndex)
        {
            if (!WriteWait) return;
            Log.AddLine($"{Name}: wait [{iterationIndex}]");
        }

        public static void Work(int iterationIndex)
        {
            if (!WriteWork) return;
            Log.AddLine($"{Name}: work [{iterationIndex}]");
        }

        public static void SetIteration(int iterationIndex)
        {
            if (!WriteSetIteration) return;
            Log.AddLine($"{Name}: set [{iterationIndex}]");
        }

        public static void WaitingForThreadsToStop()
        {
            if (!WriteWaitingForThreadsToStop) return;
            Log.AddLine($"{Name}: waiting for threads to stop...");
        }

        public static void ProcessFinished()
        {
            if (!WriteProcessFinished) return;
            Log.AddLine($"{Name}: finished!");
        }
    }
}
