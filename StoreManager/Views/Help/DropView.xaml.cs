using StoreManager.ViewModels.Help;
using System;
using System.Collections.Generic;
using System.Globalization;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace StoreManager.Views.Help
{
    /// <summary>
    /// Interaction logic for DropView.xaml
    /// </summary>
    public partial class DropView : UserControl
    {
        public DropView()
        {
            InitializeComponent();
        }

        private void dropBorder_Drop(object sender, DragEventArgs e)
        {
            (DataContext as IBorderDopViewModel)?.BorderDropCommand?.Execute(e);
        }
    }
}
