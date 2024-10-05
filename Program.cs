using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WebBanDoDienTu.Repository;

var builder = WebApplication.CreateBuilder(args);

// Cấu hình chuỗi kết nối tới cơ sở dữ liệu
builder.Services.AddDbContext<DataContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DbConnection");
    options.UseSqlServer(connectionString); // Sử dụng SQL Server với chuỗi kết nối được cấu hình
});

// Thêm các dịch vụ vào container
builder.Services.AddControllersWithViews();

// Cấu hình cache phân tán (Distributed Memory Cache)
builder.Services.AddDistributedMemoryCache();

// Cấu hình Session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Thời gian hết hạn của session (30 phút)
    options.Cookie.HttpOnly = true; // Cookie session chỉ được truy cập qua HTTP
    options.Cookie.IsEssential = true; // Cookie session là bắt buộc
});

// Xây dựng ứng dụng
var app = builder.Build();

// Kích hoạt Session
app.UseSession();

// Cấu hình pipeline HTTP
if (!app.Environment.IsDevelopment())
{
    // Sử dụng trang lỗi chung khi không phải môi trường phát triển
    app.UseExceptionHandler("/Home/Error");
}
else
{
    // Hiển thị trang lỗi chi tiết trong môi trường phát triển
    app.UseDeveloperExceptionPage();
}

// Sử dụng file tĩnh (ví dụ như CSS, JS)
app.UseStaticFiles();

// Kích hoạt định tuyến (Routing)
app.UseRouting();

// Sử dụng Authorization (cho các yêu cầu liên quan đến quyền truy cập)
app.UseAuthorization();

// Định tuyến cho các Areas (khu vực con)
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

// Định tuyến mặc định cho các Controller
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Tạo scope và khởi tạo dữ liệu
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<DataContext>();
    SeedData.SeedingData(context); // Gọi hàm SeedData để khởi tạo dữ liệu mẫu
}

// Chạy ứng dụng
app.Run();
