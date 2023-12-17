using StoreManager.DB_classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace StoreManager.Models.Data
{
    public class ProductItem : INotifyPropertyChanged
    {
        public ProductItem()
        {
            
        }
        public ProductItem(Product product)
        {
            Product = product;
        }
        public Product Product { get; set; }

        public BitmapImage Image_src
        {
            get
            {
                return Convert(Product.BinaryContent.Content);
            }
            set { }
        }

        private BitmapImage Convert(byte[] bytes)
        {
            try
            {
                var image = new BitmapImage();
                using (var stream = new MemoryStream(bytes))
                {
                    image.BeginInit();
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.StreamSource = stream;
                    image.EndInit();
                }
                return image;
            }
            catch
            {
                return null;
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
