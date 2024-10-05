using Microsoft.EntityFrameworkCore;

namespace WebBanDoDienTu.Repository
{
	public class DbContext
	{
		private DbContextOptions<DataContext> options;

		public DbContext(DbContextOptions<DataContext> options)
		{
			this.options = options;
		}
	}
}