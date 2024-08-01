using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace api_pixtopay.Model
{
    public class Blog
    {
        [Key]
        public int Id { get; set; }
        public required string  Nome { get; set; }
        public required string Description { get; set; }
        public byte[]? Image { get; set; }
        public byte[]? ImageBackground { get; set; }
    }
}
