using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diplom.Entities
{
    public class UserStatus
    {
        public int UserStatusId {  get; set; }
        public string UserStatusName { get; set; }

        public static UserStatus Of(string name)
        {
            return new UserStatus() {UserStatusName = name};
        }
    }
}
