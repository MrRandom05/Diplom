using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diplom.Entities
{
    public class DeletedDocument : Docs
    {
        public int DeletedDocumentId { get; set; }
        public string DeletedDocumentName { get; set; }
        public byte[] DeletedDocumentData { get; set; }
        public DocumentStatus documentStatus { get; set; }
        public User Creator { get; set; }
        public DateTime CreationDate { get; set; }

        public static DeletedDocument Of(string name, byte[] data, DocumentStatus status, User creator)
        {
            return new DeletedDocument() { DeletedDocumentName = name, DeletedDocumentData = data, documentStatus = status, Creator = creator, CreationDate = DateTime.Now};
        }
    
        public override void CastFromDocument(Document doc)
        {
            DeletedDocumentName = doc.DocumentName;
            DeletedDocumentData = doc.DocumentData;
            documentStatus = doc.documentStatus;
            Creator = doc.Creator;
            CreationDate = DateTime.Now;
        }

        public override Document CastToDocument()
        {
            return Document.Of(DeletedDocumentName, DeletedDocumentData, documentStatus, Creator);
        }
    }
}
