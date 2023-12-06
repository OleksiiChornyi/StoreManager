using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace StoreManager.ViewModels.Data
{
    public class ImageItem : INotifyPropertyChanged
    {
        public int productId { get; set; }
        /// <summary>
        /// Text of item
        /// </summary>
        public string Name { get; set; }
        public string fileNameImage { get; set; }
        public int categoryId { get; set; }
        public string categoryName { get; set; }
        public string categoryDescription { get; set; }
        public int price { get; set; }
        public int salesCount { get; set; }
        public int? descriptionId { get; set; }

        /// <summary>
        /// Image
        /// </summary>
        private byte[] imageBytes;

        public byte[] ImageBytes
        {
            get { return imageBytes; }
            set
            {
                imageBytes = value;
                OnPropertyChanged(nameof(Image_src));
            }
        }

        public BitmapImage Image_src
        {
            get
            {
                return Convert(ImageBytes);
            }
            set
            { }
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
        /// <summary>
        /// OnPropertyChanged
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
