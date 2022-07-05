using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace OperatingSystemsPractices.Source.Pages.VkParser
{
    public partial class SelectOptionsPage : Page
    {
        public SelectOptionsPage()
        {
            InitializeComponent();
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            if (!Int32.TryParse(NumberOfPostsIntegerUpDown.Text, out var numberOfPosts))
                return;

            if (numberOfPosts < 1)
                return;

            NavigationService.Navigate(new OpenChrome(numberOfPosts));
        }
    }
}
