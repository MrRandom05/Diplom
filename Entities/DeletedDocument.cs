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
        public Role PrivateLevel { get; set; }
        public DateTime CreationDate { get; set; }
    }
}
