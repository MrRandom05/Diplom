using Diplom.Entities;
using Microsoft.EntityFrameworkCore;

namespace Diplom
{
    public partial class NewMail : ContentPage
    {
        User curUser;
        List<FileResult> attachedFiles = new List<FileResult>();
        public NewMail(User user)
        {
            InitializeComponent();
            curUser = user;
        }

        private async void LoadGetters(object sender, EventArgs e)
        {
            try
            {
                using AppContext db = new();
                var getters = db.Users.ToList();
                List<string> getterstxt = new();
                foreach (var g in getters)
                {
                    getterstxt.Add(g.FIO);
                }
                GetterPick.ItemsSource = getterstxt;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ошибка", ex.Message, "Ок");
            }
        }

        private async void AttachDocs(object sender, EventArgs e)
        {
            try
            {
                using AppContext db = new();
                var result = await FilePicker.PickMultipleAsync();
                if (result != null)
                {
                    var res = result.ToList();
                    foreach( var r in res)
                    {
                        attachedFiles.Add(r);
                    }
                    if (attachedFiles.Count() == 0)
                    {
                        Attachedlbl.Text = "Нет прикрепленных файлов";
                    }
                    else if (attachedFiles.Count() <= 1)
                    {
                        Attachedlbl.Text = $"{attachedFiles[0].FileName}";
                    }
                    else if (attachedFiles.Count() > 1)
                    {
                        var count = attachedFiles.Count()-1;
                        Attachedlbl.Text = $"{attachedFiles[0].FileName} и еще {count} файлов";
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ошибка", ex.Message, "Ок");
            }
        }

        private async void SendMail(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(Themetxt.Text) || string.IsNullOrEmpty(Texttxt.Text) || GetterPick.SelectedIndex == -1)
                {
                    await DisplayAlert("Ошибка", "Для отправки необходимо заполнить тему и текст сообщения, а также выбрать отправителя", "Ок");
                }
                else
                {
                    using AppContext db = new();
                    var gettxt = GetterPick.SelectedItem as string;
                    var getter = db.Users.Include("UserRole").First(x => x.FIO == gettxt);
                    var senderUser = db.Users.First(x => x.UserId == curUser.UserId);
                    UserMail mail = new UserMail() { Getter = getter, SendDate = DateTime.Now, Sender = senderUser, UserEmailTitle = Themetxt.Text, UserEmailBody = Texttxt.Text };
                    if (attachedFiles.Count() > 0)
                    {
                        var existings = new List<Document>();
                        foreach (var a in attachedFiles)
                        {
                            if (db.Documents.Where(x => x.DocumentName == a.FileName).Any())
                            {
                                var exist = db.Documents.First(x => x.DocumentName == a.FileName);
                                existings.Add(exist);
                            }
                            else
                            {
                                var status = db.DocumentStatuses.First(x => x.DocumentStatusName == "в работе");
                                var creator = db.Users.Include("UserRole").First(x => x.UserId == curUser.UserId);
                                var priv = getter.UserRole;
                                var doc = Document.Of(a.FileName, File.ReadAllBytes(a.FullPath), status, creator, priv);
                                db.Documents.Add(doc);
                                db.SaveChanges();
                                var added = db.Documents.First(x => x.DocumentName == a.FileName);
                                existings.Add(added);
                            }
                        }
                        mail.AttachedDocuments = existings;
                    }
                        db.UsersMails.Add(mail);
                        db.SaveChanges();
                        await DisplayAlert("Успех", "Письмо отправлено", "Ок");
                        await Navigation.PopAsync();
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ошибка", ex.Message, "Ок");
            }
        }

    }
}