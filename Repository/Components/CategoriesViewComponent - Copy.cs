using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using WebBanDoDienTu.Models; // Đảm bảo bạn có namespace chứa CategoryModel

namespace WebBanDoDienTu.Repository.Components
{
	public class CategoriesViewComponent : ViewComponent
	{
		private readonly DataContext _dataContext;

		public CategoriesViewComponent(DataContext context)
		{
			_dataContext = context;
		}

		// Sửa lại: Sử dụng await cho phương thức bất đồng bộ và truyền kết quả đúng vào View
		public async Task<IViewComponentResult> InvokeAsync()
		{
			var categories = await _dataContext.Categories.ToListAsync();
			return View(categories); // Trả về danh sách các category
		}
	}
}
