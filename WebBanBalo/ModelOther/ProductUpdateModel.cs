using System.ComponentModel.DataAnnotations;
using WebBanBalo.Model;

namespace WebBanBalo.ModelOther
{
    public class ProductUpdateModel
    {
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        [Required]
        public float Price { get; set; }
        public float Discount { get; set; }

        public int CategoryId { get; set; }
        
        public List<IFormFile>? ImageFiles { get; set; } =new List<IFormFile>();
        public List<string> linkImage { get; set; }= new List<string>();
        public StatusProduct? status { get; set; } 
        public int SoLuong { get; set; } = 0;
    }
}
