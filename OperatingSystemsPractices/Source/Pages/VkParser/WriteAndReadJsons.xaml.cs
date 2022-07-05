using OperatingSystemsPractices.Source.Resources;
using OperatingSystemsPractices.Source.Threads;
using OperatingSystemsPractices.Source.Vk;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.IO.MemoryMappedFiles;
using OperatingSystemsPractices.Source.Multiprocessing;

namespace OperatingSystemsPractices.Source.Pages.VkParser
{ 
    public partial class WriteAndReadJsons : Page
    {
        byte demonstrationSpeedRate = 128;
        Random random = new Random();

        List<Post> posts;
        List<PostIdText> postsIdText;
        List<PostIdPhotos> postsIdPhotos;
        List<PostIdHrefs> postsIdHrefs;

        bool isWorking = true;

        Iteration[] iterations;

        ThreadWithEvents postsIdTextThread;
        ThreadWithEvents postsIdPhotosThread;
        ThreadWithEvents postsIdHrefsThread;
        ThreadWithEvents postsReadAllFilesThread;

        MemoryMappedFile sharedMemory;
        MemoryMappedFile processWorking;

        public static string OtherProcessOwnerName { get; set; } = "Process2";
        public static string CurrentProcessOwnerName { get; set; } = "OperatingSystemsPractices";
        public static int StartIterationIndex { get; set; } = 0;

        public WriteAndReadJsons(List<Post> posts)
        {
            InitializeComponent();

            DemonstrationSpeedRate.Value = demonstrationSpeedRate;

            this.posts = posts;
            postsIdText = PostIdText.Create(posts);
            postsIdPhotos = PostIdPhotos.Create(posts);
            postsIdHrefs = PostIdHrefs.Create(posts);

            postsIdTextThread = new ThreadWithEvents(new Thread(PostIdTextThread) { IsBackground = true });
            postsIdPhotosThread = new ThreadWithEvents(new Thread(PostIdPhotosThread) { IsBackground = true });
            postsIdHrefsThread = new ThreadWithEvents(new Thread(PostIdHrefsThread) { IsBackground = true });
            postsReadAllFilesThread = new ThreadWithEvents(new Thread(ReadAllFilesThread) { IsBackground = true });

            Thread thread = new Thread(() => Start()) { IsBackground = true };
            thread.Start();
        }

