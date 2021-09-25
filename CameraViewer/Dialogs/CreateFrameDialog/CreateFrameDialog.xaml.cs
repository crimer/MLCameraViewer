using System.Windows.Controls;
using System.Text.RegularExpressions;
using System.Windows.Input;

namespace CameraViewer.Dialogs.CreateFrameDialog
{
    /// <summary>
    /// Interaction logic for CreateFrameDialog.xaml
    /// </summary>
    public partial class CreateFrameDialog : UserControl
    {
        public CreateFrameDialog()
        {
            InitializeComponent();
        }
        
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}
