using System.Reflection.Metadata;
using Diplom.Entities;

namespace Diplom
{
    public class FavoriteDocument
    {
        public int FavoriteDocumentId { get; set; }
        public Entities.Document FavoritedDocument { get; set; }
        public User FavoritedUser {get; set; }
    }
}