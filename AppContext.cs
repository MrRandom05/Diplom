using Diplom.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diplom
{
    public class AppContext : DbContext
    {
        public AppContext()
        {
            if (Database.EnsureCreated())
            {
                CreateSampleData();
            }
        }

        public DbSet<ArchiveDocument> ArchiveDocuments { get; set; } = null!;
        public DbSet<DeletedDocument> DeletedDocuments { get; set; } = null!;
        public DbSet<DocumentStatus> DocumentStatuses { get; set; } = null!;
        public DbSet<UserStatus> UsersStatuses { get; set; } = null!;
        public DbSet<Document> Documents { get; set; } = null!;
        public DbSet<UserMail> UsersMails { get; set; } = null!;
        public DbSet<Role> Roles { get; set; } = null!;
        public DbSet<User> Users { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data source=KuzminDiplom.db");
        }

        private void CreateSampleData()
        {
            Roles.Add(Role.Of("админ"));
            UsersStatuses.Add(UserStatus.Of("работает"));
            SaveChanges();
            Users.Add(User.Of(Roles.First(), "1", "1", "sdjfhb jk wsofijg", UsersStatuses.First()));
            SaveChanges();
        }
    }
}
