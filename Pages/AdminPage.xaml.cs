using Diplom.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Maui.Controls.PlatformConfiguration;
using Microsoft.Maui.Handlers;

namespace Diplom
{
    public enum ListType{
        Input,
        Output,
        Documents,
        ArchiveDocuments,
        DeletedDocuments
    }
    public partial class AdminPage : ContentPage
    {
        private User curUser;
        private ListType curListViewType = ListType.Input;

        private void SetInputMailDataTemplate()
        {
            Label sender = new Label {FontSize = 16, WidthRequest = 200, LineBreakMode = LineBreakMode.NoWrap, Margin = new Thickness(30,0,0,0), HorizontalTextAlignment = TextAlignment.Center};
            Label title = new Label {FontSize = 16, WidthRequest = 200, LineBreakMode = LineBreakMode.NoWrap, Margin = new Thickness(30,0,0,0), HorizontalTextAlignment = TextAlignment.Center};
            Label sendDate = new Label {FontSize = 16, WidthRequest = 200, LineBreakMode = LineBreakMode.NoWrap, Margin = new Thickness(30,0,0,0), HorizontalTextAlignment = TextAlignment.Center};
            sender.SetBinding(Label.TextProperty, "Sender.FIO");
            title.SetBinding(Label.TextProperty, "UserEmailTitle");
            sendDate.SetBinding(Label.TextProperty, "SendDate");
            Mail.ItemTemplate = new DataTemplate(() =>
            {
                ViewCell cell = new ViewCell();
                var stack = new HorizontalStackLayout { Children = {sender, title, sendDate} };
                var tap = new TapGestureRecognizer();
                tap.Tapped += (s, e) => 
                {
                    var context = (s as HorizontalStackLayout).BindingContext as UserMail;
                    Navigation.PushAsync(new FullMailViewPage(context));
                };
                stack.GestureRecognizers.Add(tap);
                cell.View = stack;
                return cell;
            });
        }
        private void SetInputMailHeader()
        {
            HorizontalStackLayout stack = new();
            stack.Add(new Label {FontSize = 16, WidthRequest = 200, Text="От", Margin = new Thickness(30,0,0,0), HorizontalTextAlignment = TextAlignment.Center});
            stack.Add(new Label {FontSize = 16, WidthRequest = 200, Text="Тема", Margin = new Thickness(30,0,0,0), HorizontalTextAlignment = TextAlignment.Center});
            stack.Add(new Label {FontSize = 16, WidthRequest = 200, Text="Дата отправки", Margin = new Thickness(30,0,0,0), HorizontalTextAlignment = TextAlignment.Center});
            Mail.Header = stack;
        }

        private void SetOutputMailDataTemplate()
        {
            Label fio = new Label {FontSize = 16, WidthRequest = 200, LineBreakMode = LineBreakMode.NoWrap, Margin = new Thickness(30,0,0,0), HorizontalTextAlignment = TextAlignment.Center};
            Label theme = new Label {FontSize = 16, WidthRequest = 200, LineBreakMode = LineBreakMode.NoWrap, Margin = new Thickness(30,0,0,0), HorizontalTextAlignment = TextAlignment.Center};
            Label date = new Label {FontSize = 16, WidthRequest = 200, LineBreakMode = LineBreakMode.NoWrap, Margin = new Thickness(30,0,0,0), HorizontalTextAlignment = TextAlignment.Center};
            fio.SetBinding(Label.TextProperty, "Getter.FIO");
            theme.SetBinding(Label.TextProperty, "UserEmailTitle");
            date.SetBinding(Label.TextProperty, "SendDate");
            Mail.ItemTemplate = new DataTemplate(() =>
            {
                ViewCell cell = new ViewCell();
                var stack = new HorizontalStackLayout { Children = {fio, theme, date} };
                var tap = new TapGestureRecognizer();
                tap.Tapped += (s, e) => 
                {
                    var context = (s as HorizontalStackLayout).BindingContext as UserMail;
                    Navigation.PushAsync(new FullMailViewPage(context));
                };
                stack.GestureRecognizers.Add(tap);
                cell.View = stack;
                return cell;
            });
        }

        private void SetOutputMailHeader()
        {
            HorizontalStackLayout stack = new();
            stack.Add(new Label {FontSize = 16, WidthRequest = 200, Text="Кому", Margin = new Thickness(30,0,0,0), HorizontalTextAlignment = TextAlignment.Center});
            stack.Add(new Label {FontSize = 16, WidthRequest = 200, Text="Тема", Margin = new Thickness(30,0,0,0), HorizontalTextAlignment = TextAlignment.Center});
            stack.Add(new Label {FontSize = 16, WidthRequest = 200, Text="Дата отправки", Margin = new Thickness(30,0,0,0), HorizontalTextAlignment = TextAlignment.Center});
            Mail.Header = stack;
        }

