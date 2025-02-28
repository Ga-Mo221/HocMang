using System;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace server_test
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Đặt tiêu đề cho cửa sổ Console
            Console.Title = "TCP server :";

            // Tạo một Socket kiểu stream dùng giao thức TCP
            var listenner = new Socket(SocketType.Stream, ProtocolType.Tcp);

            // Gắn socket với tất cả các địa chỉ IP của máy và cổng 1308
            listenner.Bind(new IPEndPoint(IPAddress.Any, 1308));

            // Bắt đầu lắng nghe các kết nối đến với tối đa 10 kết nối chờ
            listenner.Listen(10);

            // In ra thông báo server đã khởi động cùng với địa chỉ và cổng của socket
            Console.WriteLine($"server dang khoi dong {listenner.LocalEndPoint}");

            // Vòng lặp vô hạn để liên tục chấp nhận kết nối từ client
            while (true)
            {
                // Chấp nhận kết nối từ client, tạo ra socket riêng cho phiên làm việc đó
                var worker = listenner.Accept();

                // Tạo một luồng (NetworkStream) từ socket của client để đọc và ghi dữ liệu
                var stream = new NetworkStream(worker);

                // Đối tượng StreamReader để đọc dữ liệu gửi từ client
                var reader = new StreamReader(stream);

                // Đối tượng StreamWriter để ghi dữ liệu gửi về client
                var writer = new StreamWriter(stream);

                // Đọc một dòng văn bản từ client
                var request = reader.ReadLine();

                // Khởi tạo biến chứa phản hồi trả về cho client
                var response = string.Empty;

                // In yêu cầu nhận được từ client lên Console để theo dõi
                Console.WriteLine(request);

                // Khởi tạo các biến để chứa tên lệnh và tham số của yêu cầu
                string requestName = string.Empty;
                string requestParameter = string.Empty;

                // Kiểm tra nếu yêu cầu có ít nhất 3 ký tự (để tách tên lệnh)
                if (request.Length >= 3)
                {
                    // Lấy 3 ký tự đầu tiên làm tên lệnh (ví dụ: "PRI", "ODD", "EVE")
                    requestName = request.Substring(0, 3);
                    // Loại bỏ tên lệnh ra khỏi chuỗi để lấy phần tham số
                    requestParameter = request.Replace(requestName, "");
                }

                // Cố gắng chuyển đổi tham số thành số nguyên
                if (int.TryParse(requestParameter, out int number) == true)
                {
                    // Chuyển đổi tên lệnh sang chữ hoa để đảm bảo không phân biệt hoa thường
                    switch (requestName.ToUpper())
                    {
                        // Lệnh "PRI": Kiểm tra số nguyên tố
                        case "PRI":
                            bool checkPRI = true;
                            // Duyệt từ 2 đến căn bậc hai của số để kiểm tra số chia hết
                            for (int i = 2; i <= Math.Sqrt(number); i++)
                            {
                                // Nếu tìm thấy ước số, thì không phải số nguyên tố
                                if (number % i == 0)
                                {
                                    checkPRI = false;
                                    break;
                                }
                            }
                            // Nếu checkPRI vẫn true thì số là nguyên tố, ngược lại không phải
                            if (checkPRI == true)
                            {
                                response = "la so nguyen to"; // là số nguyên tố
                            }
                            else
                            {
                                response = "khong phai la so nguyen to"; // không phải số nguyên tố
                            }
                            break;

                        // Lệnh "ODD": Kiểm tra số lẻ/chẵn
                        case "ODD":
                            if (number % 2 == 0)
                            {
                                response = "la so chan"; // số chẵn
                            }
                            else
                            {
                                response = "la so le"; // số lẻ
                            }
                            break;

                        // Lệnh "EVE": Dự kiến kiểm tra số chẵn nhưng có lỗi logic trong điều kiện
                        case "EVE":
                            // Lỗi logic: nếu số không chia hết cho 2 (số lẻ) thì lại thông báo là số chẵn
                            if (number % 2 != 0)
                            {
                                response = "la so chan";
                            }
                            else
                            {
                                response = "la so le";
                            }
                            break;

                        // Nếu không khớp với bất kỳ lệnh nào đã định nghĩa
                        default:
                            response = "unknawn command";
                            break;
                    }
                }
                else
                {
                    // Nếu tham số không thể chuyển sang số nguyên, trả về thông báo lỗi
                    response = "incorrect parameter";
                }

                // Gửi phản hồi về cho client
                writer.WriteLine(response);
                writer.Flush(); // Đẩy dữ liệu ra ngoài stream
                writer.Close(); // Đóng StreamWriter để kết thúc phiên giao tiếp với client
            }
        }
    }
}
