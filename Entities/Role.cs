using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diplom.Entities
{
    public class Role
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; }

        public static List<Role>? GetRoles()
        {
            try
            {
                using AppContext db = new();
                return db.Roles.ToList();
            }
            catch (Exception ex) {}
            return null;
        }

        public static Role Of(string name)
        {
            return new Role() {RoleName = name};
        }
    }
}
