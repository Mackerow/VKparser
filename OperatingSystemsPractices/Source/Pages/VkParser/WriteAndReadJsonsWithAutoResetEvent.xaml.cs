using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using OperatingSystemsPractices.Source.Threads;
using OperatingSystemsPractices.Source.Vk;
using OperatingSystemsPractices.Source.Resources;

namespace OperatingSystemsPractices.Source.Pages.VkParser
{
    public partial class WriteAndReadJsonsWithAutoResetEvent : Page
    {
        byte demonstrationSpeedRate = 128;
        Random random = new Random();

        List<Post> posts;
        List<PostIdText> postsIdText;
        List<PostIdPhotos> postsIdPhotos;
        List<PostIdHrefs> postsIdHrefs;

        bool isWorking = true;

        ThreadWithEvents postsIdTextThreadEvents;
        ThreadWithEvents postsIdPhotosThreadEvents;
        ThreadWithEvents postsIdHrefsThreadEvents;
        ThreadWithEvents postsReadAllFilesThreadEvents;

        public WriteAndReadJsonsWithAutoResetEvent(List<Post> posts)
        {
            InitializeComponent();

            DemonstrationSpeedRate.Value = demonstrationSpeedRate;

            this.posts = posts;
            postsIdText = PostIdText.Create(posts);
            postsIdPhotos = PostIdPhotos.Create(posts);
            postsIdHrefs = PostIdHrefs.Create(posts);

            postsIdTextThreadEvents = new ThreadWithEvents(new Thread(PostIdTextThread) { IsBackground = true });
            postsIdPhotosThreadEvents = new ThreadWithEvents(new Thread(PostIdPhotosThread) { IsBackground = true });
            postsIdHrefsThreadEvents = new ThreadWithEvents(new Thread(PostIdHrefsThread) { IsBackground = true });
            postsReadAllFilesThreadEvents = new ThreadWithEvents(new Thread(ReadAllFilesThread) { IsBackground = true });

            Thread thread = new Thread(() => Start()) { IsBackground = true };
            thread.Start();
        }

        private void Start()
        {
            DirectoryInfo dirInfo = new DirectoryInfo(Folders.ProgramDocuments);
            if (!dirInfo.Exists) dirInfo.Create();

            if (File.Exists(Files.Text.Path)) File.Delete(Files.Text.Path);
            if (File.Exists(Files.Photos.Path)) File.Delete(Files.Photos.Path);
            if (File.Exists(Files.Hrefs.Path)) File.Delete(Files.Hrefs.Path);

            Post.CreateJsonFile(Files.VkNews.Path, posts);

            postsIdTextThreadEvents.GetThread().Start();
            postsIdPhotosThreadEvents.GetThread().Start();
            postsIdHrefsThreadEvents.GetThread().Start();
            postsReadAllFilesThreadEvents.GetThread().Start();

            while (isWorking)
            {
                SetIteration("1");
                Iteration(postsIdTextThreadEvents, postsIdPhotosThreadEvents, postsIdHrefsThreadEvents);

                SetIteration("2");
                Iteration(postsReadAllFilesThreadEvents, postsIdPhotosThreadEvents, postsIdHrefsThreadEvents);

                SetIteration("3");
                Iteration(postsIdTextThreadEvents, postsReadAllFilesThreadEvents, postsIdHrefsThreadEvents);

                SetIteration("4");
                Iteration(postsIdTextThreadEvents, postsIdPhotosThreadEvents, postsReadAllFilesThreadEvents);
            }

            postsIdTextThreadEvents.GetThread().Join();
            postsIdPhotosThreadEvents.GetThread().Join();
            postsIdHrefsThreadEvents.GetThread().Join();
            postsReadAllFilesThreadEvents.GetThread().Join();

            NextStep();
        }

        private void Iteration(ThreadWithEvents firstThread, ThreadWithEvents secondThread, ThreadWithEvents thirdThread)
        {
            if (!isWorking) return;

            firstThread.StartEvent.Set();
            secondThread.StartEvent.Set();
            thirdThread.StartEvent.Set();

            firstThread.FinishEvent.WaitOne();
            secondThread.FinishEvent.WaitOne();
            thirdThread.FinishEvent.WaitOne();

            Thread.Sleep(demonstrationSpeedRate * 4); // Just for better visibility that all threads have been completed

            SetAllStandart();
        }

        private void PostIdTextThread()
        {
            while (postsIdTextThreadEvents.IsRunning)
            {
                postsIdTextThreadEvents.StartEvent.WaitOne();
                if (!postsIdTextThreadEvents.IsRunning) return;

                SetWorking(PostIdTextThreadTextBlock);

                PostIdText.AddInJsonFile(Files.Text.Path, postsIdText, checkExists: false);
                Thread.Sleep(demonstrationSpeedRate * random.Next(1, 8) * 4); // Just for better visibility that threads are waiting for each other

                SetFinished(PostIdTextThreadTextBlock);

                postsIdTextThreadEvents.FinishEvent.Set();
            }
        }

        private void PostIdPhotosThread()
        {
            while (postsIdPhotosThreadEvents.IsRunning)
            {
                postsIdPhotosThreadEvents.StartEvent.WaitOne();
                if (!postsIdPhotosThreadEvents.IsRunning) return;

                SetWorking(PostIdPhotosThreadTextBlock);

                PostIdPhotos.AddInJsonFile(Files.Photos.Path, postsIdPhotos, checkExists: false);
                Thread.Sleep(demonstrationSpeedRate * random.Next(1, 8) * 4); // Just for better visibility that threads are waiting for each other

                SetFinished(PostIdPhotosThreadTextBlock);

                postsIdPhotosThreadEvents.FinishEvent.Set();
            }
        }

        private void PostIdHrefsThread()
        {
            while (postsIdHrefsThreadEvents.IsRunning)
            {
                postsIdHrefsThreadEvents.StartEvent.WaitOne();
                if (!postsIdHrefsThreadEvents.IsRunning) return;

                SetWorking(PostIdHrefsThreadTextBlock);

                PostIdHrefs.AddInJsonFile(Files.Hrefs.Path, postsIdHrefs, checkExists: false);
                Thread.Sleep(demonstrationSpeedRate * random.Next(1, 8) * 4); // Just for better visibility that threads are waiting for each other

                SetFinished(PostIdHrefsThreadTextBlock);

                postsIdHrefsThreadEvents.FinishEvent.Set();
            }
        }

        private void ReadAllFilesThread()
        {
            for (int iteration = 0; postsReadAllFilesThreadEvents.IsRunning; iteration = (iteration + 1) % 3)
            {
                postsReadAllFilesThreadEvents.StartEvent.WaitOne();
                if (!postsReadAllFilesThreadEvents.IsRunning) return;

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

                postsReadAllFilesThreadEvents.FinishEvent.Set();
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
            postsIdTextThreadEvents.Stop();
            postsIdPhotosThreadEvents.Stop();
            postsIdHrefsThreadEvents.Stop();
            postsReadAllFilesThreadEvents.Stop();
        }

        private void DemonstrationSpeedRate_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            demonstrationSpeedRate = (byte)(byte.MaxValue - (byte)e.NewValue);
        }
    }
}
