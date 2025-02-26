using System;
using System.IO;
using System.Net.Sockets;
using System.Net;

namespace client_test
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Đặt tiêu đề cho cửa sổ Console của client
            Console.Title = "Em là đệ của anh";

            // Yêu cầu người dùng nhập địa chỉ IP của server (ở đây có thể nhập bằng tay)
            Console.WriteLine("Server ip: ");

            // Nếu muốn nhập địa chỉ IP từ bàn phím, có thể sử dụng dòng sau:
            // var address = IPAddress.Parse(Console.ReadLine());
            // Ở đây sử dụng địa chỉ IP localhost (127.0.0.1)
            var address = IPAddress.Parse("127.0.0.1");

            // Tạo đối tượng IPEndPoint với địa chỉ IP và cổng 1308 của server
            var serverEndpoint = new IPEndPoint(address, 1308);

            // Vòng lặp vô hạn để liên tục gửi yêu cầu đến server
            while (true)
            {
                // Thiết lập màu chữ cho prompt nhập lệnh
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("# COMMAND>>> ");
                Console.ResetColor();

                // Đọc chuỗi yêu cầu từ người dùng
                var request = Console.ReadLine();

                // Tạo một Socket kiểu TCP dùng cho kết nối đến server
                var client = new Socket(SocketType.Stream, ProtocolType.Tcp);

                // Kết nối đến server thông qua endpoint đã chỉ định
                client.Connect(serverEndpoint);

                // Tạo một NetworkStream từ socket để gửi/nhận dữ liệu
                var stream = new NetworkStream(client);

                // Khởi tạo StreamReader để đọc dữ liệu từ server
                var rd = new StreamReader(stream);
                // Khởi tạo StreamWriter để gửi dữ liệu đến server
                var wr = new StreamWriter(stream);

                // Gửi yêu cầu (request) đến server
                wr.WriteLine(request);
                // Đẩy dữ liệu ra ngoài stream, đảm bảo yêu cầu được gửi đi ngay
                wr.Flush();

                // Đọc phản hồi từ server
                var response = rd.ReadLine();
                // In ra phản hồi nhận được từ server
                Console.WriteLine($" > {response}");

                // Đóng kết nối socket sau khi hoàn thành giao tiếp với server
                client.Close();
            }
        }
    }
}
