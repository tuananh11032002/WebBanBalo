using System.ComponentModel.DataAnnotations;

namespace WebBanBalo.ModelOther
{
    public class CategoryCreateModel
    {
        [Required]
        public string Name { set; get; }
        [Required]
        public IFormFile Image { set; get; }
        [Required]
        public IFormFile ImageReplace { set; get; }

        public string? Title { set; get; }


    }
}
