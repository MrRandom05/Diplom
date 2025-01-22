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
            var man = Roles.First(z => z.RoleName == "руководитель");
            var work = Roles.First(z => z.RoleName == "работник");
            var man1 = Positions.First(z => z.PositionName == "Старший менеджер");
            var work1 = Positions.First(z => z.PositionName == "Рядовой сотрудник");
            Users.Add(User.Of(Roles.First(), "1", "1", "Кузьмин Н. С.", UsersStatuses.First(), "+8 (888) 660-90-00", "wwaaa@ya.com", null, Departments.First(), Positions.First()));
            Users.Add(User.Of(man, "2", "2", "Иванов Н. С.", UsersStatuses.First(), "+8 (568) 678-98-70", "wwaaa1243@ya.com", null, Departments.First(), man1));
            Users.Add(User.Of(work, "3", "3", "Сидоров Н. С.", UsersStatuses.First(), "+8 (868) 660-34-21", "wwa2354aa@ya.com", null, Departments.First(), work1));
            SaveChanges();
            UsersMails.Add(new UserMail() { Getter = Users.First(), SendDate = DateTime.Now.Date, Sender = Users.Last(), UserEmailTitle = "Тест", UserEmailBody = "Отправка писем работает"});
            UsersMails.Add(new UserMail() { Getter = Users.Last(), SendDate = DateTime.Now.Date, Sender = Users.First(), UserEmailTitle = "Обратный тест", UserEmailBody = "Отправка писем работает"});
            SaveChanges();
        }
    }
}
