namespace EduPortal.Data.Models.Entites
{
    public class BookMaterial : Material
    {
        public string Title { get; set; }
        public string Author { get; set; }
        public string FilePath { get; set; }
        public string Format { get; set; }
        public int NumberOfPages { get; set; }
        public int PublicationYear { get; set; }
    }
}
