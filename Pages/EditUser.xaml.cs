
using Diplom.Entities;
using Microsoft.EntityFrameworkCore;

namespace Diplom
{
    public partial class EditUser : ContentPage
    {
        private User curUser;
        private string imagePath;
        public EditUser(User userToEdit)
        {
            InitializeComponent();
            curUser = userToEdit;
            imagePath = "";
        }

        private async void LoadPhoto(object sender, EventArgs e)
        {
            try
            {
                var options = new PickOptions() { FileTypes = FilePickerFileType.Png };
                var result = await FilePicker.Default.PickAsync(options);
                if (result != null)
                {
                    ProfilePhoto.Source = result.FullPath;
                    imagePath = result.FullPath;
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ошибка", ex.Message, "Ок");
            }
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
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ошибка", ex.Message, "Ок");
            }
        }

        private async void LoadPositions(object sender, EventArgs e)
        {
            try
            {
                using AppContext db = new();
                var positions = db.Positions.Select(z => z.PositionName).ToList();
                PositionPick.ItemsSource = positions;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ошибка", ex.Message, "Ок");
            }
        }
        private async void LoadDeps(object sender, EventArgs e)
        {
            try
            {
                using AppContext db = new();
                var deps = db.Departments.Select(z => z.DepartmentName).ToList();
                DepartmentPick.ItemsSource = deps;
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
                var user = db.Users.Include("UserRole").Include("Department").Include("Position").First(x => x.UserId == curUser.UserId);
                if (user != null)
                {
                    Logintxt.Text = user.Login;
                    FIOtxt.Text = user.FIO;
                    Passwordtxt.Text = user.Password;
                    Telephonetxt.Text = user.Telephone;
                    Emailtxt.Text = user.Email;
                    RolePick.SelectedIndex = (RolePick.ItemsSource as List<string>).IndexOf(user.UserRole.RoleName);
                    DepartmentPick.SelectedIndex = (DepartmentPick.ItemsSource as List<string>).IndexOf(user.Department.DepartmentName);
                    PositionPick.SelectedIndex = (PositionPick.ItemsSource as List<string>).IndexOf(user.Position.PositionName);
                    if (user.Photo != null)
                    {
                        var path = $"{FileSystem.Current.AppDataDirectory}/{user.Login}_{user.UserId}.png";
                        if (!File.Exists(path))
                        {
                            File.WriteAllBytes(path, user.Photo);
                            ProfilePhoto.Source = path;
                        }
                        else
                        {
                            File.Delete(path);
                            File.WriteAllBytes(path, user.Photo);
                            ProfilePhoto.Source = path;
                        }
                    }
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