using System;
using System.Collections.Generic;
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
    }
}
