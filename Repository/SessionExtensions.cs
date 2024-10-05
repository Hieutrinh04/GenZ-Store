using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;

namespace WebBanDoDienTu.Repository
{
    // Lớp tĩnh mở rộng các phương thức cho ISession
    public static class SessionExtensions
    {
        // Phương thức để lưu dữ liệu dưới dạng JSON vào session
        public static void SetJson(this ISession session, string key, object value)
        {
            // Chuyển đổi object thành chuỗi JSON và lưu vào session
            session.SetString(key, JsonConvert.SerializeObject(value));
        }

        // Phương thức để lấy dữ liệu từ session và chuyển đổi lại thành đối tượng kiểu T
        public static T GetJson<T>(this ISession session, string key)
        {
            // Lấy chuỗi JSON từ session dựa vào key
            var sessionData = session.GetString(key);

            // Nếu dữ liệu không tồn tại, trả về giá trị mặc định của kiểu T
            // Ngược lại, chuyển chuỗi JSON thành đối tượng kiểu T
            return sessionData == null ? default(T) : JsonConvert.DeserializeObject<T>(sessionData);
        }
    }
}
