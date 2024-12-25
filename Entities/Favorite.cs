using Diplom.Entities;

namespace Diplom
{
    public class Favorite
    {
        public int FavoriteId { get; set; }
        public FavoriteType Type { get; set; }
        public Document Document { get; set; }
        public UserMail Mail { get; set; }
        public string GetNameString
        {
            get
            {
                return this.Document == null ? this.Mail == null ? "непредвиденная ошибка" : this.Mail.UserEmailTitle : this.Document.DocumentName;
            }
        }

        public string GetTypeString
        {
            get
            {
                return this.Type == FavoriteType.Document ? "Документ" : this.Type == FavoriteType.Mail ? "Письмо" : "непредвиденная ошибка";
            }
        }

        public string GetCreationDateString
        {
            get
            {
                return this.Document == null ? this.Mail == null ? "непредвиденная ошибка" : this.Mail.SendDate.ToString() : this.Document.CreationDate.ToString();
            }
        }

    }
    public enum FavoriteType
    {
        Document,
        Mail
    }
}