using System;
namespace skrilla_api.Models
{
    public class CategoryRequest
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Photoid { get; set; }
        public int Nonedit { get; set; }

        public CategoryRequest(){ }
    }
}
