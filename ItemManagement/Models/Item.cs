using System.ComponentModel.DataAnnotations;

namespace ItemManagement.Models
{
    public class Item
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "아이템의 이름을 등록해주세요.")]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "아이템의 가격을 등록해주세요.")]
        [Display(Name = "Price")]
        public int Price { get; set; }

        [Required(ErrorMessage = "아이템의 설명을 등록해주세요.")]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [Required(ErrorMessage = "아이템의 이미지를 등록해주세요.")]
        [Display(Name = "Icon")]
        public string Icon { get; set; }
    }
}