using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diplom.Entities
{
    public class User
    {
        public int UserId { get; set; }
        public Role UserRole { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string FIO { get; set; }
        public UserStatus UserStatus { get; set; }
        public string Telephone { get; set; }
        public string Email { get; set; }
        public byte[]? Photo { get; set; }
        public Department Department { get; set; }
        public Position Position { get; set; }

        public static User Of(Role role, string login, string password, string FIO, UserStatus status, string tel, string email, byte[]? photo, Department dep, Position pos)
        {
            return new User() {UserRole = role, FIO = FIO, Login = login, Password = password, UserStatus = status, Telephone = tel, Email = email, Photo = photo, Department = dep, Position = pos};
        }
    }
}