        public void Start()
        {
            DirectoryInfo dirInfo = new DirectoryInfo(Folders.ProgramDocuments);
            if (!dirInfo.Exists) dirInfo.Create();

            Post.CreateJsonFile(Files.VkNews.Path, posts);

            iterations = new Iteration[] {
                new Iteration(CurrentProcessOwnerName, postsIdTextThread, postsIdPhotosThread, postsIdHrefsThread),
                new Iteration(OtherProcessOwnerName),
                new Iteration(CurrentProcessOwnerName, postsReadAllFilesThread, postsIdPhotosThread, postsIdHrefsThread),
                new Iteration(OtherProcessOwnerName),
                new Iteration(CurrentProcessOwnerName, postsIdTextThread, postsReadAllFilesThread, postsIdHrefsThread),
                new Iteration(OtherProcessOwnerName),
                new Iteration(CurrentProcessOwnerName, postsIdTextThread, postsIdPhotosThread, postsReadAllFilesThread)
            };

            postsIdTextThread.GetThread().Start();
            postsIdPhotosThread.GetThread().Start();
            postsIdHrefsThread.GetThread().Start();
            postsReadAllFilesThread.GetThread().Start();

            SharedMemory.CreateOrOpen(SharedMemory.Name, sizeof(int), out sharedMemory);
            SharedMemory.CreateOrOpen(@"Global\" + CurrentProcessOwnerName, sizeof(bool), out processWorking);
            SharedMemory.Write(processWorking, 0, true);

            Log.ProcessStarted();

            int iterationIndex;
            while (true)
            {
                if (!Process.IsRunningAndReady(OtherProcessOwnerName)) { iterationIndex = StartIterationIndex; Iteration.SetIteration(iterationIndex); break; }
                if (Iteration.TryGetIteration(out iterationIndex))
                {
                    if (iterations[iterationIndex].ProcessOwnerName != CurrentProcessOwnerName) continue;
                    if (iterationIndex == StartIterationIndex) break;
                    else Iteration.SetIteration(Iteration.GetNextIterationIndex(iterations, iterationIndex));
                }
                else Log.UnableToLocateStart();
            }

            Log.StartLocated(iterationIndex);

            for (; isWorking; iterationIndex++)
            {
                if (iterationIndex == iterations.Length) iterationIndex = 0;

                if (iterations[iterationIndex].ProcessOwnerName != CurrentProcessOwnerName) continue;

                Log.Wait(iterationIndex);
                Iteration.WaitForIteration(iterations, iterationIndex);

                Log.Work(iterationIndex);
                Iteration.SetIteration(iterationIndex);
                SetIteration(iterationIndex.ToString());
                StartIteration(iterations[iterationIndex].ThreadsWithEvents);
                Thread.Sleep(500);

                int setIterationIndex = Iteration.GetNextIterationIndex(iterations, iterationIndex);
                Log.SetIteration(setIterationIndex);
                Iteration.SetIteration(setIterationIndex);

                //Log.AddLine("");
            }

            Log.WaitingForThreadsToStop();

            postsIdTextThread.Stop();
            postsIdPhotosThread.Stop();
            postsIdHrefsThread.Stop();
            postsReadAllFilesThread.Stop();

            postsIdTextThread.GetThread().Join();
            postsIdPhotosThread.GetThread().Join();
            postsIdHrefsThread.GetThread().Join();
            postsReadAllFilesThread.GetThread().Join();

            Log.ProcessFinished();
            SharedMemory.Write(processWorking, 0, false);
            processWorking.Dispose();
            Dispatcher.Invoke(() => Application.Current.Shutdown());
        }

        private void StartIteration(ThreadWithEvents[] threadsWithEvents)
        {
            if (!isWorking) return;
            foreach (ThreadWithEvents threadWithEvent in threadsWithEvents) threadWithEvent.StartEvent.Set();
            foreach (ThreadWithEvents threadWithEvent in threadsWithEvents) threadWithEvent.FinishEvent.WaitOne();
            Thread.Sleep(demonstrationSpeedRate * 4); // Just for better visibility that all threads have been completed
            SetAllStandart();
        }

        private void PostIdTextThread()
        {
            while (postsIdTextThread.IsRunning)
            {
                postsIdTextThread.StartEvent.WaitOne();
                if (!postsIdTextThread.IsRunning) return;

                SetWorking(PostIdTextThreadTextBlock);

                PostIdText.CreateJsonFile(Files.Text.Path, postsIdText);
                Thread.Sleep(demonstrationSpeedRate * random.Next(1, 8) * 4); // Just for better visibility that threads are waiting for each other

                SetFinished(PostIdTextThreadTextBlock);

                postsIdTextThread.FinishEvent.Set();
            }
        }

        private void PostIdPhotosThread()
        {
            while (postsIdPhotosThread.IsRunning)
            {
                postsIdPhotosThread.StartEvent.WaitOne();
                if (!postsIdPhotosThread.IsRunning) return;

                SetWorking(PostIdPhotosThreadTextBlock);

                PostIdPhotos.CreateJsonFile(Files.Photos.Path, postsIdPhotos);
                Thread.Sleep(demonstrationSpeedRate * random.Next(1, 8) * 4); // Just for better visibility that threads are waiting for each other

                SetFinished(PostIdPhotosThreadTextBlock);

                postsIdPhotosThread.FinishEvent.Set();
            }
        }

        private void PostIdHrefsThread()
        {
            while (postsIdHrefsThread.IsRunning)
            {
                postsIdHrefsThread.StartEvent.WaitOne();
                if (!postsIdHrefsThread.IsRunning) return;

                SetWorking(PostIdHrefsThreadTextBlock);

                PostIdHrefs.CreateJsonFile(Files.Hrefs.Path, postsIdHrefs);
                Thread.Sleep(demonstrationSpeedRate * random.Next(1, 8) * 4); // Just for better visibility that threads are waiting for each other

                SetFinished(PostIdHrefsThreadTextBlock);

                postsIdHrefsThread.FinishEvent.Set();
            }
        }

        private void ReadAllFilesThread()
        {
            for (int iteration = 0; postsReadAllFilesThread.IsRunning; iteration = (iteration + 1) % 3)
            {
                postsReadAllFilesThread.StartEvent.WaitOne();
                if (!postsReadAllFilesThread.IsRunning) return;

                switch (iteration)
                {
                    case 0:
                        {
                            SetWorking(ReadAllFilesThreadTextBlock, $"working[{Files.Text.Name}]...");
                            PostIdText.ReadFromJsonFile(Files.Text.Path);
                            break;
                        }
                    case 1:
                        {
                            SetWorking(ReadAllFilesThreadTextBlock, $"working[{Files.Photos.Name}]...");
                            PostIdPhotos.ReadFromJsonFile(Files.Photos.Path);
                            break;
                        }
                    case 2:
                        {
                            SetWorking(ReadAllFilesThreadTextBlock, $"working[{Files.Hrefs.Name}]...");
                            PostIdHrefs.ReadFromJsonFile(Files.Hrefs.Path);
                            break;
                        }
                }
                Thread.Sleep(demonstrationSpeedRate * random.Next(1, 8) * 4); // Just for better visibility that threads are waiting for each other

                SetFinished(ReadAllFilesThreadTextBlock);

                postsReadAllFilesThread.FinishEvent.Set();
            }
        }

        private void SetAllStandart()
        {
            SetWaiting(PostIdTextThreadTextBlock);
            SetWaiting(PostIdPhotosThreadTextBlock);
            SetWaiting(PostIdHrefsThreadTextBlock);
            SetWaiting(ReadAllFilesThreadTextBlock);
        }

        private void SetIteration(string s)
        {
            IterationNumberTextBlock.Dispatcher.Invoke(() => IterationNumberTextBlock.Text = s);
        }

        private void SetWaiting(TextBlock textBlock)
        {
            textBlock.Dispatcher.Invoke(() =>
            {
                textBlock.Text = "waiting...";
                textBlock.Foreground = System.Windows.Media.Brushes.Black;
            });
        }

        private void SetWorking(TextBlock textBlock, string text = "working...")
        {
            textBlock.Dispatcher.Invoke(() =>
            {
                textBlock.Text = text;
                textBlock.Foreground = System.Windows.Media.Brushes.Red;
            });
        }

        private void SetFinished(TextBlock textBlock)
        {
            textBlock.Dispatcher.Invoke(() =>
            {
                textBlock.Text = "finished!";
                textBlock.Foreground = System.Windows.Media.Brushes.Green;
            });
        }

        private void NextStep()
        {
            Dispatcher.InvokeAsync(() => NavigationService.Navigate(new FinalPage()));
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            StopButton.IsEnabled = false;
            StopButton.Content = "Stopping...";

            isWorking = false;
            postsIdTextThread.Stop();
            postsIdPhotosThread.Stop();
            postsIdHrefsThread.Stop();
            postsReadAllFilesThread.Stop();
        }

        private void DemonstrationSpeedRate_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            demonstrationSpeedRate = (byte)(byte.MaxValue - (byte)e.NewValue);
        }
    }
}
