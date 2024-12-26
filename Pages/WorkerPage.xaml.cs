
using Diplom.Entities;
using Microsoft.EntityFrameworkCore;

namespace Diplom
{
    public partial class WorkerPage : ContentPage
    {
        #region private vars
        private User curUser;
        private ListType curListViewType = ListType.Input;
        #endregion
        public WorkerPage(User user)
        {
            InitializeComponent();
            curUser = user;
        }

        #region delegates for xaml
         private async void LoadFavourite(object sender, EventArgs e)
        {
            try
            {
                GetFav();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ошибка", ex.Message, "Ок");
            }
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
                        db.Documents.Add(Document.Of(result.FileName, File.ReadAllBytes(result.FullPath), status, creator));
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
         private async void GetFav()
        {
            try
            {
                List<Favorite> favorites = new();
                using AppContext db = new();
                var favdocs = db.FavoriteDocuments.Include("FavoritedDocument").Where(x => x.FavoritedUser.UserId == curUser.UserId).ToList();
                var favmails = db.FavoriteMails.Include("FavoritedMail").Where(x => x.FavoritedUser.UserId == curUser.UserId).ToList();
                foreach (var fav in favdocs)
                {
                    favorites.Add(new Favorite() {FavoriteId = fav.FavoriteDocumentId, Document = fav.FavoritedDocument, Type = FavoriteType.Document});
                }
                foreach (var fav in favmails)
                {
                    favorites.Add(new Favorite() {FavoriteId = fav.FavoriteMailId, Mail = fav.FavoritedMail, Type = FavoriteType.Mail});
                }
                SetFavoriteHeader();
                SetFavoriteDataTemplate();
                Mail.ItemsSource = favorites;
                curListViewType = ListType.Favorite;
                
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ошибка", ex.Message, "Ок");
            }
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

        #endregion
    
        #region List view styles

        private async void SetFavoriteHeader()
        {
            try
            {
                HorizontalStackLayout stack = new();
            var name = new Label {FontSize = 16, WidthRequest = 200, Text="Название", Margin = new Thickness(90,0,0,0), HorizontalTextAlignment = TextAlignment.Center};
            var type = new Label {FontSize = 16, WidthRequest = 200, Text="Тип", Margin = new Thickness(30,0,0,0), HorizontalTextAlignment = TextAlignment.Center};
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
                
                var mail = Mail.ItemsSource as List<Favorite>;
                
                switch (lbl.Text)
                {
                    case "Название":
                        if (sort == SortType.Ascending)
                        {
                            Mail.ItemsSource = mail.OrderBy(x => x.GetNameString).ToList();
                            lbl.Text += asc;
                        }
                        else
                        {
                            Mail.ItemsSource = mail.OrderByDescending(x => x.GetNameString).ToList();
                            lbl.Text = lbl.Text + desc;
                        }
                        break;
                    case "Тип":
                        if (sort == SortType.Ascending)
                        {
                            Mail.ItemsSource = mail.OrderBy(x => x.GetTypeString).ToList();
                            lbl.Text += asc;
                        }
                        else
                        {
                            Mail.ItemsSource = mail.OrderByDescending(x => x.GetTypeString).ToList();
                            lbl.Text = lbl.Text + desc;
                        }
                        break;
                    case "Дата создания":
                        if (sort == SortType.Ascending)
                        {
                            Mail.ItemsSource = mail.OrderBy(x => x.GetCreationDateString).ToList();
                            lbl.Text += asc;
                        }
                        else
                        {
                            Mail.ItemsSource = mail.OrderByDescending(x => x.GetCreationDateString).ToList();
                            lbl.Text = lbl.Text + desc;
                        }
                        break;
                }

            };
            name.GestureRecognizers.Add(tap);
            type.GestureRecognizers.Add(tap);
            date.GestureRecognizers.Add(tap);
            stack.Add(name);
            stack.Add(type);
            stack.Add(date);
            Mail.Header = stack;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ошибка", ex.Message, "Ок");
            }
        }
        
        private async void SetFavoriteDataTemplate()
        {
            try
            {
                Mail.ItemTemplate = new DataTemplate(() =>
            {
                ViewCell cell = new ViewCell();
                Label favName = new Label { FontSize = 16, WidthRequest = 200, LineBreakMode = LineBreakMode.NoWrap, Margin = new Thickness(30, 0, 0, 0), HorizontalTextAlignment = TextAlignment.Center, VerticalTextAlignment = TextAlignment.Center };
                Label favType = new Label { FontSize = 16, WidthRequest = 200, LineBreakMode = LineBreakMode.NoWrap, Margin = new Thickness(30, 0, 0, 0), HorizontalTextAlignment = TextAlignment.Center, VerticalTextAlignment = TextAlignment.Center };
                Label date = new Label { FontSize = 16, WidthRequest = 200, LineBreakMode = LineBreakMode.NoWrap, Margin = new Thickness(30, 0, 0, 0), HorizontalTextAlignment = TextAlignment.Center, VerticalTextAlignment = TextAlignment.Center };
                Image fav = new Image() {Source="checked_star.png", WidthRequest = 30, HeightRequest = 30, Margin = new Thickness(30,0,0,0)};
                TapGestureRecognizer starTap = new();
                starTap.Tapped += (s, e) => 
                {
                    var star = s as Image;
                    var context = star.BindingContext as Favorite;
                    star.Source = "star.png";
                    RemoveFromFav(context);
                };
                fav.GestureRecognizers.Add(starTap);
                favName.SetBinding(Label.TextProperty, "GetNameString");
                favType.SetBinding(Label.TextProperty, "GetTypeString");
                date.SetBinding(Label.TextProperty, "GetCreationDateString");
                var stack = new HorizontalStackLayout { Children = {fav, favName, favType, date} };
                var tap = new TapGestureRecognizer();
                tap.Tapped += FavoriteInteract;
                stack.GestureRecognizers.Add(tap);
                cell.View = stack;
                return cell;
            });
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ошибка", ex.Message, "Ок");
            }
        }
        
        private void SetInputMailDataTemplate()
        {
            Mail.ItemTemplate = new DataTemplate(() =>
            {
                ViewCell cell = new ViewCell();
                Label sender = new Label { FontSize = 16, WidthRequest = 200, LineBreakMode = LineBreakMode.NoWrap, Margin = new Thickness(30, 0, 0, 0), HorizontalTextAlignment = TextAlignment.Center };
                Label title = new Label { FontSize = 16, WidthRequest = 200, LineBreakMode = LineBreakMode.NoWrap, Margin = new Thickness(30, 0, 0, 0), HorizontalTextAlignment = TextAlignment.Center };
                Label sendDate = new Label { FontSize = 16, WidthRequest = 200, LineBreakMode = LineBreakMode.NoWrap, Margin = new Thickness(30, 0, 0, 0), HorizontalTextAlignment = TextAlignment.Center };
                Image fav = new Image() {Source="star.png", WidthRequest = 30, HeightRequest = 30, Margin = new Thickness(30,0,0,0)};
                fav.Loaded += (s, e) => 
                {
                    var star = s as Image;
                    var context = star.BindingContext as UserMail;
                    if (context.IsFavourite(curUser))
                    {
                        star.Source = "checked_star.png";
                    }
                    else
                    {
                        star.Source = "star.png";
                    }
                };
                TapGestureRecognizer starTap = new();
                starTap.Tapped += (s, e) => 
                {
                    var star = s as Image;
                    var context = star.BindingContext as UserMail;
                    if (!context.IsFavourite(curUser))
                    {
                        star.Source = "checked_star.png";
                        AddToFav(context);
                    }
                    else
                    {
                        star.Source = "star.png";
                        RemoveFromFav(context);
                    }
                };
                fav.GestureRecognizers.Add(starTap);
                sender.SetBinding(Label.TextProperty, "Sender.FIO");
                title.SetBinding(Label.TextProperty, "UserEmailTitle");
                sendDate.SetBinding(Label.TextProperty, "SendDate");
                var stack = new HorizontalStackLayout { Children = {fav, sender, title, sendDate} };
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
            var sender = new Label {FontSize = 16, WidthRequest = 200, Text="От", Margin = new Thickness(90,0,0,0), HorizontalTextAlignment = TextAlignment.Center};
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
                Image fav = new Image() {Source="star.png", WidthRequest = 30, HeightRequest = 30, Margin = new Thickness(30,0,0,0)};
                fav.Loaded += (s, e) => 
                {
                    var star = s as Image;
                    var context = star.BindingContext as UserMail;
                    if (context.IsFavourite(curUser))
                    {
                        star.Source = "checked_star.png";
                    }
                    else
                    {
                        star.Source = "star.png";
                    }
                };
                TapGestureRecognizer starTap = new();
                starTap.Tapped += (s, e) => 
                {
                    var star = s as Image;
                    var context = star.BindingContext as UserMail;
                    if (!context.IsFavourite(curUser))
                    {
                        star.Source = "checked_star.png";
                        AddToFav(context);
                    }
                    else
                    {
                        star.Source = "star.png";
                        RemoveFromFav(context);
                    }
                };
                fav.GestureRecognizers.Add(starTap);
                fio.SetBinding(Label.TextProperty, "Getter.FIO");
                theme.SetBinding(Label.TextProperty, "UserEmailTitle");
                date.SetBinding(Label.TextProperty, "SendDate");
                var stack = new HorizontalStackLayout { Children = {fav, fio, theme, date} };
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
            var getter = new Label {FontSize = 16, WidthRequest = 200, Text="Кому", Margin = new Thickness(90,0,0,0), HorizontalTextAlignment = TextAlignment.Center};
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
                Image fav = new Image() {Source="star.png", WidthRequest = 30, HeightRequest = 30, Margin = new Thickness(30,0,0,0)};
                fav.Loaded += (s, e) => 
                {
                    var star = s as Image;
                    var context = star.BindingContext as Document;
                    if (context.IsFavourite(curUser))
                    {
                        star.Source = "checked_star.png";
                    }
                    else
                    {
                        star.Source = "star.png";
                    }
                };
                TapGestureRecognizer starTap = new();
                starTap.Tapped += (s, e) => 
                {
                    var star = s as Image;
                    var context = star.BindingContext as Document;
                    if (!context.IsFavourite(curUser))
                    {
                        star.Source = "checked_star.png";
                        AddToFav(context);
                    }
                    else
                    {
                        star.Source = "star.png";
                        RemoveFromFav(context);
                    }
                };
                fav.GestureRecognizers.Add(starTap);
                docName.SetBinding(Label.TextProperty, "DocumentName");
                docStat.SetBinding(Label.TextProperty, "documentStatus.DocumentStatusName");
                date.SetBinding(Label.TextProperty, "CreationDate");
                var stack = new HorizontalStackLayout { Children = {fav, docName, docStat, date} };
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
            var name = new Label {FontSize = 16, WidthRequest = 200, Text="Название", Margin = new Thickness(90,0,0,0), HorizontalTextAlignment = TextAlignment.Center};
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
                        var del = db.Documents.Include("Creator").Include("documentStatus").First(x => x.DocumentId == context.DocumentId);
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

        private async void AddToFav(object target)
        {
            try
            {
                var favdoc = target as Document;
                var favmail = target as UserMail;
                using AppContext db = new();
                var user = db.Users.First(x => x.UserId == curUser.UserId);
                if (favdoc == null && favmail != null)
                {
                    var mail = db.UsersMails.First(x => x.UserMailId == favmail.UserMailId);
                    db.FavoriteMails.Add(new FavoriteMail() {FavoritedMail = mail, FavoritedUser = user});
                }
                else if (favmail == null && favdoc != null)
                {
                    var doc = db.Documents.First(x => x.DocumentId == favdoc.DocumentId);
                    db.FavoriteDocuments.Add(new FavoriteDocument() {FavoritedDocument = doc, FavoritedUser = user});
                }
                else
                {
                    await DisplayAlert("Ошибка", "Ошибка преобразования типов", "Ок");
                    return;
                }
                db.SaveChanges();

            }
            catch (Exception ex)
            {
                await DisplayAlert("Ошибка", ex.Message, "Ок");
            }
        }

        private async void RemoveFromFav(object target)
        {
            try
            {
                var favdoc = target as Document;
                var favorite = target as Favorite;
                var favmail = target as UserMail;
                using AppContext db = new();
                if (favdoc == null && favmail != null)
                {
                    var fav = db.FavoriteMails.First(x => x.FavoritedMail.UserMailId == favmail.UserMailId && x.FavoritedUser.UserId == curUser.UserId);
                    db.FavoriteMails.Remove(fav);
                }
                else if (favmail == null && favdoc != null)
                {
                    var fav = db.FavoriteDocuments.First(x => x.FavoritedDocument.DocumentId == favdoc.DocumentId && x.FavoritedUser.UserId == curUser.UserId);
                    db.FavoriteDocuments.Remove(fav);
                }
                else if (favdoc == null && favmail == null && favorite != null)
                {
                    switch (favorite.Type)
                    {
                        case FavoriteType.Document:
                            var doc = db.FavoriteDocuments.First(x => x.FavoriteDocumentId == favorite.FavoriteId);
                            db.FavoriteDocuments.Remove(doc);
                            break;
                        case FavoriteType.Mail:
                            var mail = db.FavoriteMails.First(x => x.FavoriteMailId == favorite.FavoriteId);
                            db.FavoriteMails.Remove(mail);
                            break;
                    }
                    db.SaveChanges();
                    GetFav();
                }
                else
                {
                    await DisplayAlert("Ошибка", "Ошибка преобразования типов", "Ок");
                    return;
                }
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ошибка", ex.Message, "Ок");
            }
        }

        private async void FavoriteInteract(object sender, EventArgs e)
        {
            try
            {
                var context = (sender as HorizontalStackLayout).BindingContext as Favorite;
                using AppContext db = new();
                switch(context.Type)
                {
                    case FavoriteType.Document:
                        if (await DisplayAlert("Подтвердить действие", $"Вы хотите скачать документ {context.GetNameString}?", "Да", "Нет"))
                        {
                            var file = db.Documents.First(x => x.DocumentId == context.Document.DocumentId);
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
                        break;
                    case FavoriteType.Mail:
                        if (await DisplayAlert("Подтвердить действие", $"Вы хотите открыть сообщение {context.GetNameString}?", "Да", "Нет"))
                        {
                            var mail = db.UsersMails.Include("Sender").Include("Getter").Include("AttachedDocuments").First(x => x.UserMailId == context.Mail.UserMailId);
                            if (mail != null)
                            {
                                await Navigation.PushAsync(new FullMailViewPage(mail));
                            }
                            else await DisplayAlert("Ошибка", "Сообщение не найдено", "Ок");
                        }
                        break;
                }
                
            }
            catch(Exception ex)
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
                case ListType.Favorite:
                    {
                        List<Favorite> favorites = new();
                        List<Favorite> res = new();
                        var favdocs = db.FavoriteDocuments.Include("FavoritedDocument").Where(x => x.FavoritedUser.UserId == curUser.UserId).ToList();
                        var favmails = db.FavoriteMails.Include("FavoritedMail").Where(x => x.FavoritedUser.UserId == curUser.UserId).ToList();
                        foreach (var fav in favdocs)
                        {
                            favorites.Add(new Favorite() { FavoriteId = fav.FavoriteDocumentId, Document = fav.FavoritedDocument, Type = FavoriteType.Document });
                        }
                        foreach (var fav in favmails)
                        {
                            favorites.Add(new Favorite() { FavoriteId = fav.FavoriteMailId, Mail = fav.FavoritedMail, Type = FavoriteType.Mail });
                        }
                        res.AddRange(favorites.Where(x => x.GetNameString.ToLower().Contains(query)).ToList());
                        res.AddRange(favorites.Where(x => x.GetTypeString.ToLower().Contains(query)).ToList());
                        if (res.Count == 0)
                        {
                            res.AddRange(favorites.Where(x => DateTime.Parse(x.GetCreationDateString).Date == date.Date).ToList());
                        }
                        Mail.ItemsSource = res;
                    }
                    break;
            }
        }

        private void FillListByBaseData()
        {
            switch(curListViewType)
            {
                case ListType.Documents:
                    GetDocs();
                    break;
                case ListType.Input:
                    GetInputMail();
                    break;
                case ListType.Output:
                    GetOutputMail();
                    break;
                case ListType.Favorite:
                    GetFav();
                    break;
            }
        }

        #endregion
    }
}