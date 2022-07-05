using OperatingSystemsPractices.Source.Threads;
using OperatingSystemsPractices.Source.Pages.VkParser;

namespace OperatingSystemsPractices.Source.Multiprocessing
{
    public class Iteration
    {
        public string ProcessOwnerName { get; private set; }
        public ThreadWithEvents[] ThreadsWithEvents { get; private set; }
        public Iteration(string processOwnerName, params ThreadWithEvents[] threadsWithEvents)
        {
            ProcessOwnerName = processOwnerName;
            ThreadsWithEvents = threadsWithEvents;
        }
        public static int GetNextIterationIndex(Iteration[] iterations, int currentIterationIndex)
        {
            for (int iterationIndex = currentIterationIndex + 1; iterationIndex < iterations.Length; iterationIndex++)
            {
                if (Process.IsRunningAndReady(iterations[iterationIndex].ProcessOwnerName))
                    return iterationIndex;
            }
            for (int iterationIndex = 0; iterationIndex < currentIterationIndex; iterationIndex++)
            {
                if (Process.IsRunningAndReady(iterations[iterationIndex].ProcessOwnerName))
                    return iterationIndex;
            }
            return currentIterationIndex;
        }

        public static void WaitForIteration(Iteration[] iterations, int iterationIndex)
        {
            while (true)
            {
                if (TryGetIteration(out var iterationIndexInSharedMemory))
                {
                    if (iterationIndexInSharedMemory == iterationIndex) break;
                }
                if (!Process.IsRunningAndReady(WriteAndReadJsons.OtherProcessOwnerName))
                {
                    Log.Skip(iterationIndex == 0 ? iterations.Length : iterationIndex);
                    break;
                }
            }
        }

        public static void SetIteration(int iterationIndex)
        {
            while (true)
            {
                if (!SharedMemory.CreateOrOpen(SharedMemory.Name, sizeof(int), out var memoryMappedFile)) continue;
                if (!SharedMemory.Write(memoryMappedFile, 0, iterationIndex)) continue;
                break;
            }
        }

        public static bool TryGetIteration(out int res)
        {
            if (!SharedMemory.OpenExisting(SharedMemory.Name, out var memoryMappedFile)) { res = 0; return false; }
            if (!SharedMemory.Read(memoryMappedFile, 0, out res)) { res = 0; return false; }
            return true;
        }
    }
}
