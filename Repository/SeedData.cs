using Microsoft.EntityFrameworkCore;
using WebBanDoDienTu.Models;

namespace WebBanDoDienTu.Repository
{
    public class SeedData
    {
        public static void SeedingData(DataContext _context)
        {
            _context.Database.Migrate();

            // Kiểm tra xem dữ liệu đã tồn tại chưa
            if (!_context.Products.Any() && !_context.Categories.Any() && !_context.Brands.Any())
            {
                // Tạo các đối tượng Category và Brand
                CategoryModel macbook = new CategoryModel
                {
                    Name = "Macbook",
                    Slug = "macbook",
                    Description = "Macbook is large Brand in the world",
                    Status = 1
                };

                CategoryModel pc = new CategoryModel
                {
                    Name = "Pc",
                    Slug = "pc",
                    Description = "Pc is large Brand in the world",
                    Status = 1
                };

                BrandModel apple = new BrandModel
                {
                    Name = "Apple",
                    Slug = "apple",
                    Description = "Apple is large Brand in the world",
                    Status = 1
                };

                BrandModel samsung = new BrandModel
                {
                    Name = "Samsung",
                    Slug = "samsung",
                    Description = "Samsung is large Brand in the world",
                    Status = 1
                };

                // Thêm các Category và Brand vào cơ sở dữ liệu trước
                _context.Categories.AddRange(macbook, pc);
                _context.Brands.AddRange(apple, samsung);
                _context.SaveChanges();

                // Thêm các Product sau khi đã có Category và Brand trong cơ sở dữ liệu
                _context.Products.AddRange(
                    new ProductModel
                    {
                        Name = "Macbook",
                        Slug = "macbook",
                        Description = "Macbook is Best",
                        Image = "1.jpg",
                        Category = macbook,
                        Brand = apple,
                        Price = 1234
                    },
                    new ProductModel
                    {
                        Name = "Pc",
                        Slug = "pc",
                        Description = "Pc is Best",
                        Image = "1.jpg",
                        Category = pc,
                        Brand = samsung,
                        Price = 1234
                    }
                );

                // Lưu các thay đổi vào cơ sở dữ liệu
                _context.SaveChanges();
            }
        }
    }
}
