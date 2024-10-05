using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using WebBanDoDienTu.Models; // Đảm bảo bạn đã thêm namespace chứa DataContext và các Model

namespace WebBanDoDienTu.Controllers
{
    public class BrandController : Controller
    {
        private readonly DataContext _dataContext;

        public BrandController(DataContext context)
        {
            _dataContext = context;
        }

        public async Task<IActionResult> Index(string Slug = "")
        {
            // Tìm kiếm thương hiệu (Brand) dựa trên Slug
            var brand = await _dataContext.Brands
                                    .Where(b => b.Slug == Slug)
                                    .FirstOrDefaultAsync();

            // Nếu không tìm thấy Brand, chuyển hướng về trang Index mặc định
            if (brand == null)
            {
                return RedirectToAction("Index", "Home");
            }

            // Lấy danh sách sản phẩm dựa trên BrandId
            var productsByBrand = _dataContext.Products
                                        .Where(p => p.BrandId == brand.Id)
                                        .OrderByDescending(p => p.Id);

            // Trả về view với danh sách sản phẩm theo thương hiệu đã lấy
            return View(await productsByBrand.ToListAsync());
        }
    }
}
