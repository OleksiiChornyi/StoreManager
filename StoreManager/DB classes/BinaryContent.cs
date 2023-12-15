using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;

namespace StoreManager.DB_classes
{
    public class BinaryContent
    {
        public BinaryContent() { }
        public BinaryContent(int contentID, string fileName, string fileType, string fileExtension, DateTime uploadDate, DateTime modificationDate, byte[] content) 
        {
            ContentID = contentID;
            FileName = fileName;
            FileType = fileType;
            FileExtension = fileExtension;
            UploadDate = uploadDate;
            ModificationDate = modificationDate;
            Content = content;
        }
        public BinaryContent(string filePath)
        {
            FileInfo fileInfo = new FileInfo(filePath);
            FileName = fileInfo.Name;
            FileType = "Image";
            FileExtension = fileInfo.Extension;
            Content = File.ReadAllBytes(filePath);
        }
        public int ContentID { get; set; }
        public string FileName { get; set; }
        public string FileType { get; set; }
        public string FileExtension { get; set; }
        public DateTime UploadDate { get; set; }
        public DateTime ModificationDate { get; set; }
        public byte[] Content { get; set; }
    }
}
