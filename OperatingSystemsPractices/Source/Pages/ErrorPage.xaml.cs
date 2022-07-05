using System.Windows.Controls;

namespace OperatingSystemsPractices.Source.Pages
{
    public partial class ErrorPage : Page
    {
        public ErrorPage(string message)
        {
            InitializeComponent();
            TextBlock.Text = message;
            
        }
    }
}
