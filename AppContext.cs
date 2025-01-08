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
        public DbSet<DocumentStatus> DocumentStatuses { get; set; } = null!;
        public DbSet<FavoriteMail> FavoriteMails { get; set; } = null!;
        public DbSet<UserStatus> UsersStatuses { get; set; } = null!;
        public DbSet<Department> Departments { get; set; } = null!;
        public DbSet<UserMail> UsersMails { get; set; } = null!;
        public DbSet<Position> Positions { get; set; } = null!;
        public DbSet<Document> Documents { get; set; } = null!;
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
            Positions.Add(Position.Of("Администратор"));
            Positions.Add(Position.Of("Старший менеджер"));
            Positions.Add(Position.Of("Специалист"));
            Positions.Add(Position.Of("Младший менеджер"));
            Positions.Add(Position.Of("Рядовой сотрудник"));
            Departments.Add(Department.Of("Административный отдел"));
            Departments.Add(Department.Of("Производственный отдел"));
            Departments.Add(Department.Of("Логистический отдел"));
            Departments.Add(Department.Of("Технический отдел"));
            Departments.Add(Department.Of("IT отдел"));
            UsersStatuses.Add(UserStatus.Of("активен"));
            UsersStatuses.Add(UserStatus.Of("заблокирован"));
            UsersStatuses.Add(UserStatus.Of("удален"));
            DocumentStatuses.Add(new DocumentStatus() {DocumentStatusName = "создан"});
            DocumentStatuses.Add(new DocumentStatus() {DocumentStatusName = "в работе"});
            DocumentStatuses.Add(new DocumentStatus() {DocumentStatusName = "удален"});
            DocumentStatuses.Add(new DocumentStatus() {DocumentStatusName = "в архиве"});
            DocumentStatuses.Add(new DocumentStatus() {DocumentStatusName = "подписан"});
            DocumentStatuses.Add(new DocumentStatus() {DocumentStatusName = "на подписании"});
            DocumentStatuses.Add(new DocumentStatus() {DocumentStatusName = "выполнен"});
            SaveChanges();
            Users.Add(User.Of(Roles.First(), "1", "1", "Кузьмин Н. С.", UsersStatuses.First(), "88005553535", "wwaaa@ya.com", null, Departments.First(), Positions.First()));
            SaveChanges();
        }
    }
}
