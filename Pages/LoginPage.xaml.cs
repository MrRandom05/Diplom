using System.Data.Common;
using Diplom.Entities;
using Microsoft.EntityFrameworkCore;

namespace Diplom
{
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            InitializeComponent();
            using AppContext db = new();
            db.Users.Load();
        }

        private async void TryLogin(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(Passwordtxt.Text) || string.IsNullOrEmpty(Logintxt.Text))
                {
                    await DisplayAlert("Ошибка", "Все поля должны быть заполнены", "Ок");
                }
                else
                {
                    using AppContext db = new();
                    User? user = db.Users.Include("UserRole").First(x => x.Login == Logintxt.Text);
                    if (user is null) await DisplayAlert("Ошибка", "Пользователя с таким логином не существует", "Ок");
                    else if (user.Password != Passwordtxt.Text) await DisplayAlert("Ошибка", "Неверный логин или пароль", "Ок");
                    else if (user.Password == Passwordtxt.Text)
                    {
                        switch (user.UserRole.RoleName)
                        {
                            case "админ":
                            await Navigation.PushAsync(new AdminPage(user));
                            break;
                            case "руководитель":
                            await Navigation.PushAsync(new ManagerPage(user));
                            break;
                        }
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