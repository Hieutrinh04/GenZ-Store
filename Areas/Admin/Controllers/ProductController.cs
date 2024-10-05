using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebBanDoDienTu.Models;
using System.IO;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace WebBanDoDienTu.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly IWebHostEnvironment _webHostEnvironment;

        // Constructor khởi tạo IWebHostEnvironment
        public ProductController(DataContext context, IWebHostEnvironment webHostEnvironment)
        {
            _dataContext = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // Phương thức Index để hiển thị danh sách sản phẩm
        public async Task<IActionResult> Index()
        {
            var products = await _dataContext.Products
                .OrderByDescending(p => p.Id)
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .ToListAsync();

            return View(products);
        }

        // Phương thức Create hiển thị form tạo sản phẩm (GET)
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Categories = new SelectList(_dataContext.Categories, "Id", "Name");
            ViewBag.Brands = new SelectList(_dataContext.Brands, "Id", "Name");
            return View();
        }

        // Phương thức Create xử lý việc tạo sản phẩm mới (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductModel product)
        {
            if (ModelState.IsValid)
            {
                // Tạo slug cho sản phẩm
                product.Slug = product.Name.Replace(" ", "-").ToLower();
                var existingProduct = await _dataContext.Products.FirstOrDefaultAsync(p => p.Slug == product.Slug);

                if (existingProduct != null)
                {
                    ModelState.AddModelError("", "Sản phẩm đã có trong database");
                }
                else
                {
                    // Nếu có file hình ảnh, thực hiện lưu file
                    if (product.ImageUpload != null)
                    {
                        string uploadDir = Path.Combine(_webHostEnvironment.WebRootPath, "media/products");
                        string imageName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(product.ImageUpload.FileName);
                        string filePath = Path.Combine(uploadDir, imageName);

                        using (var fs = new FileStream(filePath, FileMode.Create))
                        {
                            await product.ImageUpload.CopyToAsync(fs);
                        }
                        product.Image = imageName;
                    }

                    // Thêm sản phẩm vào cơ sở dữ liệu
                    _dataContext.Add(product);
                    await _dataContext.SaveChangesAsync();

                    TempData["success"] = "Thêm sản phẩm thành công";
                    return RedirectToAction("Index");
                }
            }

            // Nếu model không hợp lệ, hiển thị lại form với dữ liệu đã nhập và thông báo lỗi
            ViewBag.Categories = new SelectList(_dataContext.Categories, "Id", "Name", product.CategoryId);
            ViewBag.Brands = new SelectList(_dataContext.Brands, "Id", "Name", product.BrandId);

            TempData["error"] = "Có lỗi xảy ra. Vui lòng kiểm tra lại thông tin.";
            return View(product);
        }

        // Phương thức Edit để chỉnh sửa sản phẩm (GET)
        [HttpGet]
        public async Task<IActionResult> Edit(int Id)
        {
            var product = await _dataContext.Products.FindAsync(Id);

            if (product == null)
            {
                TempData["error"] = "Sản phẩm không tồn tại.";
                return RedirectToAction("Index");
            }

            ViewBag.Categories = new SelectList(_dataContext.Categories, "Id", "Name", product.CategoryId);
            ViewBag.Brands = new SelectList(_dataContext.Brands, "Id", "Name", product.BrandId);

            return View(product);
        }

        // Phương thức Edit xử lý việc chỉnh sửa sản phẩm (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int Id, ProductModel product)
        {
          



            if (Id != product.Id)
            {
                return NotFound();
            }

            var updatedProduct = await _dataContext.Products.FirstOrDefaultAsync(x => x.Id == Id);
            if (updatedProduct == null)
            {
                TempData["error"] = "Sản phẩm không tồn tại.";
                return RedirectToAction("Index");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Cập nhật các thông tin của sản phẩm
                    updatedProduct.Name = product.Name;
                    updatedProduct.Slug = product.Name.Replace(" ", "-").ToLower(); // Tạo lại slug
                    updatedProduct.CategoryId = product.CategoryId;
                    updatedProduct.BrandId = product.BrandId;
                    updatedProduct.Price = product.Price;
                    updatedProduct.Description = product.Description;

                    // Nếu có file hình ảnh mới, cập nhật hình ảnh
                    if (product.ImageUpload != null)
                    {
                        string uploadDir = Path.Combine(_webHostEnvironment.WebRootPath, "media/products");
                        string imageName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(product.ImageUpload.FileName);
                        string filePath = Path.Combine(uploadDir, imageName);

                        using (var fs = new FileStream(filePath, FileMode.Create))
                        {
                            await product.ImageUpload.CopyToAsync(fs);
                        }

                        // Xóa hình ảnh cũ nếu có
                        if (!string.IsNullOrEmpty(updatedProduct.Image))
                        {
                            var oldImagePath = Path.Combine(uploadDir, updatedProduct.Image);
                            if (System.IO.File.Exists(oldImagePath))
                            {
                                System.IO.File.Delete(oldImagePath);
                            }
                        }

                        updatedProduct.Image = imageName;
                    }

                    _dataContext.Update(updatedProduct);
                    await _dataContext.SaveChangesAsync();

                    TempData["success"] = "Cập nhật sản phẩm thành công";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_dataContext.Products.Any(p => p.Id == product.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index");
            }

            ViewBag.Categories = new SelectList(_dataContext.Categories, "Id", "Name", updatedProduct.CategoryId);
            ViewBag.Brands = new SelectList(_dataContext.Brands, "Id", "Name", updatedProduct.BrandId);

            TempData["error"] = "Có lỗi xảy ra. Vui lòng kiểm tra lại thông tin.";
            return View(updatedProduct);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int Id)
        {
            var product = await _dataContext.Products.FindAsync(Id);

            if (product == null)
            {
                TempData["error"] = "Sản phẩm không tồn tại.";
                return RedirectToAction("Index");
            }

            try
            {
                // Xóa hình ảnh nếu có
                if (!string.Equals(product.Image, "noname.jpg"))
                {
                    string uploadDir = Path.Combine(_webHostEnvironment.WebRootPath, "media/products");
                    string oldFilePath = Path.Combine(uploadDir, product.Image);
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }

                _dataContext.Products.Remove(product);
                await _dataContext.SaveChangesAsync();

                TempData["success"] = "Xóa sản phẩm thành công";
            }
            catch
            {
                TempData["error"] = "Có lỗi xảy ra khi xóa sản phẩm.";
            }

            return RedirectToAction("Index");
        }


    }
}
