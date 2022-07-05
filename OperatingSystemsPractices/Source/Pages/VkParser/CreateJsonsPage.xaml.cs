using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using OperatingSystemsPractices.Source.Vk;

namespace OperatingSystemsPractices.Source.Pages.VkParser
{
    public partial class CreateJsonsPage : Page
    {
        List<Post> posts;
        Thread vkNewsThread;
        Thread postIdTextThread;
        Thread postIdPhotosThread;
        Thread postIdHrefsThread;
        Thread readAllFiles;
        Thread workThread;

        public CreateJsonsPage(List<Post> posts)
        {
            InitializeComponent();

            this.posts = posts;

            workThread = new Thread(() => Start()) { IsBackground = true };
            workThread.Start();
        }

        private void Start()
        {
            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) +
                                  @"\OperatingSystemsPractices";

            DirectoryInfo dirInfo = new DirectoryInfo(documentsPath);
            if (!dirInfo.Exists)
                dirInfo.Create();

            MessageBoxResult addPosts = MessageBoxResult.No;
            if (File.Exists(documentsPath + @"\VkNews.json"))
            {
                addPosts = MessageBox.Show("The file already exists. Do you want to add new posts to it?" + "\n" + "Otherwise, a new file will be created.",
                    "Message", MessageBoxButton.YesNo, MessageBoxImage.Question);
            }

            readAllFiles = new Thread(ReadAllFiles);

            if (addPosts == MessageBoxResult.Yes)
            {
                UpdateStatus("Adding to jsons...");
                vkNewsThread = new Thread(() => Post.AddInJsonFile(documentsPath + @"\VkNews.json", posts)) { IsBackground = true };
                postIdTextThread = new Thread(() => PostIdText.AddInJsonFile(documentsPath + @"\1.json", PostIdText.Create(posts))) { IsBackground = true };
                postIdPhotosThread = new Thread(() => PostIdPhotos.AddInJsonFile(documentsPath + @"\2.json", PostIdPhotos.Create(posts))) { IsBackground = true };
                postIdHrefsThread = new Thread(() => PostIdHrefs.AddInJsonFile(documentsPath + @"\3.json", PostIdHrefs.Create(posts))) { IsBackground = true };
            }
            else
            {
                UpdateStatus("Creating jsons...");
                vkNewsThread = new Thread(() => Post.CreateJsonFile(documentsPath + @"\VkNews.json", posts)) { IsBackground = true };
                postIdTextThread = new Thread(() => PostIdText.CreateJsonFile(documentsPath + @"\1.json", PostIdText.Create(posts))) { IsBackground = true };
                postIdPhotosThread = new Thread(() => PostIdPhotos.CreateJsonFile(documentsPath + @"\2.json", PostIdPhotos.Create(posts))) { IsBackground = true };
                postIdHrefsThread = new Thread(() => PostIdHrefs.CreateJsonFile(documentsPath + @"\3.json", PostIdHrefs.Create(posts))) { IsBackground = true };
            }

            vkNewsThread.Start();
            postIdTextThread.Start();
            postIdPhotosThread.Start();
            postIdHrefsThread.Start();
            readAllFiles.Start();

            vkNewsThread.Join();
            postIdTextThread.Join();
            postIdPhotosThread.Join();
            postIdHrefsThread.Join();
            readAllFiles.Join();

            Process.Start("explorer.exe", documentsPath);

            NextStep();
        }

        private void ReadAllFiles()
        {
            System.Threading.Thread.Sleep(1000);
            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) +
                                                   @"\OperatingSystemsPractices";
            string[] filesTexts = new string[3];
            for (int fileNumber = 1; fileNumber <= 3; fileNumber++)
            {
                try
                {
                    string filePath = documentsPath + "\\" + fileNumber.ToString() + ".json";
                    string fileCopyPath = documentsPath + "\\" + fileNumber.ToString() + "Temp.json";

                    File.Copy(filePath, fileCopyPath);
                    filesTexts[fileNumber - 1] = File.ReadAllText(fileCopyPath);
                    File.Delete(fileCopyPath);
                }
                catch
                {
                    Error("Some error occurred!");
                }
            }
        }

        private void NextStep()
        {
            Dispatcher.InvokeAsync(() => NavigationService.Navigate(new WriteAndReadJsons(posts)));
        }

        private void Error(string message)
        {
            Dispatcher.InvokeAsync(() => NavigationService.Navigate(new ErrorPage(message)));
            workThread.Abort();
            vkNewsThread.Abort();
            postIdTextThread.Abort();
            postIdPhotosThread.Abort();
            postIdHrefsThread.Abort();
            readAllFiles.Abort();
        }

        private void UpdateStatus(string text)
        {
            StatusTextBlock.Dispatcher.Invoke(() => StatusTextBlock.Text = text);
        }
    }
}
