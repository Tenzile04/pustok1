namespace Pustokk.Models
{
    public class BookImage:BaseEntity
    {
        public string ImageUrl { get; set; }        
        public int BookId { get; set; }
        public Book? Book { get; set; }
        public bool? IsPoster { get; set; }

    }
}
