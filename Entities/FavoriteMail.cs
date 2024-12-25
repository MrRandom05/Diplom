using Diplom.Entities;

namespace Diplom
{
    public class FavoriteMail
    {
        public int FavoriteMailId {get; set; }
        public UserMail FavoritedMail { get; set; }
        public User FavoritedUser { get; set; }
    }
}