using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // Thêm thư viện để sử dụng EF Core
using WebBanDoDienTu.Models;
using System.Linq;
using System.Threading.Tasks;

namespace WebBanDoDienTu.Controllers
{
    public class CategoryController : Controller
    {
        private readonly DataContext _dataContext;

        public CategoryController(DataContext context)
        {
            _dataContext = context;
        }

        public async Task<IActionResult> Index(string Slug = "")
        {
            // Tìm kiếm category dựa trên Slug
            var category = await _dataContext.Categories
                                    .Where(c => c.Slug == Slug)
                                    .FirstOrDefaultAsync();

            // Nếu không tìm thấy category, chuyển hướng về trang Index mặc định
            if (category == null)
            {
                return RedirectToAction("Index", "Home");
            }

            // Lấy danh sách sản phẩm dựa trên CategoryId
            var productsByCategory = _dataContext.Products
                                        .Where(p => p.CategoryId == category.Id)
                                        .OrderByDescending(p => p.Id);

            // Trả về view với danh sách sản phẩm đã lấy
            return View(await productsByCategory.ToListAsync());
        }
    }
}
