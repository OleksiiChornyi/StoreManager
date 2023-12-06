using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace StoreManager.ViewModels.Converters
{
    internal class ConfirmOrderButtonConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length > 0 && values[0] is int dataGridItemCount && dataGridItemCount > 0)
            {
                if (values.Length > 1 && values[1] is bool isChecked && !isChecked)
                {
                    return true;
                }

                if (values.Length > 2 && values[2] is string cardNumber && cardNumber.Length == 16)
                {
                    return true;
                }
            }

            return false;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
