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
    public class UserItem : INotifyPropertyChanged
    {
        public UserItem(User user)
        {
            this.user = user;
        }
        public User user { get; set; }

        public BitmapImage Image_src
        {
            get
            {
                return Convert(user.BinaryContent.Content);
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
