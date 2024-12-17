
using Diplom.Entities;
using Microsoft.EntityFrameworkCore;

namespace Diplom
{
    public partial class ManagerPage : ContentPage
    {

        #region private vars
        private User curUser;
        private ListType curListViewType = ListType.Input;
        #endregion
        public ManagerPage(User user)
        {
            InitializeComponent();
            curUser = user;
        }

        #region delegates for xaml
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
                    if (db.Documents.Any() && db.Documents.Where(x => x.DocumentName == result.FileName).Any())
                    {
                        await DisplayAlert("Ошибка", "Файл с таким названием уже существует", "Ок");
                    }
                    else
                    {
                        var status = db.DocumentStatuses.First(x => x.DocumentStatusName == "в работе");
                        var creator = db.Users.Include("UserRole").First(x => x.UserId == curUser.UserId);
                        var priv = creator.UserRole;
                        db.Documents.Add(Document.Of(result.FileName, File.ReadAllBytes(result.FullPath), status, creator, priv));
                        db.SaveChanges();
                        GetDocs();
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ошибка", ex.Message, "Ок");
            }
        }

        private async void SearchByListViewType(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(Searcher.Text))
                {
                    FillListByType(Searcher.Text);
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ошибка", ex.Message, "Ок");
            }
        }
        
        private void ClearSearchResults(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(Searcher.Text))
            {
                FillListByBaseData();
            }
        }
        
        private async void CreateMail(object sender, EventArgs e)
        {
            try
            {
                await Navigation.PushAsync(new NewMail(curUser));
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ошибка", ex.Message, "Ок");
            }
        }
        
        private async void OpenProfile(object sender, EventArgs e)
        {
            try
            {
                await Navigation.PushAsync(new Profile(curUser));
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ошибка", ex.Message, "Ок");
            }
        }
        
        #endregion
    
        #region get methods
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
                    curListViewType = ListType.Input;
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
                    curListViewType = ListType.Output;
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
                    curListViewType = ListType.Documents;
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
                    SetDelDocumentsHeader();
                    SetDelDocumentsDataTemplate();
                    Mail.ItemsSource = docs;
                    curListViewType = ListType.DeletedDocuments;
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
                    SetArcDocumentsHeader();
                    SetArcDocumentsDataTemplate();
                    Mail.ItemsSource = docs;
                    curListViewType = ListType.ArchiveDocuments;
                }
            }
            catch (Exception ex) {}
        }
        #endregion
    
        #region List view styles
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
            var sender = new Label {FontSize = 16, WidthRequest = 200, Text="От", Margin = new Thickness(30,0,0,0), HorizontalTextAlignment = TextAlignment.Center};
            var title = new Label {FontSize = 16, WidthRequest = 200, Text="Тема", Margin = new Thickness(30,0,0,0), HorizontalTextAlignment = TextAlignment.Center};
            var date = new Label {FontSize = 16, WidthRequest = 200, Text="Дата отправки", Margin = new Thickness(30,0,0,0), HorizontalTextAlignment = TextAlignment.Center};
            var tap = new TapGestureRecognizer();
            tap.Tapped += (s, e) =>
            {
                Label lbl = s as Label;
                HorizontalStackLayout parent = lbl.Parent as HorizontalStackLayout;
                var lbls = parent.Children.Where(x => x is Label).ToList();
                string asc = "↑";
                string desc = "↓";
                SortType sort = SortType.Descending;

                foreach(var lable in lbls)
                    {
                        var l = lable as Label;
                        if (l.Text.Contains(asc))
                        {
                            l.Text = l.Text.Remove(l.Text.IndexOf(asc));
                            if (l == lbl)
                            {
                                sort = SortType.Descending;
                            }
                        }
                        if (l.Text.Contains(desc))
                        {
                            l.Text = l.Text.Remove(l.Text.IndexOf(desc));
                            if (l == lbl)
                            {
                                sort = SortType.Ascending;
                            }
                        }
                    }
                
                var mail = Mail.ItemsSource as List<UserMail>;
                
                switch (lbl.Text)
                {
                    case "От":
                        if (sort == SortType.Ascending)
                        {
                            Mail.ItemsSource = mail.OrderBy(x => x.Sender).ToList();
                            lbl.Text += asc;
                        }
                        else
                        {
                            Mail.ItemsSource = mail.OrderByDescending(x => x.Sender).ToList();
                            lbl.Text = lbl.Text + desc;
                        }
                        break;
                    case "Тема":
                        if (sort == SortType.Ascending)
                        {
                            Mail.ItemsSource = mail.OrderBy(x => x.UserEmailTitle).ToList();
                            lbl.Text += asc;
                        }
                        else
                        {
                            Mail.ItemsSource = mail.OrderByDescending(x => x.UserEmailTitle).ToList();
                            lbl.Text = lbl.Text + desc;
                        }
                        break;
                    case "Дата отправки":
                        if (sort == SortType.Ascending)
                        {
                            Mail.ItemsSource = mail.OrderBy(x => x.SendDate).ToList();
                            lbl.Text += asc;
                        }
                        else
                        {
                            Mail.ItemsSource = mail.OrderByDescending(x => x.SendDate).ToList();
                            lbl.Text = lbl.Text + desc;
                        }
                        break;
                }

            };
            sender.GestureRecognizers.Add(tap);
            title.GestureRecognizers.Add(tap);
            date.GestureRecognizers.Add(tap);
            stack.Add(sender);
            stack.Add(title);
            stack.Add(date);
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
            var getter = new Label {FontSize = 16, WidthRequest = 200, Text="Кому", Margin = new Thickness(30,0,0,0), HorizontalTextAlignment = TextAlignment.Center};
            var title = new Label {FontSize = 16, WidthRequest = 200, Text="Тема", Margin = new Thickness(30,0,0,0), HorizontalTextAlignment = TextAlignment.Center};
            var date = new Label {FontSize = 16, WidthRequest = 200, Text="Дата отправки", Margin = new Thickness(30,0,0,0), HorizontalTextAlignment = TextAlignment.Center};
            var tap = new TapGestureRecognizer();
            tap.Tapped += (s, e) =>
            {
                Label lbl = s as Label;
                HorizontalStackLayout parent = lbl.Parent as HorizontalStackLayout;
                var lbls = parent.Children.Where(x => x is Label).ToList();
                string asc = "↑";
                string desc = "↓";
                SortType sort = SortType.Descending;

                foreach(var lable in lbls)
                    {
                        var l = lable as Label;
                        if (l.Text.Contains(asc))
                        {
                            l.Text = l.Text.Remove(l.Text.IndexOf(asc));
                            if (l == lbl)
                            {
                                sort = SortType.Descending;
                            }
                        }
                        if (l.Text.Contains(desc))
                        {
                            l.Text = l.Text.Remove(l.Text.IndexOf(desc));
                            if (l == lbl)
                            {
                                sort = SortType.Ascending;
                            }
                        }
                    }
                
                var mail = Mail.ItemsSource as List<UserMail>;
                
                switch (lbl.Text)
                {
                    case "Кому":
                        if (sort == SortType.Ascending)
                        {
                            Mail.ItemsSource = mail.OrderBy(x => x.Getter).ToList();
                            lbl.Text += asc;
                        }
                        else
                        {
                            Mail.ItemsSource = mail.OrderByDescending(x => x.Getter).ToList();
                            lbl.Text = lbl.Text + desc;
                        }
                        break;
                    case "Тема":
                        if (sort == SortType.Ascending)
                        {
                            Mail.ItemsSource = mail.OrderBy(x => x.UserEmailTitle).ToList();
                            lbl.Text += asc;
                        }
                        else
                        {
                            Mail.ItemsSource = mail.OrderByDescending(x => x.UserEmailTitle).ToList();
                            lbl.Text = lbl.Text + desc;
                        }
                        break;
                    case "Дата отправки":
                        if (sort == SortType.Ascending)
                        {
                            Mail.ItemsSource = mail.OrderBy(x => x.SendDate).ToList();
                            lbl.Text += asc;
                        }
                        else
                        {
                            Mail.ItemsSource = mail.OrderByDescending(x => x.SendDate).ToList();
                            lbl.Text = lbl.Text + desc;
                        }
                        break;
                }

            };
            getter.GestureRecognizers.Add(tap);
            title.GestureRecognizers.Add(tap);
            date.GestureRecognizers.Add(tap);
            stack.Add(getter);
            stack.Add(title);
            stack.Add(date);
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
            var name = new Label {FontSize = 16, WidthRequest = 200, Text="Название", Margin = new Thickness(30,0,0,0), HorizontalTextAlignment = TextAlignment.Center};
            var status = new Label {FontSize = 16, WidthRequest = 200, Text="Статус", Margin = new Thickness(30,0,0,0), HorizontalTextAlignment = TextAlignment.Center};
            var date = new Label {FontSize = 16, WidthRequest = 200, Text="Дата создания", Margin = new Thickness(30,0,0,0), HorizontalTextAlignment = TextAlignment.Center};
            var tap = new TapGestureRecognizer();
            tap.Tapped += (s, e) =>
            {
                Label lbl = s as Label;
                HorizontalStackLayout parent = lbl.Parent as HorizontalStackLayout;
                var lbls = parent.Children.Where(x => x is Label).ToList();
                string asc = "↑";
                string desc = "↓";
                SortType sort = SortType.Descending;

                foreach(var lable in lbls)
                    {
                        var l = lable as Label;
                        if (l.Text.Contains(asc))
                        {
                            l.Text = l.Text.Remove(l.Text.IndexOf(asc));
                            if (l == lbl)
                            {
                                sort = SortType.Descending;
                            }
                        }
                        if (l.Text.Contains(desc))
                        {
                            l.Text = l.Text.Remove(l.Text.IndexOf(desc));
                            if (l == lbl)
                            {
                                sort = SortType.Ascending;
                            }
                        }
                    }
                
                var mail = Mail.ItemsSource as List<Document>;
                
                switch (lbl.Text)
                {
                    case "Название":
                        if (sort == SortType.Ascending)
                        {
                            Mail.ItemsSource = mail.OrderBy(x => x.DocumentName).ToList();
                            lbl.Text += asc;
                        }
                        else
                        {
                            Mail.ItemsSource = mail.OrderByDescending(x => x.DocumentName).ToList();
                            lbl.Text = lbl.Text + desc;
                        }
                        break;
                    case "Статус":
                        if (sort == SortType.Ascending)
                        {
                            Mail.ItemsSource = mail.OrderBy(x => x.documentStatus).ToList();
                            lbl.Text += asc;
                        }
                        else
                        {
                            Mail.ItemsSource = mail.OrderByDescending(x => x.documentStatus).ToList();
                            lbl.Text = lbl.Text + desc;
                        }
                        break;
                    case "Дата создания":
                        if (sort == SortType.Ascending)
                        {
                            Mail.ItemsSource = mail.OrderBy(x => x.CreationDate).ToList();
                            lbl.Text += asc;
                        }
                        else
                        {
                            Mail.ItemsSource = mail.OrderByDescending(x => x.CreationDate).ToList();
                            lbl.Text = lbl.Text + desc;
                        }
                        break;
                }

            };
            name.GestureRecognizers.Add(tap);
            status.GestureRecognizers.Add(tap);
            date.GestureRecognizers.Add(tap);
            stack.Add(name);
            stack.Add(status);
            stack.Add(date);
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
            var name = new Label {FontSize = 16, WidthRequest = 200, Text="Название", Margin = new Thickness(30,0,0,0), HorizontalTextAlignment = TextAlignment.Center};
            var deleter = new Label {FontSize = 16, WidthRequest = 200, Text="Удаливший", Margin = new Thickness(30,0,0,0), HorizontalTextAlignment = TextAlignment.Center};
            var date = new Label {FontSize = 16, WidthRequest = 200, Text="Дата удаления", Margin = new Thickness(30,0,0,0), HorizontalTextAlignment = TextAlignment.Center};
            var tap = new TapGestureRecognizer();
            tap.Tapped += (s, e) =>
            {
                Label lbl = s as Label;
                HorizontalStackLayout parent = lbl.Parent as HorizontalStackLayout;
                var lbls = parent.Children.Where(x => x is Label).ToList();
                string asc = "↑";
                string desc = "↓";
                SortType sort = SortType.Descending;

                foreach(var lable in lbls)
                    {
                        var l = lable as Label;
                        if (l.Text.Contains(asc))
                        {
                            l.Text = l.Text.Remove(l.Text.IndexOf(asc));
                            if (l == lbl)
                            {
                                sort = SortType.Descending;
                            }
                        }
                        if (l.Text.Contains(desc))
                        {
                            l.Text = l.Text.Remove(l.Text.IndexOf(desc));
                            if (l == lbl)
                            {
                                sort = SortType.Ascending;
                            }
                        }
                    }
                
                var mail = Mail.ItemsSource as List<DeletedDocument>;
                
                switch (lbl.Text)
                {
                    case "Название":
                        if (sort == SortType.Ascending)
                        {
                            Mail.ItemsSource = mail.OrderBy(x => x.DeletedDocumentName).ToList();
                            lbl.Text += asc;
                        }
                        else
                        {
                            Mail.ItemsSource = mail.OrderByDescending(x => x.DeletedDocumentName).ToList();
                            lbl.Text = lbl.Text + desc;
                        }
                        break;
                    case "Удаливший":
                        if (sort == SortType.Ascending)
                        {
                            Mail.ItemsSource = mail.OrderBy(x => x.Creator).ToList();
                            lbl.Text += asc;
                        }
                        else
                        {
                            Mail.ItemsSource = mail.OrderByDescending(x => x.Creator).ToList();
                            lbl.Text = lbl.Text + desc;
                        }
                        break;
                    case "Дата удаления":
                        if (sort == SortType.Ascending)
                        {
                            Mail.ItemsSource = mail.OrderBy(x => x.CreationDate).ToList();
                            lbl.Text += asc;
                        }
                        else
                        {
                            Mail.ItemsSource = mail.OrderByDescending(x => x.CreationDate).ToList();
                            lbl.Text = lbl.Text + desc;
                        }
                        break;
                }

            };
            name.GestureRecognizers.Add(tap);
            deleter.GestureRecognizers.Add(tap);
            date.GestureRecognizers.Add(tap);
            stack.Add(name);
            stack.Add(deleter);
            stack.Add(date);
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
            var name = new Label {FontSize = 16, WidthRequest = 200, Text="Название", Margin = new Thickness(30,0,0,0), HorizontalTextAlignment = TextAlignment.Center};
            var archiver = new Label {FontSize = 16, WidthRequest = 200, Text="Отправил в архив", Margin = new Thickness(30,0,0,0), HorizontalTextAlignment = TextAlignment.Center};
            var date = new Label {FontSize = 16, WidthRequest = 200, Text="Дата архивации", Margin = new Thickness(30,0,0,0), HorizontalTextAlignment = TextAlignment.Center};
            var tap = new TapGestureRecognizer();
            tap.Tapped += async (s, e) =>
            {
                Label lbl = s as Label;
                HorizontalStackLayout parent = lbl.Parent as HorizontalStackLayout;
                var lbls = parent.Children.Where(x => x is Label).ToList();
                string asc = "↑";
                string desc = "↓";
                SortType sort = SortType.Descending;

                foreach(var lable in lbls)
                    {
                        var l = lable as Label;
                        if (l.Text.Contains(asc))
                        {
                            l.Text = l.Text.Remove(l.Text.IndexOf(asc));
                            if (l == lbl)
                            {
                                sort = SortType.Descending;
                            }
                        }
                        if (l.Text.Contains(desc))
                        {
                            l.Text = l.Text.Remove(l.Text.IndexOf(desc));
                            if (l == lbl)
                            {
                                sort = SortType.Ascending;
                            }
                        }
                    }
                
                var mail = Mail.ItemsSource as List<ArchiveDocument>;

                switch (lbl.Text)
                {
                    case "Название":
                        if (sort == SortType.Ascending)
                        {
                            Mail.ItemsSource = mail.OrderBy(x => x.ArchiveDocumentName).ToList();
                            lbl.Text += asc;
                        }
                        else
                        {
                            Mail.ItemsSource = mail.OrderByDescending(x => x.ArchiveDocumentName).ToList();
                            lbl.Text = lbl.Text + desc;
                        }
                        break;
                    case "Отправил в архив":
                        if (sort == SortType.Ascending)
                        {
                            Mail.ItemsSource = mail.OrderBy(x => x.Creator).ToList();
                            lbl.Text += asc;
                        }
                        else
                        {
                            Mail.ItemsSource = mail.OrderByDescending(x => x.Creator).ToList();
                            lbl.Text = lbl.Text + desc;
                        }
                        break;
                    case "Дата архивации":
                        if (sort == SortType.Ascending)
                        {
                            Mail.ItemsSource = mail.OrderBy(x => x.CreationDate).ToList();
                            lbl.Text += asc;
                        }
                        else
                        {
                            Mail.ItemsSource = mail.OrderByDescending(x => x.CreationDate).ToList();
                            lbl.Text = lbl.Text + desc;
                        }
                        break;
                }

            };
            name.GestureRecognizers.Add(tap);
            archiver.GestureRecognizers.Add(tap);
            date.GestureRecognizers.Add(tap);
            stack.Add(name);
            stack.Add(archiver);
            stack.Add(date);
            Mail.Header = stack;
        }
        #endregion
    
        #region Context actions

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
                else
                {
                    await DisplayAlert("Ошибка", "Недостаточно прав для удаления. (Вы можете удалять только созданные вами документы)", "Ок");
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

        #endregion
    
        #region other
        // Надо дописывать каждый новый тип для listview, этот метод для поиска, а следующий для заполнения данными
         private void FillListByType(string query)
        {
            using AppContext db = new();
            query = query.ToLower();
            DateTime date;
            DateTime.TryParse(query, out date);
            switch (curListViewType)
            {
                case ListType.Input:
                    {
                        var mail = db.UsersMails.Include("Sender").Include("Getter").Where(x => x.Getter.UserId == curUser.UserId).ToList();
                        List<UserMail> res = mail.Where(x => x.Getter.FIO.ToLower().Contains(query) ||
                        x.UserEmailTitle.ToLower().Contains(query)).ToList();
                        if (res.Count == 0)
                        {
                            res = mail.Where(x => x.SendDate.Date == date.Date).ToList();
                        }
                        SetInputMailDataTemplate();
                        Mail.ItemsSource = res;
                    }
                    break;
                case ListType.Output:
                    {
                        var mail = db.UsersMails.Include("Sender").Include("Getter").Where(x => x.Sender.UserId == curUser.UserId).ToList();
                        List<UserMail> res = mail.Where(x => x.Sender.FIO.ToLower().Contains(query) ||
                        x.UserEmailTitle.ToLower().Contains(query)).ToList();
                        if (res.Count == 0)
                        {
                            res = mail.Where(x => x.SendDate.Date == date.Date).ToList();
                        }
                        SetOutputMailDataTemplate();
                        Mail.ItemsSource = res;
                    }
                    break;
                case ListType.Documents:
                    {
                        var docs = db.Documents.Include("Creator").Include("documentStatus").ToList();
                        List<Document> res = docs.Where(x => x.DocumentName.ToLower().Contains(query)).ToList();
                        if (res.Count == 0)
                        {
                            res = docs.Where(x => x.CreationDate.Date == date.Date).ToList();
                        }
                        SetDocumentsDataTemplate();
                        Mail.ItemsSource = res;
                    }
                    break;
                case ListType.ArchiveDocuments:
                    {
                        var docs = db.ArchiveDocuments.Include("Creator").Include("documentStatus").ToList();
                        List<ArchiveDocument> res = docs.Where(x => x.ArchiveDocumentName.ToLower().Contains(query)).ToList();
                        if (res.Count == 0)
                        {
                            res = docs.Where(x => x.CreationDate.Date == date.Date).ToList();
                        }
                        SetArcDocumentsDataTemplate();
                        Mail.ItemsSource = res;
                    }
                    break;
                case ListType.DeletedDocuments:
                    {
                        var docs = db.DeletedDocuments.Include("Creator").Include("documentStatus").ToList();
                        List<DeletedDocument> res = docs.Where(x => x.DeletedDocumentName.ToLower().Contains(query)).ToList();
                        if (res.Count == 0)
                        {
                            res = docs.Where(x => x.CreationDate.Date == date.Date).ToList();
                        }
                        SetDelDocumentsDataTemplate();
                        Mail.ItemsSource = res;
                    }
                    break;
            }
        }

        private void FillListByBaseData()
        {
            switch(curListViewType)
            {
                case ListType.ArchiveDocuments:
                    GetArcDocs();
                    break;
                case ListType.DeletedDocuments:
                    GetDelDocs();
                    break;
                case ListType.Documents:
                    GetDocs();
                    break;
                case ListType.Input:
                    GetInputMail();
                    break;
                case ListType.Output:
                    GetOutputMail();
                    break;
            }
        }

        #endregion
    }
}