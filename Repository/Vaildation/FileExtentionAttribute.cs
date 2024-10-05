using System.ComponentModel.DataAnnotations;
using System.IO;  // Thêm dòng này để sử dụng Path
using Microsoft.AspNetCore.Http;  // Thêm dòng này để sử dụng IFormFile

namespace WebBanDoDienTu.Repository.Validation
{
    public class FileExtensionAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is IFormFile file)
            {
                var extension = Path.GetExtension(file.FileName); // Đảm bảo không phân biệt chữ hoa, chữ thường

                string[] extensions = { "jpg", "png", "jpeg" }; // Thêm dấu chấm vào các phần mở rộng
                bool result = extensions.Any(x => extension.EndsWith(x));
                if (!result)
                {
                    return new ValidationResult("Allowed extensions are jpg or png or jpeg");
                }

              
            }

            return ValidationResult.Success;
        }
    }
}
