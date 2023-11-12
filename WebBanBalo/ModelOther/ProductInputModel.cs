using System.ComponentModel.DataAnnotations;

namespace WebBanBalo.ModelOther
{
    public class ProductInputModel
    {
        [Required]
        public string Name { get; set; }
        public string? Description { get; set; }= string.Empty;
        [Required]
        public float Price { get; set; }

        [Required]
        [DataType(DataType.Upload)] 
        [Display(Name = "Images")]  
        public List<IFormFile> ImageFiles { get; set; }
        [Required]
        public int SoLuong { get; set; }
        public int CategoryId { get; set; }
        public float Discount { get; set; }

    }
}
