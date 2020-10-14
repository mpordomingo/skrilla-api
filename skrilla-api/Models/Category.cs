using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace skrilla_api.Models
{
    [Serializable]
    public class Category
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CategoryId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public bool Nonedit { get; set; }

        [Required]
        public string IconDescriptor { get; set; }

        [Required]
        public string PersonId { get; set; }

        public Category(string name,  bool nonedit, string personId, string icon)
        {
            this.Name = name;
            this.Nonedit = nonedit;
            this.PersonId = personId;
            this.IconDescriptor = icon;
        }

        public Category()
        {
        }
    }
}
