using StoreManager.Models.Abstract.Classes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace StoreManager.DB_classes
{
    public class Description
    {
        public Description()
        { }
        public Description(string filePath, string info)
        {
            FileInfo fileInfo = new FileInfo(filePath);
            string fileName = fileInfo.Name;
            string fileExtension = fileInfo.Extension;
            string fileType = AdminStoreInteraction.GetFileType(fileExtension);
            byte[] fileData = File.ReadAllBytes(filePath);
            FileName = fileName;
            FileExtension = fileExtension;
            FileType = fileType;
            FileData = fileData;
            Info = info;
        }
        public Description(int Id, string filePath, string info)
        {
            FileInfo fileInfo = new FileInfo(filePath);
            string fileName = fileInfo.Name;
            string fileExtension = fileInfo.Extension;
            string fileType = AdminStoreInteraction.GetFileType(fileExtension);
            byte[] fileData = File.ReadAllBytes(filePath);
            DescriptionID = Id;
            FileName = fileName;
            FileExtension = fileExtension;
            FileType = fileType;
            FileData = fileData;
            Info = info;
        }

        public Description(int descriptionId, string descriptionFileName, string descriptionFileType, string descriptionFileExtension, byte[] descriptionFileData, DateTime descriptionUploadDate, DateTime descriptionModificationDate, string descriptionInfo)
        {
            DescriptionID = descriptionId;
            FileName = descriptionFileName;
            FileType = descriptionFileType;
            FileExtension = descriptionFileExtension;
            FileData = descriptionFileData;
            UploadDate = descriptionUploadDate;
            ModificationDate = descriptionModificationDate;
            Info = descriptionInfo;
        }

        public int DescriptionID { get; set; }
        public string FileName { get; set; }
        public string FileType { get; set; }
        public string FileExtension { get; set; }
        public byte[] FileData { get; set; }
        public DateTime UploadDate { get; set; }
        public DateTime ModificationDate { get; set; }
        public string Info { get; set; }
    }
}
