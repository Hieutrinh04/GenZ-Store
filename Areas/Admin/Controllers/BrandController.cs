using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using WebBanDoDienTu.Models;

namespace WebBanDoDienTu.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class BrandController : Controller
    {
        private readonly DataContext _dataContext;

        public BrandController(DataContext context, IWebHostEnvironment webHostEnvironment)
        {
            _dataContext = context;
        }

        // Phương thức Index hiển thị danh sách các thương hiệu
        public async Task<IActionResult> Index()
        {
            var brands = await _dataContext.Brands.OrderByDescending(p => p.Id).ToListAsync();
            return View(brands);
        }

        // Phương thức Create trả về view để tạo mới thương hiệu (GET)
        public IActionResult Create()
        {
            return View();
        }

        // Phương thức Create xử lý việc tạo thương hiệu mới (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BrandModel brand)
        {
            if (ModelState.IsValid)
            {
                // Tạo slug từ tên thương hiệu
                brand.Slug = brand.Name.Replace(" ", "-").ToLower();

                // Kiểm tra xem slug đã tồn tại trong database hay chưa
                var existingBrand = await _dataContext.Brands.FirstOrDefaultAsync(p => p.Slug == brand.Slug);
                if (existingBrand != null)
                {
                    ModelState.AddModelError("", "Thương hiệu đã tồn tại trong database");
                    return View(brand);
                }

                // Thêm thương hiệu mới vào cơ sở dữ liệu nếu slug chưa tồn tại
                _dataContext.Brands.Add(brand);
                await _dataContext.SaveChangesAsync();

                // Sử dụng TempData để truyền thông báo thành công
                TempData["success"] = "Thêm thương hiệu thành công";
                return RedirectToAction(nameof(Index));
            }

            // Sử dụng TempData để truyền thông báo lỗi
            TempData["error"] = "Có lỗi xảy ra. Vui lòng kiểm tra lại thông tin.";
            return View(brand);
        }

        // Phương thức Edit hiển thị view để chỉnh sửa thương hiệu (GET)
        [HttpGet]
        public async Task<IActionResult> EditAsync(int id)
        {
            var brand = await _dataContext.Brands.FindAsync(id);
            if (brand == null)
            {
                TempData["error"] = "Thương hiệu không tồn tại.";
                return RedirectToAction(nameof(Index));
            }
            return View(brand);
        }

        // Phương thức Edit xử lý việc cập nhật thương hiệu (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(BrandModel brand)
        {
            if (ModelState.IsValid)
            {
                // Tạo slug từ tên thương hiệu
                brand.Slug = brand.Name.Replace(" ", "-").ToLower();

                // Kiểm tra xem slug đã tồn tại trong database hay chưa, ngoại trừ thương hiệu hiện tại
                var existingBrand = await _dataContext.Brands.FirstOrDefaultAsync(p => p.Slug == brand.Slug && p.Id != brand.Id);
                if (existingBrand != null)
                {
                    ModelState.AddModelError("", "Thương hiệu đã tồn tại trong database");
                    return View(brand);
                }

                // Cập nhật thương hiệu trong cơ sở dữ liệu
                _dataContext.Brands.Update(brand);
                await _dataContext.SaveChangesAsync();

                // Sử dụng TempData để truyền thông báo thành công
                TempData["success"] = "Cập nhật thương hiệu thành công";
                return RedirectToAction(nameof(Index));
            }

            // Sử dụng TempData để truyền thông báo lỗi
            TempData["error"] = "Có lỗi xảy ra. Vui lòng kiểm tra lại thông tin.";
            return View(brand);
        }

        // Phương thức Delete để xóa thương hiệu (GET)
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            // Tìm thương hiệu theo Id
            var brand = await _dataContext.Brands.FindAsync(id);

            // Kiểm tra xem thương hiệu có tồn tại hay không
            if (brand == null)
            {
                TempData["error"] = "Thương hiệu không tồn tại.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                // Xóa thương hiệu
                _dataContext.Brands.Remove(brand);
                await _dataContext.SaveChangesAsync();

                TempData["success"] = "Thương hiệu đã xóa thành công.";
            }
            catch
            {
                TempData["error"] = "Có lỗi xảy ra khi xóa thương hiệu.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
