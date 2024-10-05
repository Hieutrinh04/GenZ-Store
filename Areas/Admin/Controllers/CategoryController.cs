using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebBanDoDienTu.Models;

namespace WebBanDoDienTu.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly DataContext _dataContext;
      

        public CategoryController(DataContext context, IWebHostEnvironment webHostEnvironment)
        {
            _dataContext = context;
          
        }

        // Phương thức Index hiển thị danh sách các danh mục
        public async Task<IActionResult> Index()
        {
            var categories = await _dataContext.Categories.OrderByDescending(p => p.Id).ToListAsync();
            return View(categories);
        }

        // Phương thức Create trả về view để tạo mới danh mục (GET)
        public IActionResult Create()
        {
            return View();
        }

        // Phương thức Create xử lý việc tạo danh mục mới (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryModel category)
        {
            if (ModelState.IsValid)
            {
                // Tạo slug từ tên danh mục
                category.Slug = category.Name.Replace(" ", "-").ToLower();

                // Kiểm tra xem slug đã tồn tại trong database hay chưa
                var existingCategory = await _dataContext.Categories.FirstOrDefaultAsync(p => p.Slug == category.Slug);
                if (existingCategory != null)
                {
                    ModelState.AddModelError("", "Danh mục đã tồn tại trong database");
                    return View(category);
                }

                // Thêm danh mục mới vào cơ sở dữ liệu nếu slug chưa tồn tại
                _dataContext.Categories.Add(category);
                await _dataContext.SaveChangesAsync();

                // Sử dụng TempData để truyền thông báo thành công
                TempData["success"] = "Thêm danh mục thành công";
                return RedirectToAction(nameof(Index));
            }

            // Sử dụng TempData để truyền thông báo lỗi
            TempData["error"] = "Có lỗi xảy ra. Vui lòng kiểm tra lại thông tin.";
            return View(category);
        }
        // Phương thức Edit hiển thị view để chỉnh sửa danh mục (GET)
        [HttpGet]
        public async Task<IActionResult> EditAsync(int id)
        {
            var category = await _dataContext.Categories.FindAsync(id);
            if (category == null)
            {
                TempData["error"] = "Danh mục không tồn tại.";
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        // Phương thức Edit xử lý việc cập nhật danh mục (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CategoryModel category)
        {
            if (ModelState.IsValid)
            {
                // Tạo slug từ tên danh mục
                category.Slug = category.Name.Replace(" ", "-").ToLower();

                // Kiểm tra xem slug đã tồn tại trong database hay chưa, ngoại trừ danh mục hiện tại
                var existingCategory = await _dataContext.Categories.FirstOrDefaultAsync(p => p.Slug == category.Slug && p.Id != category.Id);
                if (existingCategory != null)
                {
                    ModelState.AddModelError("", "Danh mục đã tồn tại trong database");
                    return View(category);
                }

                // Cập nhật danh mục trong cơ sở dữ liệu
                _dataContext.Categories.Update(category);
                await _dataContext.SaveChangesAsync();

                // Sử dụng TempData để truyền thông báo thành công
                TempData["success"] = "Cập nhật danh mục thành công";
                return RedirectToAction(nameof(Index));
            }

            // Sử dụng TempData để truyền thông báo lỗi
            TempData["error"] = "Có lỗi xảy ra. Vui lòng kiểm tra lại thông tin.";
            return View(category);
        }


        // Phương thức Delete để xóa danh mục (GET)
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            // Tìm danh mục theo Id
            var category = await _dataContext.Categories.FindAsync(id);

            // Kiểm tra xem danh mục có tồn tại hay không
            if (category == null)
            {
                TempData["error"] = "Danh mục không tồn tại.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                // Xóa danh mục
                _dataContext.Categories.Remove(category);
                await _dataContext.SaveChangesAsync();

                TempData["success"] = "Danh mục đã xóa thành công.";
            }
            catch
            {
                TempData["error"] = "Có lỗi xảy ra khi xóa danh mục.";
            }

            return RedirectToAction(nameof(Index));
        }
       

    }
}

