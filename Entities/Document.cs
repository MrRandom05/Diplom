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
        public Role PrivateLevel { get; set; }
        public DateTime CreationDate { get; set; }

        public static Document Of(string name, byte[] data, DocumentStatus status, User creator, Role privatelvl)
        {
            return new Document() {DocumentName = name, DocumentData = data, documentStatus = status, Creator = creator, PrivateLevel = privatelvl, CreationDate = DateTime.Now};
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
