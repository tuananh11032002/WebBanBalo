using System.ComponentModel.DataAnnotations;

namespace WebBanBalo.ModelOther
{
    public class ProductInputModel
    {
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        [Required]
        public float Price { get; set; }

        [Required]
        [DataType(DataType.Upload)] 
        [Display(Name = "Images")]  
        public List<IFormFile> ImageFiles { get; set; }
    }
}
