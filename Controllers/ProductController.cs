using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace WebBanDoDienTu.Controllers
{
    public class ProductController : Controller
    {
        private readonly DataContext _dataContext;

        public ProductController(DataContext context)
        {
            _dataContext = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Details(int id)
        {
            // Kiểm tra ID có hợp lệ không
            if (id <= 0)
            {
                return RedirectToAction("Index");
            }

            // Lấy sản phẩm theo ID
            var product = await _dataContext.Products
                                    .Where(p => p.Id == id)
                                    .FirstOrDefaultAsync();

            // Kiểm tra xem sản phẩm có tồn tại không
            if (product == null)
            {
                return NotFound(); // Trả về 404 nếu không tìm thấy sản phẩm
            }

            return View(product); // Trả về sản phẩm cho view
        }
    }
}
