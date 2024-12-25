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

        public DbSet<FavoriteDocument> FavoriteDocuments { get; set; } = null!;
        public DbSet<ArchiveDocument> ArchiveDocuments { get; set; } = null!;
        public DbSet<DeletedDocument> DeletedDocuments { get; set; } = null!;
        public DbSet<DocumentStatus> DocumentStatuses { get; set; } = null!;
        public DbSet<FavoriteMail> FavoriteMails { get; set; } = null!;
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
            Roles.Add(Role.Of("руководитель"));
            Roles.Add(Role.Of("работник"));
            UsersStatuses.Add(UserStatus.Of("работает"));
            UsersStatuses.Add(UserStatus.Of("в отпуске"));
            UsersStatuses.Add(UserStatus.Of("больничный"));
            DocumentStatuses.Add(new DocumentStatus() {DocumentStatusName = "в работе"});
            DocumentStatuses.Add(new DocumentStatus() {DocumentStatusName = "удален"});
            DocumentStatuses.Add(new DocumentStatus() {DocumentStatusName = "восстановлен"});
            DocumentStatuses.Add(new DocumentStatus() {DocumentStatusName = "в архиве"});
            SaveChanges();
            Users.Add(User.Of(Roles.First(), "1", "1", "Кузьмин Н. С.", UsersStatuses.First()));
            SaveChanges();
        }
    }
}
