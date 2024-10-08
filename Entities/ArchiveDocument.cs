using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diplom.Entities
{
    public class ArchiveDocument : Docs
    {
        public int ArchiveDocumentId { get; set; }
        public string ArchiveDocumentName { get; set; }
        public byte[] ArchiveDocumentData { get; set; }
        public DocumentStatus documentStatus { get; set; }
        public User Creator { get; set; }
        public Role PrivateLevel { get; set; }
        public DateTime CreationDate { get; set; }

        public override T CastFromDocument<T>()
        {
            throw new NotImplementedException();
        }

        public override Document CastToDocument()
        {
            throw new NotImplementedException();
        }
    }
}
