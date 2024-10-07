using Diplom.Entities;

namespace Diplom
{
    partial class FullMailViewPage : ContentPage
    {
        private UserMail curMail;
        public FullMailViewPage(UserMail mail)
        {
            InitializeComponent();
            curMail = mail;
        }
    }
}