using System.Windows;

namespace OperatingSystemsPractices.Source
{
    public partial class ChooseTaskWindow : Window
    {
        public ChooseTaskWindow()
        {
            InitializeComponent();
        }

        private void VkParserButton_Click(object sender, RoutedEventArgs e)
        {
            TaskWindow taskWindow = new TaskWindow();
            taskWindow.TaskFrame.Content = new Pages.VkParser.SelectOptionsPage();
            taskWindow.Show();
        }
    }
}
