using System.ComponentModel.DataAnnotations;

public enum Colors{
    secondary,
    success,
    danger,
    warning,
    info,
    light,
    dark,
    primary
}

namespace DiseaseApp.Entity
{
    public class Tag
    {
        [Key]
        public int TagId { get; set; }

        public string? Text { get; set; }

        public Colors? Color { get; set; }

        public string? Url { get; set; }

        public List<Post> Posts { get; set; } = new List<Post>();
    }
}