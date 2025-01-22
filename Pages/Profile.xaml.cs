using Diplom.Entities;
using Microsoft.EntityFrameworkCore;


namespace Diplom
{
    public partial class Profile : ContentPage
    {
        private User curUser;
        private string imagePath;
        public Profile(User user)
        {
            InitializeComponent();
            curUser = user;
            imagePath = "";
            LoadUserInfo();
        }

        private void DisposeFile(object sender, EventArgs e)
        {
            ProfilePhoto.Source = null;
        }

        private async void LoadUserInfo()
        {
            try
            {
                using AppContext db = new();
                db.Departments.Load();
                db.Positions.Load();
                var user = db.Users.First(x => x.UserId == curUser.UserId);
                if (user != null)
                {
                    Logintxt.Text = user.Login;
                    FIOtxt.Text = user.FIO;
                    Telephonetxt.Text = user.Telephone;
                    Emailtxt.Text = user.Email;
                    Departmenttxt.Text = user.Department.DepartmentName;
                    Positiontxt.Text = user.Position.PositionName;
                    if (user.Photo != null)
                    {
                        var path = $"{FileSystem.Current.AppDataDirectory}/{user.Login}_{user.UserId}.png";
                        // if (!File.Exists(path))
                        // {
                        //     File.WriteAllBytes(path, user.Photo);
                        //     ProfilePhoto.Source = path;
                        // }
                        // else
                        // {
                        //     File.OpenHandle(path);
                        //     File.WriteAllBytes(path, user.Photo);
                        //     ProfilePhoto.Source = path;
                        // }
                        ProfilePhoto.Source = ImageSource.FromStream(() =>
                        {
                           MemoryStream ms = new MemoryStream(curUser.Photo);
                           return ms; 
                        });
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
                    if (!string.IsNullOrEmpty(Passwordtxt.Text))
                    {
                        using AppContext db = new();
                        var reg = new System.Text.RegularExpressions.Regex("^[a-z0-9!#$%&'*+/=?^_{|}~-]+(?:\\.[a-z0-9!#$%&'*+/=?^_{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?$");
                        var user = db.Users.First(x => x.UserId == curUser.UserId);
                        if (db.Users.Where(x => x.Login == Logintxt.Text).Any() && Logintxt.Text != curUser.Login)
                        {
                            await DisplayAlert("Успех", "Данный логин занят", "Ок");
                        }
                        else
                        {
                            var old = await DisplayPromptAsync("Подтвердите смену пароля", "Старый пароль:", "Ок", "Отмена");
                            if (old == curUser.Password)
                            {
                                user.Password = Passwordtxt.Text;
                                user.Login = Logintxt.Text;
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
                                    user.Telephone = Telephonetxt.Text;
                                    if (reg.IsMatch(Emailtxt.Text))
                                    {
                                        user.Email = Emailtxt.Text;
                                        if (!string.IsNullOrEmpty(imagePath))
                                        {
                                            user.Photo = File.ReadAllBytes(imagePath);
                                        }
                                        db.SaveChanges();
                                        await DisplayAlert("Успех", "Данные сохранены", "Ок");
                                        await Navigation.PopAsync();
                                    }
                                    else await DisplayAlert("Ошибка", "Неверный формат почты", "Ок");
                                }
                            }
                            else await DisplayAlert("Ошибка", "Неверный пароль", "Ок");

                        }
                    }
                    else
                    {
                       using AppContext db = new();
                        var user = db.Users.First(x => x.UserId == curUser.UserId);
                        var reg = new System.Text.RegularExpressions.Regex("^[a-z0-9!#$%&'*+/=?^_{|}~-]+(?:\\.[a-z0-9!#$%&'*+/=?^_{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?$");
                        
                        if (db.Users.Where(x => x.Login == Logintxt.Text).Any() && Logintxt.Text != curUser.Login)
                        {
                            await DisplayAlert("Ошибка", "Данный логин занят", "Ок");
                        }
                        else
                        {
                            user.Login = Logintxt.Text;
                            var telarr = Telephonetxt.Text.Where(x => char.IsDigit(x) == true);
                            string tel = "";
                            foreach(var t in telarr)
                            {
                                tel += t;
                            }
                            if (string.IsNullOrEmpty(tel) || tel.Count() < 11)
                            {
                                await DisplayAlert("Ошибка", "Неверный формат телефона", "Ок");
                            }
                            else
                            {
                                user.Telephone = Telephonetxt.Text;
                                if (reg.IsMatch(Emailtxt.Text))
                                {
                                    user.Email = Emailtxt.Text;
                                    if (!string.IsNullOrEmpty(imagePath))
                                    {
                                        user.Photo = File.ReadAllBytes(imagePath);
                                    }
                                    db.SaveChanges();
                                    await DisplayAlert("Успех", "Данные сохранены", "Ок");
                                    await Navigation.PopAsync();
                                }
                                else await DisplayAlert("Ошибка", "Неверный формат почты", "Ок");
                            }

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