using System.Diagnostics;
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
            Mail.ItemTemplate = new DataTemplate(() =>
            {
                ViewCell cell = new ViewCell();
                Label sender = new Label { FontSize = 16, WidthRequest = 200, LineBreakMode = LineBreakMode.NoWrap, Margin = new Thickness(30, 0, 0, 0), HorizontalTextAlignment = TextAlignment.Center };
                Label title = new Label { FontSize = 16, WidthRequest = 200, LineBreakMode = LineBreakMode.NoWrap, Margin = new Thickness(30, 0, 0, 0), HorizontalTextAlignment = TextAlignment.Center };
                Label sendDate = new Label { FontSize = 16, WidthRequest = 200, LineBreakMode = LineBreakMode.NoWrap, Margin = new Thickness(30, 0, 0, 0), HorizontalTextAlignment = TextAlignment.Center };
                sender.SetBinding(Label.TextProperty, "Sender.FIO");
                title.SetBinding(Label.TextProperty, "UserEmailTitle");
                sendDate.SetBinding(Label.TextProperty, "SendDate");
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
            Mail.ItemTemplate = new DataTemplate(() =>
            {
                ViewCell cell = new ViewCell();
                Label fio = new Label { FontSize = 16, WidthRequest = 200, LineBreakMode = LineBreakMode.NoWrap, Margin = new Thickness(30, 0, 0, 0), HorizontalTextAlignment = TextAlignment.Center };
                Label theme = new Label { FontSize = 16, WidthRequest = 200, LineBreakMode = LineBreakMode.NoWrap, Margin = new Thickness(30, 0, 0, 0), HorizontalTextAlignment = TextAlignment.Center };
                Label date = new Label { FontSize = 16, WidthRequest = 200, LineBreakMode = LineBreakMode.NoWrap, Margin = new Thickness(30, 0, 0, 0), HorizontalTextAlignment = TextAlignment.Center };
                fio.SetBinding(Label.TextProperty, "Getter.FIO");
                theme.SetBinding(Label.TextProperty, "UserEmailTitle");
                date.SetBinding(Label.TextProperty, "SendDate");
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
            Mail.ItemTemplate = new DataTemplate(() =>
            {
                ViewCell cell = new ViewCell();
                Label docName = new Label { FontSize = 16, WidthRequest = 200, LineBreakMode = LineBreakMode.NoWrap, Margin = new Thickness(30, 0, 0, 0), HorizontalTextAlignment = TextAlignment.Center, VerticalTextAlignment = TextAlignment.Center };
                Label docStat = new Label { FontSize = 16, WidthRequest = 200, LineBreakMode = LineBreakMode.NoWrap, Margin = new Thickness(30, 0, 0, 0), HorizontalTextAlignment = TextAlignment.Center, VerticalTextAlignment = TextAlignment.Center };
                Label date = new Label { FontSize = 16, WidthRequest = 200, LineBreakMode = LineBreakMode.NoWrap, Margin = new Thickness(30, 0, 0, 0), HorizontalTextAlignment = TextAlignment.Center, VerticalTextAlignment = TextAlignment.Center };
                docName.SetBinding(Label.TextProperty, "DocumentName");
                docStat.SetBinding(Label.TextProperty, "documentStatus.DocumentStatusName");
                date.SetBinding(Label.TextProperty, "CreationDate");
                var stack = new HorizontalStackLayout { Children = {docName, docStat, date} };
                MenuFlyout menuElements = new MenuFlyout();
                MenuFlyoutItem download = new MenuFlyoutItem() { Text = "Скачать" };
                MenuFlyoutItem delete = new MenuFlyoutItem() { Text = "Удалить" };
                MenuFlyoutItem archive = new MenuFlyoutItem() {Text = "В архив"};
                download.Clicked += DownloadDoc;
                delete.Clicked += DeleteDoc;
                archive.Clicked += SendToArchive;
                menuElements.Add(download);
                menuElements.Add(delete);
                menuElements.Add(archive);
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

        private void SetDelDocumentsDataTemplate()
        {
            Mail.ItemTemplate = new DataTemplate(() =>
            {
                ViewCell cell = new ViewCell();
                Label docName = new Label { FontSize = 16, WidthRequest = 200, LineBreakMode = LineBreakMode.NoWrap, Margin = new Thickness(30, 0, 0, 0), HorizontalTextAlignment = TextAlignment.Center, VerticalTextAlignment = TextAlignment.Center };
                Label docStat = new Label { FontSize = 16, WidthRequest = 200, LineBreakMode = LineBreakMode.NoWrap, Margin = new Thickness(30, 0, 0, 0), HorizontalTextAlignment = TextAlignment.Center, VerticalTextAlignment = TextAlignment.Center };
                Label date = new Label { FontSize = 16, WidthRequest = 200, LineBreakMode = LineBreakMode.NoWrap, Margin = new Thickness(30, 0, 0, 0), HorizontalTextAlignment = TextAlignment.Center, VerticalTextAlignment = TextAlignment.Center };
                docName.SetBinding(Label.TextProperty, "DeletedDocumentName");
                docStat.SetBinding(Label.TextProperty, "Creator.FIO");
                date.SetBinding(Label.TextProperty, "CreationDate");
                var stack = new HorizontalStackLayout { Children = {docName, docStat, date} };
                MenuFlyout menuElements = new MenuFlyout();
                MenuFlyoutItem reset = new MenuFlyoutItem() { Text = "Восстановить" };
                MenuFlyoutItem delete = new MenuFlyoutItem() { Text = "Удалить" };
                reset.Clicked += RestoreDoc;
                delete.Clicked += FullDeleteDoc;
                menuElements.Add(reset);
                menuElements.Add(delete);
                FlyoutBase.SetContextFlyout(stack, menuElements);
                cell.View = stack;
                return cell;
            });
        }

        private void SetDelDocumentsHeader()
        {
            HorizontalStackLayout stack = new();
            stack.Add(new Label {FontSize = 16, WidthRequest = 200, Text="Название", Margin = new Thickness(30,0,0,0), HorizontalTextAlignment = TextAlignment.Center});
            stack.Add(new Label {FontSize = 16, WidthRequest = 200, Text="Удаливший", Margin = new Thickness(30,0,0,0), HorizontalTextAlignment = TextAlignment.Center});
            stack.Add(new Label {FontSize = 16, WidthRequest = 200, Text="Дата удаления", Margin = new Thickness(30,0,0,0), HorizontalTextAlignment = TextAlignment.Center});
            Mail.Header = stack;
        }

        private void SetArcDocumentsDataTemplate()
        {
            Mail.ItemTemplate = new DataTemplate(() =>
            {
                ViewCell cell = new ViewCell();
                Label docName = new Label { FontSize = 16, WidthRequest = 200, LineBreakMode = LineBreakMode.NoWrap, Margin = new Thickness(30, 0, 0, 0), HorizontalTextAlignment = TextAlignment.Center, VerticalTextAlignment = TextAlignment.Center };
                Label docStat = new Label { FontSize = 16, WidthRequest = 200, LineBreakMode = LineBreakMode.NoWrap, Margin = new Thickness(30, 0, 0, 0), HorizontalTextAlignment = TextAlignment.Center, VerticalTextAlignment = TextAlignment.Center };
                Label date = new Label { FontSize = 16, WidthRequest = 200, LineBreakMode = LineBreakMode.NoWrap, Margin = new Thickness(30, 0, 0, 0), HorizontalTextAlignment = TextAlignment.Center, VerticalTextAlignment = TextAlignment.Center };
                docName.SetBinding(Label.TextProperty, "ArchiveDocumentName");
                docStat.SetBinding(Label.TextProperty, "Creator.FIO");
                date.SetBinding(Label.TextProperty, "CreationDate");
                var stack = new HorizontalStackLayout { Children = {docName, docStat, date} };
                MenuFlyout menuElements = new MenuFlyout();
                MenuFlyoutItem download = new MenuFlyoutItem() { Text = "Скачать" };
                download.Clicked += DownloadDoc;
                menuElements.Add(download);
                FlyoutBase.SetContextFlyout(stack, menuElements);
                cell.View = stack;
                return cell;
            });
        }

        private void SetArcDocumentsHeader()
        {
            HorizontalStackLayout stack = new();
            stack.Add(new Label {FontSize = 16, WidthRequest = 200, Text="Название", Margin = new Thickness(30,0,0,0), HorizontalTextAlignment = TextAlignment.Center});
            stack.Add(new Label {FontSize = 16, WidthRequest = 200, Text="Отправил в архив", Margin = new Thickness(30,0,0,0), HorizontalTextAlignment = TextAlignment.Center});
            stack.Add(new Label {FontSize = 16, WidthRequest = 200, Text="Дата архивации", Margin = new Thickness(30,0,0,0), HorizontalTextAlignment = TextAlignment.Center});
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
                    SetInputMailHeader();
                    SetInputMailDataTemplate();
                    Mail.ItemsSource = mail;
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
                    SetOutputMailHeader();
                    SetOutputMailDataTemplate();
                    Mail.ItemsSource = mail;
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
                    SetDocumentsHeader();
                    SetDocumentsDataTemplate();
                    Mail.ItemsSource = docs;
                }
            }
            catch (Exception ex) {}
        }

        private void GetDelDocs()
        {
            try
            {
                using AppContext db = new();
                var docs = db.DeletedDocuments.Include("Creator").Include("documentStatus").ToList();
                if (docs != null)
                {
                    Mail.ItemsSource = docs;
                    SetDelDocumentsHeader();
                    SetDelDocumentsDataTemplate();
                }
            }
            catch (Exception ex) {}
        }

        private void GetArcDocs()
        {
            try
            {
                using AppContext db = new();
                var docs = db.ArchiveDocuments.Include("Creator").Include("documentStatus").ToList();
                if (docs != null)
                {
                    Mail.ItemsSource = docs;
                    SetArcDocumentsHeader();
                    SetArcDocumentsDataTemplate();
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
            GetArcDocs();
        }

        private void LoadDeletedDocuments(object sender, EventArgs e)
        {
            GetDelDocs();
        }

        private async void LoadDoc(object sender, EventArgs e)
        {
            try
            {
                using AppContext db = new();
                var result = await FilePicker.Default.PickAsync();
                if (result != null)
                {
                    var status = db.DocumentStatuses.First(x => x.DocumentStatusName == "в работе");
                    var creator = db.Users.Include("UserRole").First(x => x.UserId == curUser.UserId);
                    var priv = creator.UserRole;
                    db.Documents.Add(Document.Of(result.FileName, File.ReadAllBytes(result.FullPath), status, creator, priv));
                    db.SaveChanges();
                    GetDocs();
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ошибка", ex.Message, "Ок");
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
                    using (var stream = File.Create(path))
                    {
                        stream.Write(file.DocumentData, 0, file.DocumentData.Length);
                    }
                    bool openFile = await DisplayAlert("Успех", "Файл скачан, открыть?", "Да", "Нет");
                    if (openFile)
                    {
                        OpenFileRequest openFileRequest = new OpenFileRequest();
                        openFileRequest.File = new ReadOnlyFile(path);
                        await Launcher.OpenAsync(openFileRequest);
                    }
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
                    if (await DisplayAlert("Подтвердить действие", "Вы хотите удалить документ?", "Да", "Нет"))
                    {
                        var del = db.Documents.Include("Creator").Include("documentStatus").Include("PrivateLevel").First(x => x.DocumentId == context.DocumentId);
                        var deletedStatus = db.DocumentStatuses.First(x => x.DocumentStatusName == "удален");
                        var deldoc = new DeletedDocument();
                        deldoc.CastFromDocument(del);
                        deldoc.documentStatus = deletedStatus;
                        db.DeletedDocuments.Add(deldoc);
                        db.Documents.Remove(del);
                        db.SaveChanges();
                        GetDocs();
                    }

                }
                
            }
            catch(Exception ex)
            {
                await DisplayAlert("Ошибка", ex.Message, "Ок");
            }
        }

        private async void RestoreDoc(object sender, EventArgs e)
        {
            try
            {
                using AppContext db = new();
                var context = (sender as MenuFlyoutItem).BindingContext as DeletedDocument;
                if (context.Creator.UserId == curUser.UserId || curUser.UserRole.RoleName == "админ")
                {
                    var deldoc = db.DeletedDocuments.Include("Creator").Include("documentStatus").Include("PrivateLevel").First(x => x.DeletedDocumentId == context.DeletedDocumentId);
                    var status = db.DocumentStatuses.First(x => x.DocumentStatusName == "восстановлен");
                    var doc = deldoc.CastToDocument();
                    doc.documentStatus = status;
                    db.DeletedDocuments.Remove(deldoc);
                    db.Documents.Add(doc);
                    db.SaveChanges();
                    GetDocs();
                }
                
            }
            catch(Exception ex)
            {
                await DisplayAlert("Ошибка", ex.Message, "Ок");
            }
        }

        private async void FullDeleteDoc(object sender, EventArgs e)
        {
            try
            {
                using AppContext db = new();
                var context = (sender as MenuFlyoutItem).BindingContext as DeletedDocument;
                if (context.Creator.UserId == curUser.UserId || curUser.UserRole.RoleName == "админ")
                {
                    if (await DisplayAlert("Подтвердить действие", "Вы хотите безвозвратно удалить документ?", "Да", "Нет"))
                    {
                        var deldoc = db.DeletedDocuments.Include("Creator").Include("documentStatus").Include("PrivateLevel").First(x => x.DeletedDocumentId == context.DeletedDocumentId);
                        db.DeletedDocuments.Remove(deldoc);
                        db.SaveChanges();
                        GetDelDocs();
                    }

                }
                
            }
            catch(Exception ex)
            {
                await DisplayAlert("Ошибка", ex.Message, "Ок");
            }
        }
    
        private async void SendToArchive(object sender, EventArgs e)
        {
            try
            {
                using AppContext db = new();
                var context = (sender as MenuFlyoutItem).BindingContext as Document;
                if (context.Creator.UserId == curUser.UserId || curUser.UserRole.RoleName == "админ")
                {
                    var doc = db.Documents.Include("Creator").Include("documentStatus").Include("PrivateLevel").First(x => x.DocumentId == context.DocumentId);
                    var status = db.DocumentStatuses.First(x => x.DocumentStatusName == "в архиве");
                    ArchiveDocument arc = new();
                    arc.CastFromDocument(doc);
                    db.ArchiveDocuments.Add(arc);
                    db.Documents.Remove(doc);
                    db.SaveChanges();
                    GetDocs();
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ошибка", ex.Message, "Ок");
            }
        }
    
    }
}