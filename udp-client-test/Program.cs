using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace UdpClientApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Đặt tiêu đề cho cửa sổ Console của client
            Console.Title = "Udp Client";

            // Yêu cầu nhập địa chỉ IP của Server
            Console.Write("Server IP: ");
            // Ở đây sử dụng địa chỉ IP localhost; có thể thay thế bằng Console.ReadLine() để nhập từ bàn phím
            var ipString = "127.0.0.1";
            // Chuyển đổi chuỗi IP sang đối tượng IPAddress
            var serverIp = IPAddress.Parse(ipString);

            // Yêu cầu nhập cổng của Server
            Console.Write("Server port: ");
            // Ở đây sử dụng cổng 1308; có thể thay thế bằng Console.ReadLine() để nhập từ bàn phím
            var portString = "1308";
            // Chuyển đổi chuỗi cổng sang số nguyên
            var serverPort = int.Parse(portString);

            // Vòng lặp vô hạn để liên tục gửi yêu cầu đến server
            while (true)
            {
                // Thiết lập màu chữ cho dòng lệnh nhập (prompt)
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("# COMMAND >>>");
                Console.ResetColor();

                // Đọc dòng văn bản nhập từ người dùng (lệnh sẽ gửi tới server)
                string text = Console.ReadLine();
                // Nếu không nhập gì hoặc chỉ nhập khoảng trắng thì bỏ qua vòng lặp hiện tại
                if (string.IsNullOrWhiteSpace(text)) continue;

                // Tạo một Socket sử dụng giao thức UDP
                var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                // Tạo endpoint chứa địa chỉ IP và cổng của server để gửi dữ liệu tới
                var sendEndPoint = new IPEndPoint(serverIp, serverPort);
                // Chuyển đổi chuỗi nhập thành mảng byte dùng Encoding ASCII
                var sendBuffer = Encoding.ASCII.GetBytes(text);

                // Gửi dữ liệu đến server thông qua socket tới endpoint đã chỉ định
                socket.SendTo(sendBuffer, sendEndPoint);

                // Chuẩn bị bộ đệm nhận dữ liệu từ server
                var size = 1024; // Kích thước bộ đệm nhận
                var receiveBuffer = new byte[size];
                // Tạo một EndPoint tạm (dummy) để nhận thông tin của endpoint gửi phản hồi
                EndPoint dummyEndPoint = new IPEndPoint(IPAddress.Any, 0);
                // Nhận dữ liệu từ server, số byte nhận được được lưu trong biến length
                int length = socket.ReceiveFrom(receiveBuffer, ref dummyEndPoint);
                // Chuyển đổi dữ liệu nhận được từ dạng byte sang chuỗi sử dụng Encoding ASCII
                var result = Encoding.ASCII.GetString(receiveBuffer, 0, length);

                // Hiển thị kết quả nhận được từ server ra màn hình
                Console.WriteLine($">>> {result}");
                // Đóng socket sau khi hoàn thành giao tiếp
                socket.Close();
            }
        }
    }
}
