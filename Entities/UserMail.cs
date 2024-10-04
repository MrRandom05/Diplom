using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diplom.Entities
{
    public class UserMail
    {
        public int UserMailId { get; set; }
        public User Sender { get; set; }
        public User Getter { get; set; }
        public string UserEmailTitle { get; set; }
        public string UserEmailBody { get; set; }
        public DateTime SendDate { get; set; }
        public List<Document> AttachedDocuments { get; set; }
    }
}
