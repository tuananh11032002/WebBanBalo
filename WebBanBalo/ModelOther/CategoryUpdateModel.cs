using System.ComponentModel.DataAnnotations;

namespace WebBanBalo.ModelOther
{
    public class CategoryUpdateModel
    {
        [Required]
        public int Id { set; get; }
        public string? Name { set; get; }
        public IFormFile? Image { set; get; }
        public IFormFile? ImageReplace { set; get; }

        public string? Title { set; get; }
    }
}
