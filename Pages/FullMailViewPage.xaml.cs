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
            LoadMailInfo();
        }

        private async void LoadMailInfo()
        {
            try
            {
                Sendertxt.Text += curMail.Sender.FIO;
                Gettertxt.Text += curMail.Getter.FIO;
                Themetxt.Text += curMail.UserEmailTitle;
                Texttxt.Text = curMail.UserEmailBody;
                if (curMail.AttachedDocuments.Count() > 0)
                {
                    AttachedFilesView.ItemsSource = curMail.AttachedDocuments;
                }
                else
                {
                    Attachedlbl.Text = "Прикрепленные файлы отсутствуют";
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ошибка", ex.Message, "Ок");
            }
        }

        private async void DownloadAttachedFile(object sender, EventArgs e)
        {
            try
            {
                var document = (sender as VerticalStackLayout).BindingContext as Document;
                using AppContext db = new();
                var file = db.Documents.First(x => x.DocumentId == document.DocumentId);
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
            catch (Exception ex)
            {
                await DisplayAlert("Ошибка", ex.Message, "Ок");
            }
        }
    }
}