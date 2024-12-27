using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DNDMapper
{
    /// <summary>
    /// Interaction logic for CreateMapDialog.xaml
    /// </summary>
    public partial class CreateMapDialog : Window
    {
        public string MapName { get; private set; }
        public int MapXSize { get; private set; }
        public int MapYSize { get; private set; }

        public CreateMapDialog()
        {
            InitializeComponent();
        }

        private void OnOkClick(object sender, RoutedEventArgs e)
        {
            try
            {
                MapName = NameInput.Text;
                MapXSize = int.Parse(XInput.Text);
                MapYSize = int.Parse(YInput.Text);

                if (string.IsNullOrWhiteSpace(MapName) || MapXSize <= 0 || MapYSize <= 0)
                {
                    throw new ArgumentException("Incorrect data");
                }

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().Name, MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}