using Microsoft.AspNetCore.Mvc;
using WebBanDoDienTu.Models;
using WebBanDoDienTu.Models.ViewModels;
using WebBanDoDienTu.Repository;
using System.Threading.Tasks;
using System.Linq;

namespace WebBanDoDienTu.Controllers
{
    // Controller xử lý các hành động liên quan đến giỏ hàng
    public class CartController : Controller
    {
        // Khai báo DataContext để làm việc với cơ sở dữ liệu
        private readonly DataContext _dataContext;

        // Hàm khởi tạo CartController, nhận vào DataContext
        public CartController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        // Hành động để hiển thị trang giỏ hàng
        public IActionResult Index()
        {
            // Lấy danh sách các mục trong giỏ hàng từ Session, nếu không có thì tạo danh sách trống
            List<CartItemModel> cartItems = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();

            // Tạo mô hình ViewModel để truyền dữ liệu sang View
            CartItemViewModel cartVM = new()
            {
                // Gán danh sách các mục trong giỏ hàng
                CartItems = cartItems,

                // Tính tổng tiền bằng cách nhân số lượng với giá của từng sản phẩm
                GrandTotal = cartItems.Sum(x => x.Quantity * x.Price)
            };

            // Trả về View với mô hình ViewModel giỏ hàng
            return View(cartVM);
        }

        // Hành động để hiển thị trang thanh toán
        public IActionResult Checkout()
        {
            // Trả về View của trang thanh toán
            return View("~/Views/Checkout/Index.cshtml");
        }

        // Hành động thêm sản phẩm vào giỏ hàng
        public async Task<IActionResult> Add(int Id)
        {
            // Tìm sản phẩm dựa trên Id
            ProductModel product = await _dataContext.Products.FindAsync(Id);

            if (product == null)
            {
                TempData["error"] = "Product not found";
                return RedirectToAction("Index");
            }

            // Lấy danh sách giỏ hàng từ Session, nếu không có thì tạo danh sách trống
            List<CartItemModel> cart = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();

            // Kiểm tra xem sản phẩm đã có trong giỏ hàng chưa
            CartItemModel cartItem = cart.Where(c => c.ProductId == Id).FirstOrDefault();

            if (cartItem == null)
            {
                // Nếu sản phẩm chưa có trong giỏ hàng, thêm mới
                cart.Add(new CartItemModel(product));
            }
            else
            {
                // Nếu sản phẩm đã có, tăng số lượng
                cartItem.Quantity += 1;
            }

            // Lưu lại danh sách giỏ hàng vào session
            HttpContext.Session.SetJson("Cart", cart);

            TempData["success"] = "Item added to cart successfully";

            // Redirect về trang trước đó (trang đã gọi hành động thêm vào giỏ hàng)
            return Redirect(Request.Headers["Referer"].ToString());
        }

        // Hành động giảm số lượng sản phẩm trong giỏ hàng
        public IActionResult Decrease(int Id)
        {
            List<CartItemModel> cart = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();

            CartItemModel cartItem = cart.Where(c => c.ProductId == Id).FirstOrDefault();

            if (cartItem != null)
            {
                if (cartItem.Quantity > 1)
                {
                    cartItem.Quantity--;
                }
                else
                {
                    cart.Remove(cartItem);
                }
            }

            UpdateCartSession(cart);

            TempData["success"] = "Item quantity decreased successfully";
            return RedirectToAction("Index");
        }

        // Hành động tăng số lượng sản phẩm trong giỏ hàng
        public IActionResult Increase(int Id)
        {
            List<CartItemModel> cart = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();

            CartItemModel cartItem = cart.Where(c => c.ProductId == Id).FirstOrDefault();

            if (cartItem != null)
            {
                cartItem.Quantity++;
            }

            UpdateCartSession(cart);

            TempData["success"] = "Item quantity increased successfully";
            return RedirectToAction("Index");
        }

        // Hành động xóa sản phẩm khỏi giỏ hàng
        public IActionResult Remove(int Id)
        {
            List<CartItemModel> cart = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();

            cart.RemoveAll(p => p.ProductId == Id);

            UpdateCartSession(cart);

            TempData["success"] = "Item removed from cart successfully";
            return RedirectToAction("Index");
        }

        // Hành động xóa toàn bộ giỏ hàng
        public IActionResult Clear()
        {
            HttpContext.Session.Remove("Cart");
            TempData["success"] = "Cart cleared successfully";
            return RedirectToAction("Index");
        }

        // Hàm tiện ích để cập nhật lại session giỏ hàng
        private void UpdateCartSession(List<CartItemModel> cart)
        {
            if (cart.Count == 0)
            {
                HttpContext.Session.Remove("Cart");
            }
            else
            {
                HttpContext.Session.SetJson("Cart", cart);
            }
        }
    }
}
