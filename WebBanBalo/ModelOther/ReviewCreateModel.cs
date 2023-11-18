using System.ComponentModel.DataAnnotations;

namespace WebBanBalo.ModelOther
{
    public class ReviewCreateModel
    {
        [Required]
        [MinLength(50)]
        public string Comment { get; set; }
        [Required]
        public int Rating { get; set; }

        [Required]
        public int ProductId { get; set; }
        [Required]
        public int UserId { get; set; }
        [Required]
        public int OrderId { get; set; }
    }
}
