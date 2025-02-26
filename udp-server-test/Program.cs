using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace UdpServer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Đặt tiêu đề cho cửa sổ console
            Console.Title = "Udp Server";

            // Lấy địa chỉ IP bất kỳ của máy (server sẽ lắng nghe trên tất cả các địa chỉ)
            var localIp = IPAddress.Any;
            // Xác định cổng mà server sẽ lắng nghe, ở đây là 1308
            var localPort = 1308;
            // Tạo endpoint kết hợp giữa IP và cổng đã chỉ định
            var localEndPoint = new IPEndPoint(localIp, localPort);

            // Tạo một socket sử dụng giao thức UDP (SocketType.Dgram)
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            // Gán địa chỉ IP và cổng cho socket (bind socket) để chuẩn bị nhận dữ liệu
            socket.Bind(localEndPoint);

            // In ra thông báo cho biết server đã khởi động thành công và đang chờ dữ liệu
            Console.WriteLine($"Local socket bind to {localEndPoint}. Waiting for request...");

            // Xác định kích thước bộ đệm nhận dữ liệu (1024 byte)
            var size = 1024;
            // Tạo mảng byte để chứa dữ liệu nhận được từ client
            var receiveBuffer = new byte[size];

            // Vòng lặp vô hạn, server luôn sẵn sàng nhận và xử lý dữ liệu
            while (true)
            {
                // Khởi tạo một EndPoint để lưu thông tin của client gửi dữ liệu
                EndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
                // Nhận dữ liệu từ client và lưu số byte nhận được vào biến length
                int length = socket.ReceiveFrom(receiveBuffer, ref remoteEndPoint);

                // Chuyển đổi dữ liệu nhận được từ dạng byte sang chuỗi sử dụng mã ASCII
                var request = Encoding.ASCII.GetString(receiveBuffer, 0, length);
                // In ra thông tin của client và nội dung yêu cầu đã nhận
                Console.WriteLine($"Received from {remoteEndPoint}: {request}");

                // Tìm vị trí của ký tự khoảng trắng đầu tiên trong chuỗi yêu cầu
                int khoangTrang = request.IndexOf(' ');
                // Khởi tạo biến lưu tên lệnh (command)
                string requestName = string.Empty;

                // Nếu tìm thấy khoảng trắng, nghĩa là yêu cầu có chứa lệnh và tham số
                if (khoangTrang != -1)
                {
                    // Lấy phần tên lệnh từ đầu chuỗi đến ký tự khoảng trắng
                    requestName = request.Substring(0, khoangTrang);
                }

                // Xóa phần tên lệnh và khoảng trắng để lấy phần tham số của yêu cầu
                string requestParameter = request.Replace($"{requestName} ", "");
                // Khởi tạo biến chứa phản hồi trả về cho client
                var response = string.Empty;

                // Xử lý yêu cầu dựa trên tên lệnh (chuyển đổi thành chữ hoa để không phân biệt chữ hoa/chữ thường)
                switch (requestName.ToUpper())
                {
                    // Nếu lệnh là "UPPER": chuyển tham số thành chữ hoa
                    case "UPPER":
                        response = requestParameter.ToUpper();
                        break;
                    // Nếu lệnh là "LOWER": chuyển tham số thành chữ thường
                    case "LOWER":
                        response = requestParameter.ToLower();
                        break;
                    // Nếu lệnh là "LENGTH": trả về độ dài của chuỗi tham số
                    case "LENGTH":
                        response = requestParameter.Length.ToString();
                        break;
                    // Nếu lệnh không được nhận diện
                    default:
                        response = "UNKNOWN COMMAND";
                        break;
                }

                // Chuyển đổi chuỗi phản hồi thành mảng byte (sử dụng mã ASCII) để gửi về client
                var sendBuffer = Encoding.ASCII.GetBytes(response);
                // Gửi phản hồi về client đến đúng endpoint đã nhận được yêu cầu
                socket.SendTo(sendBuffer, remoteEndPoint);

                // Xóa bộ đệm nhận dữ liệu để sẵn sàng cho lần nhận dữ liệu tiếp theo
                Array.Clear(receiveBuffer, 0, size);
            }
        }
    }
}
