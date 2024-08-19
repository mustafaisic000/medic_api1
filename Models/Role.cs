using System.ComponentModel.DataAnnotations;

namespace MedicalLabApi.Models
{
    public class Role
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
    }
}
