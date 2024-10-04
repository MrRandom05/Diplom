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

        public static User Of(Role role, string login, string password, string FIO, UserStatus status)
        {
            return new User() {UserRole = role, FIO = FIO, Login = login, Password = password, UserStatus = status};
        }
    }
}
