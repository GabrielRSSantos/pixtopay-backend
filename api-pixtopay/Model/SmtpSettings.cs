using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace api_pixtopay.Model
{
    public class Contato
    {
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Tel { get; set; }
        public string Company { get; set; }
        public string Message { get; set; }
    }
}
