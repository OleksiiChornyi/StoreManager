using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace StoreManager.ViewModels.Help
{
    public interface IBorderDopViewModel
    {
        ICommand BorderDropCommand { get; }
        void ExecuteBorderDrop(DragEventArgs e);

        string DropBorderText { get; set; }
    }
}
