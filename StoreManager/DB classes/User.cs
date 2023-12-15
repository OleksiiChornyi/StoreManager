using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using StoreManager.Models.Abstract.Interfaces;
using StoreManager.Models.SQL_static;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Markup;

namespace StoreManager.DB_classes
{
    public class User
    {
        public User() { }
        public User(string userName, string password, string email, BinaryContent binaryContent, DateTime birthDate, string phoneNumber)
        {
            UserName = userName;
            PasswordHash = Checkings.HashPassword(password);
            Email = email;
            BinaryContent = binaryContent;
            UserRole = Role.client;
            OrderCount = 0;
            BirthDate = birthDate;
            PhoneNumber = phoneNumber;
        }
        public User(string userName, string password)
        {
            UserName = userName;
            PasswordHash = Checkings.HashPassword(password);
            BinaryContent = null;
            OrderCount = 0;
        }
        public int UserID { get; set; }
        public string UserName { get; set; }
        public string PasswordHash { get; set; }
        public string Email { get; set; }
        public BinaryContent BinaryContent { get; set; }
        public Role UserRole { get; set; }
        public int OrderCount { get; set; }
        public DateTime BirthDate { get; set; }
        public string PhoneNumber { get; set; }
        public static User CreateGuest()
        {
            return new User()
            {
                UserName = "Guest",
                PasswordHash = Checkings.HashPassword("1111"),
                UserRole = Role.guest
            };
        }
    }
}
