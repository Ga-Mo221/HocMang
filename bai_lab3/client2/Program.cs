using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace client2
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8; // Hỗ trợ Tiếng Việt
            Console.Title = "TCP Chat Client";
            Console.Write("Nhập tên của bạn: ");
            string username = Console.ReadLine();

            var client = new TcpClient("127.0.0.1", 1308);
            var stream = client.GetStream();
            var reader = new StreamReader(stream, Encoding.UTF8);
            var writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };

            // Tạo luồng để nhận tin nhắn từ server
            Thread listenThread = new Thread(() =>
            {
                while (true)
                {
                    try
                    {
                        string response = reader.ReadLine();
                        if (response != null)
                        {
                            Console.WriteLine(response);
                        }
                    }
                    catch
                    {
                        Console.WriteLine("Mất kết nối với server.");
                        break;
                    }
                }
            });
            listenThread.Start();

            // Vòng lặp để gửi tin nhắn
            while (true)
            {
                string message = Console.ReadLine();
                if (message.ToLower() == "exit")
                {
                    break; // Thoát chương trình nếu nhập "exit"
                }

                writer.WriteLine($"{username}: {message}");
            }

            // Đóng kết nối khi thoát
            client.Close();
        }
    }
}
