using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diplom.Entities
{
    public class Document : Docs
    {
        public int DocumentId { get; set; }
        public string DocumentName { get; set; }
        public byte[] DocumentData { get; set; }
        public DocumentStatus documentStatus { get; set; }
        public User Creator { get; set; }
        public User? ResponsibleWorker { get; set; } = null;
        public DateTime CreationDate { get; set; }

        [NotMapped]
        public string GetResponsibleUserString
        {
            get
            {
                return ResponsibleWorker == null ? "Отсутствует" : ResponsibleWorker.FIO;
            }
        }
        public bool IsFavourite(User user)
        {
            using AppContext db = new();
            var res = db.FavoriteDocuments.Where(x => x.FavoritedDocument.DocumentId == this.DocumentId && user.UserId == x.FavoritedUser.UserId);
            if (res.Any()) return true;
            return false;
        }

        public static Document Of(string name, byte[] data, DocumentStatus status, User creator)
        {
            return new Document() {DocumentName = name, DocumentData = data, documentStatus = status, Creator = creator, CreationDate = DateTime.Now};
        }

        public override void CastFromDocument(Document doc)
        {
            throw new NotImplementedException();
        }

        public override Document CastToDocument()
        {
            throw new NotImplementedException();
        }
    }
}
