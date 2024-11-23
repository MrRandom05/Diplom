using Diplom.Entities;
using Microsoft.EntityFrameworkCore;

namespace Diplom
{
    public partial class Profile : ContentPage
    {
        private User curUser;
        public Profile(User user)
        {
            InitializeComponent();
            curUser = user;
            LoadUserInfo();
        }

        private async void LoadUserInfo()
        {
            try
            {
                using AppContext db = new();
                var user = db.Users.First(x => x.UserId == curUser.UserId);
                if (user != null)
                {
                    Logintxt.Text = user.Login;
                    FIOtxt.Text = user.FIO;
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

        private async void EditProfile(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(Logintxt.Text) || string.IsNullOrEmpty(FIOtxt.Text))
                {
                    await DisplayAlert("Ошибка", "Поля ФИО и Логин не должны быть пустыми", "Ок");
                }
                else
                {
                    if (curUser.Login == Logintxt.Text && curUser.FIO == FIOtxt.Text && (Passwordtxt.Text == curUser.Password || string.IsNullOrEmpty(Passwordtxt.Text)))
                    {
                        await DisplayAlert("Успех", "Данные не были изменены пользователем", "Ок");
                        await Navigation.PopAsync();
                    }
                    else if (!string.IsNullOrEmpty(Passwordtxt.Text))
                    {
                        using AppContext db = new();
                        var user = db.Users.First(x => x.UserId == curUser.UserId);
                        if (db.Users.Where(x => x.Login == Logintxt.Text).Any() && Logintxt.Text != curUser.Login)
                        {
                            await DisplayAlert("Успех", "Данный логин занят", "Ок");
                        }
                        else
                        {
                            user.Login = Logintxt.Text;
                            user.FIO = FIOtxt.Text;
                            user.Password = Passwordtxt.Text;
                            db.SaveChanges();
                            await DisplayAlert("Успех", "Данные сохранены", "Ок");
                            await Navigation.PopAsync();
                        }
                    }
                    else
                    {
                        using AppContext db = new();
                        var user = db.Users.First(x => x.UserId == curUser.UserId);
                        user.Login = Logintxt.Text;
                        user.FIO = FIOtxt.Text;
                        db.SaveChanges();
                        await DisplayAlert("Успех", "Данные сохранены", "Ок");
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