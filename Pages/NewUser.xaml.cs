
using Diplom.Entities;

namespace Diplom
{
    public partial class NewUser : ContentPage
    {
        private string imagePath;
        public NewUser()
        {
            InitializeComponent();
            imagePath = "";
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
                var reg = new System.Text.RegularExpressions.Regex("^[a-z0-9!#$%&'*+/=?^_{|}~-]+(?:\\.[a-z0-9!#$%&'*+/=?^_{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?$");
                if (string.IsNullOrEmpty(Logintxt.Text) || string.IsNullOrEmpty(Passwordtxt.Text) 
                || string.IsNullOrEmpty(FIOtxt.Text) || string.IsNullOrEmpty(Telephonetxt.Text) || string.IsNullOrEmpty(Emailtxt.Text) ||
                RolePick.SelectedIndex == -1 || DepartmentPick.SelectedIndex == -1 || PositionPick.SelectedIndex == -1)
                {
                    await DisplayAlert("Ошибка", "Заполните все поля", "Ок");
                }
                else if (!reg.IsMatch(Emailtxt.Text))
                {
                    await DisplayAlert("Ошибка", "Неверный формат почты", "Ок");
                }
                else
                {
                    using AppContext db = new();
                    var telarr = Telephonetxt.Text.Where(x => char.IsDigit(x) == true);
                    string tel = "";
                    foreach (var t in telarr)
                    {
                        tel += t;
                    }
                    if (string.IsNullOrEmpty(tel) || tel.Count() < 11)
                    {
                        await DisplayAlert("Ошибка", "Неверный формат телефона", "Ок");
                    }
                    else
                    {
                        if (db.Users.Where(x => x.Login == Logintxt.Text).Any())
                        {
                            await DisplayAlert("Ошибка", "выбранный логин занят", "Ок");
                        }
                        else
                        {
                            var roletxt = RolePick.SelectedItem as string;
                            var role = db.Roles.First(x => x.RoleName == roletxt);
                            var dep = db.Departments.First(z => z.DepartmentName == DepartmentPick.SelectedItem as string);
                            var pos = db.Positions.First(z => z.PositionName == PositionPick.SelectedItem as string);
                            var status = db.UsersStatuses.First();
                            User newUser = User.Of(role, Logintxt.Text, Passwordtxt.Text, FIOtxt.Text, status, Telephonetxt.Text, Emailtxt.Text, null, dep, pos);
                            if (!string.IsNullOrEmpty(imagePath))
                            {
                                newUser.Photo = File.ReadAllBytes(imagePath);
                            }
                            db.Users.Add(newUser);
                            db.SaveChanges();
                            await DisplayAlert("Успех", "Пользователь добавлен", "Ок");
                            await Navigation.PopAsync();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ошибка", ex.Message, "Ок");
            }
        }

        private async void LoadPhoto(object sender, EventArgs e)
        {
            try
            {
                var options = new PickOptions() { FileTypes = FilePickerFileType.Png};
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
        
    }
}