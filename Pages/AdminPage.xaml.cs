using Diplom.Entities;
using Microsoft.EntityFrameworkCore;

namespace Diplom
{
    public partial class AdminPage : ContentPage
    {
        private User curUser;
        private static DataTemplate inputMailDT = new DataTemplate(() => {
            Label fio = new Label {FontSize = 16, WidthRequest = 200, LineBreakMode = LineBreakMode.NoWrap, Margin=new Thickness(30,0,0,0)};
            Label title = new Label {FontSize = 16, WidthRequest = 200, LineBreakMode = LineBreakMode.NoWrap, Margin=new Thickness(30,0,0,0)};
            Label date = new Label {FontSize = 16, WidthRequest = 200, LineBreakMode = LineBreakMode.NoWrap, Margin=new Thickness(30,0,0,0)};
            fio.SetBinding(Label.TextProperty, "Sender.FIO");
            title.SetBinding(Label.TextProperty, "UserEmailTitle");
            date.SetBinding(Label.TextProperty, "SendDate");

            return new ViewCell
            {
                View = new HorizontalStackLayout
                {
                    Children = {fio, title, date}
                }
            };
        });

        public AdminPage(User user)
        {
            curUser = user;
            InitializeComponent();
        }

        private void LoadInputMail(object sender, EventArgs e)
        {
            using AppContext db = new();
            var mail = db.UsersMails.Include("Sender").Include("Getter").Include("AttachedDocuments").Where(x => x.Getter.UserId == curUser.UserId).ToList();
            if (mail != null)
            {
                InputMail.ItemTemplate = inputMailDT;
                InputMail.ItemsSource = mail;
            }
        }
    }
}