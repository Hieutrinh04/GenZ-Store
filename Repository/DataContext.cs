using Microsoft.EntityFrameworkCore;
using WebBanDoDienTu.Models;

public class DataContext : DbContext
{
	public DataContext(DbContextOptions<DataContext> options) : base(options)
	{
	}

	// DbSet for BrandModel
	public DbSet<BrandModel> Brands { get; set; }

	// DbSet for ProductModel
	public DbSet<ProductModel> Products { get; set; }
	public DbSet<CategoryModel> Categories { get; set; }

}
