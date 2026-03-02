namespace EduPortal.Data.Models.Entites
{
    public class VideoMaterial : Material
    {
        public TimeSpan Duration { get; set; }
        public string ResourceURL { get; set; }
        public string Quality { get; set; }
    }
}
