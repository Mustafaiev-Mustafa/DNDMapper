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
    /// Interaction logic for CreateRegionDialog.xaml
    /// </summary>
    public partial class CreateRegionDialog : Window
    {
        public string RegionName { get; private set; }
        public string RegionOwner { get; private set; }
        public string RegionColor { get; private set; }

        public CreateRegionDialog()
        {
            InitializeComponent();
        }

        private void OnOkClick(object sender, RoutedEventArgs e)
        {
            RegionName = RegionNameInput.Text;
            RegionOwner = RegionOwnerInput.Text;
            RegionColor = RegionColorInput.Text;

            if (string.IsNullOrWhiteSpace(RegionName) || string.IsNullOrWhiteSpace(RegionColor))
            {
                MessageBox.Show("All fields should be field.");
                return;
            }

            DialogResult = true;
            Close();
        }

        private void OnCancelClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
