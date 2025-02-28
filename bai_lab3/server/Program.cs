using System;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Collections.Generic;
using System.Threading;
using System.Text;

namespace server
{
    internal class Program
    {
        static List<StreamWriter> clients = new List<StreamWriter>();
        static object lockObj = new object();

        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8; // Hỗ trợ hiển thị tiếng Việt
            Console.Title = "TCP Chat Server";

            var listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            listener.Bind(new IPEndPoint(IPAddress.Any, 1308));
            listener.Listen(10);

            Console.WriteLine("Server đang chạy, chờ kết nối...");

            while (true)
            {
                var worker = listener.Accept();
                Thread clientThread = new Thread(HandleClient);
                clientThread.Start(worker);
            }
        }

        static void HandleClient(object obj)
        {
            var socket = (Socket)obj;
            var stream = new NetworkStream(socket);
            var reader = new StreamReader(stream, Encoding.UTF8);  // Hỗ trợ Tiếng Việt
            var writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };

            lock (lockObj)
            {
                clients.Add(writer);
            }

            try
            {
                while (true)
                {
                    var message = reader.ReadLine();
                    if (message == null) break; // Nếu client ngắt kết nối

                    Console.WriteLine($"Client gửi: {message}");
                    BroadcastMessage(message, writer);
                }
            }
            catch
            {
                Console.WriteLine("Một client đã ngắt kết nối.");
            }
            finally
            {
                lock (lockObj)
                {
                    clients.Remove(writer);
                }
                writer.Close();
                reader.Close();
                stream.Close();
                socket.Close();
            }
        }

        static void BroadcastMessage(string message, StreamWriter sender)
        {
            lock (lockObj)
            {
                foreach (var client in clients)
                {
                    if (client != sender) // Gửi tin nhắn cho tất cả trừ người gửi
                    {
                        try
                        {
                            client.WriteLine(message);
                        }
                        catch
                        {
                            // Nếu gửi tin nhắn bị lỗi, có thể client đã ngắt kết nối
                        }
                    }
                }
            }
        }
    }
}
