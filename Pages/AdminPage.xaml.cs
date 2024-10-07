using Diplom.Entities;
using Microsoft.EntityFrameworkCore;

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
        private ListType ListViewType = ListType.Input;

        private void FinishSimpleTemplate(Label fio, Label title, Label date)
        {
            Mail.ItemTemplate = new DataTemplate(() =>
            {
                ViewCell cell = new ViewCell();
                var stack = new HorizontalStackLayout { Children = {fio, title, date} };
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

        private void SetInputMailDataTemplate()
        {
            Label fio = new Label {FontSize = 16, WidthRequest = 200, LineBreakMode = LineBreakMode.NoWrap, Margin = new Thickness(30,0,0,0), HorizontalTextAlignment = TextAlignment.Center};
            Label title = new Label {FontSize = 16, WidthRequest = 200, LineBreakMode = LineBreakMode.NoWrap, Margin = new Thickness(30,0,0,0), HorizontalTextAlignment = TextAlignment.Center};
            Label date = new Label {FontSize = 16, WidthRequest = 200, LineBreakMode = LineBreakMode.NoWrap, Margin = new Thickness(30,0,0,0), HorizontalTextAlignment = TextAlignment.Center};
            fio.SetBinding(Label.TextProperty, "Sender.FIO");
            title.SetBinding(Label.TextProperty, "UserEmailTitle");
            date.SetBinding(Label.TextProperty, "SendDate");
            FinishSimpleTemplate(fio, title, date);
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
            Label title = new Label {FontSize = 16, WidthRequest = 200, LineBreakMode = LineBreakMode.NoWrap, Margin = new Thickness(30,0,0,0), HorizontalTextAlignment = TextAlignment.Center};
            Label date = new Label {FontSize = 16, WidthRequest = 200, LineBreakMode = LineBreakMode.NoWrap, Margin = new Thickness(30,0,0,0), HorizontalTextAlignment = TextAlignment.Center};
            fio.SetBinding(Label.TextProperty, "Getter.FIO");
            title.SetBinding(Label.TextProperty, "UserEmailTitle");
            date.SetBinding(Label.TextProperty, "SendDate");
            FinishSimpleTemplate(fio, title, date);
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
                AppContext db = new();
                var docs = db.Documents.ToList();
                if (docs != null)
                {

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
    }
}