
using Diplom.Entities;
using Microsoft.EntityFrameworkCore;

namespace Diplom
{
    public partial class EditUser : ContentPage
    {
        private User curUser;
        public EditUser(User userToEdit)
        {
            InitializeComponent();
            curUser = userToEdit;
        }

        private async void LoadRoles(object sender, EventArgs e)
        {
            try
            {
                using AppContext db = new();
                var roles = db.Roles.ToList();
                List<string> rolestxt = new();
                foreach (var r in roles)
                {
                    rolestxt.Add(r.RoleName);
                }
                RolePick.ItemsSource = rolestxt;
                LoadUserInfo();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ошибка", ex.Message, "Ок");
            }
        }

        private async void LoadUserInfo()
        {
            try
            {
                using AppContext db = new();
                var user = db.Users.Include("UserRole").First(x => x.UserId == curUser.UserId);
                if (user != null)
                {
                    Logintxt.Text = user.Login;
                    FIOtxt.Text = user.FIO;
                    Passwordtxt.Text = user.Password;
                    var list = RolePick.ItemsSource as List<string>;
                    int i = list.IndexOf(user.UserRole.RoleName);
                    RolePick.SelectedIndex = i;
                }
                else
                {
                    await DisplayAlert("Ошибка", "Пользователь не найден", "Ок");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ошибка", ex.Message, "Ок");
            }
        }

        private async void Edit(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(Logintxt.Text) || string.IsNullOrEmpty(Passwordtxt.Text) || string.IsNullOrEmpty(FIOtxt.Text))
                {
                    await DisplayAlert("Ошибка", "Заполните все поля", "Ок");
                }
                else
                {
                    using AppContext db = new();
                    if (db.Users.Where(x => x.Login == Logintxt.Text).Any() && Logintxt.Text != curUser.Login)
                    {
                        await DisplayAlert("Успех", "Выбранный логин занят", "Ок");
                    }
                    else
                    {
                        var roletxt = RolePick.SelectedItem as string;
                        var role = db.Roles.First(x => x.RoleName == roletxt);
                        var status = db.UsersStatuses.First();
                        var user = db.Users.Include("UserRole").Include("UserStatus").First(x => x.UserId == curUser.UserId);
                        user.UserRole = role;
                        user.UserStatus = status;
                        user.Login = Logintxt.Text;
                        user.Password = Passwordtxt.Text;
                        user.FIO = FIOtxt.Text;
                        db.SaveChanges();
                        await DisplayAlert("Успех", "Пользователь изменен", "Ок");
                        await Navigation.PopAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ошибка", ex.Message, "Ок");
            }
        }
    }
}