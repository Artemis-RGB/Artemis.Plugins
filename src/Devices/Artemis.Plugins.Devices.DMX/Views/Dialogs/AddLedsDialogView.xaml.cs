using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Artemis.Plugins.Devices.DMX.Views.Dialogs
{
    /// <summary>
    ///     Interaction logic for AddLedsDialogView.xaml
    /// </summary>
    public partial class AddLedsDialogView : UserControl
    {
        public AddLedsDialogView()
        {
            InitializeComponent();
        }

        private void FrameworkElement_OnLoaded(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox) sender;
            Keyboard.Focus(textBox);
            textBox.SelectAll();
        }
    }
}