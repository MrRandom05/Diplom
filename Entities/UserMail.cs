using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
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
        public bool IsFavourite(User user)
        {
            using AppContext db = new();
            var res = db.FavoriteMails.Where(x => x.FavoritedMail.UserMailId == this.UserMailId && user.UserId == x.FavoritedUser.UserId);
            if (res.Any()) return true;
            return false;
        }
    }
}
