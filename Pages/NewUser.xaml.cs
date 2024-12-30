
using Diplom.Entities;

namespace Diplom
{
    public partial class NewUser : ContentPage
    {
        public NewUser()
        {
            InitializeComponent();
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
                RolePick.SelectedIndex = 0;
                
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ошибка", ex.Message, "Ок");
            }
        }

        private async void CreateUser(object sender, EventArgs e)
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
                    if (db.Users.Where(x => x.Login == Logintxt.Text).Any())
                    {
                        await DisplayAlert("Ошибка", "выбранный логин занят", "Ок");
                    }
                    else
                    {
                        // var roletxt = RolePick.SelectedItem as string;
                        // var role = db.Roles.First(x => x.RoleName == roletxt);
                        // var status = db.UsersStatuses.First();
                        // User newUser = User.Of(role, Logintxt.Text, Passwordtxt.Text, FIOtxt.Text, status, "", "", null, );
                        // db.Users.Add(newUser);
                        // db.SaveChanges();
                        await DisplayAlert("Успех", "Пользователь добавлен", "Ок");
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