        private void SetDocumentsDataTemplate()
        {
            Label docName = new Label {FontSize = 16, WidthRequest = 200, LineBreakMode = LineBreakMode.NoWrap, Margin = new Thickness(30,0,0,0), HorizontalTextAlignment = TextAlignment.Center, VerticalTextAlignment= TextAlignment.Center};
            Label docStat = new Label {FontSize = 16, WidthRequest = 200, LineBreakMode = LineBreakMode.NoWrap, Margin = new Thickness(30,0,0,0), HorizontalTextAlignment = TextAlignment.Center, VerticalTextAlignment= TextAlignment.Center};
            Label date = new Label {FontSize = 16, WidthRequest = 200, LineBreakMode = LineBreakMode.NoWrap, Margin = new Thickness(30,0,0,0), HorizontalTextAlignment = TextAlignment.Center, VerticalTextAlignment= TextAlignment.Center};
            docName.SetBinding(Label.TextProperty, "DocumentName");
            docStat.SetBinding(Label.TextProperty, "documentStatus.DocumentStatusName");
            date.SetBinding(Label.TextProperty, "CreationDate");
            Mail.ItemTemplate = new DataTemplate(() =>
            {
                ViewCell cell = new ViewCell();
                var stack = new HorizontalStackLayout { Children = {docName, docStat, date} };
                MenuFlyout menuElements = new MenuFlyout();
                MenuFlyoutItem download = new MenuFlyoutItem() { Text = "Скачать" };
                MenuFlyoutItem delete = new MenuFlyoutItem() { Text = "Удалить" };
                download.Clicked += DownloadDoc;
                delete.Clicked += DeleteDoc;
                menuElements.Add(download);
                menuElements.Add(delete);
                FlyoutBase.SetContextFlyout(stack, menuElements);
                cell.View = stack;
                return cell;
            });
        }

        private void SetDocumentsHeader()
        {
            HorizontalStackLayout stack = new();
            stack.Add(new Label {FontSize = 16, WidthRequest = 200, Text="Название", Margin = new Thickness(30,0,0,0), HorizontalTextAlignment = TextAlignment.Center});
            stack.Add(new Label {FontSize = 16, WidthRequest = 200, Text="Статус", Margin = new Thickness(30,0,0,0), HorizontalTextAlignment = TextAlignment.Center});
            stack.Add(new Label {FontSize = 16, WidthRequest = 200, Text="Дата создания", Margin = new Thickness(30,0,0,0), HorizontalTextAlignment = TextAlignment.Center});
            Mail.Header = stack;
        }

        public AdminPage(User user)
        {
            curUser = user;
            InitializeComponent();
        }

        private void GetInputMail()
        {
            try
            {
                using AppContext db = new();
                var mail = db.UsersMails.Include("Sender").Include("Getter").Include("AttachedDocuments").Where(x => x.Getter.UserId == curUser.UserId).ToList();
                if (mail != null)
                {
                    Mail.ItemsSource = mail;
                    SetInputMailHeader();
                    SetInputMailDataTemplate();
                }
            }
            catch (Exception ex) {}
        }

        private void GetOutputMail()
        {
            try
            {
                using AppContext db = new();
                var mail = db.UsersMails.Include("Sender").Include("Getter").Include("AttachedDocuments").Where(x => x.Sender.UserId == curUser.UserId).ToList();
                if (mail != null)
                {
                    Mail.ItemsSource = mail;
                    SetOutputMailHeader();
                    SetOutputMailDataTemplate();
                }
            }
            catch (Exception ex) {}
        }

        private void GetDocs()
        {
            try
            {
                using AppContext db = new();
                var docs = db.Documents.Include("Creator").Include("documentStatus").ToList();
                if (docs != null)
                {
                    Mail.ItemsSource = docs;
                    SetDocumentsHeader();
                    SetDocumentsDataTemplate();
                }
            }
            catch (Exception ex) {}
        }

        private void LoadInputMail(object sender, EventArgs e)
        {
            GetInputMail();
        }

        private void LoadOutputMail(object sender, EventArgs e)
        {
            GetOutputMail();
        }

        private void LoadDocuments(object sender, EventArgs e)
        {
            GetDocs();
        }

        private void LoadArchiveDocuments(object sender, EventArgs e)
        {
            
        }

        private void LoadDeletedDocuments(object sender, EventArgs e)
        {
            
        }

        private async void LoadDoc(object sender, EventArgs e)
        {
            using AppContext db = new();
            var result = await FilePicker.Default.PickAsync();
            if (result != null)
            {   
                var status = db.DocumentStatuses.First();
                var creator = db.Users.First(x => x.UserId == curUser.UserId);
                var priv = db.Roles.First();
                using var stream = await result.OpenReadAsync();
                using MemoryStream ms = new MemoryStream();
                byte[] data = new byte[stream.Length];
                stream.CopyTo(ms);
                data = ms.ToArray();
                db.Documents.Add(Document.Of(result.FileName, data, status, creator, priv));
                db.SaveChanges();
            }
        }

        private async void DownloadDoc(object sender, EventArgs e)
        {
            try
            {
                var context = (sender as MenuFlyoutItem).BindingContext as Document;
                using AppContext db = new();
                var file = db.Documents.First(x => x.DocumentId == context.DocumentId);
                if (file != null)
                {
                    string path = Environment.GetEnvironmentVariable("USERPROFILE") + @"\" + "Downloads" + @"\" + file.DocumentName;
                    using var stream = File.Create(path);
                    stream.Write(file.DocumentData, 0, file.DocumentData.Length);
                    await DisplayAlert("Успех", "Файл скачан", "Ок");
                }
            }
            catch(Exception ex)
            {
                await DisplayAlert("Ошибка", ex.Message, "Ок");
            }
        }

        private async void DeleteDoc(object sender, EventArgs e)
        {
            try
            {
                using AppContext db = new();
                var context = (sender as MenuFlyoutItem).BindingContext as Document;
                if (context.Creator.UserId == curUser.UserId || curUser.UserRole.RoleName == "админ")
                {
                    var del = db.Documents.First(x => x.DocumentId == context.DocumentId);
                    var doc = del as Docs;
                    var deldoc = doc as DeletedDocument;
                }
                
            }
            catch(Exception ex)
            {
                await DisplayAlert("Ошибка", ex.Message, "Ок");
            }
        }
    }
}