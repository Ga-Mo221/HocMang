﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace bai1_server
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //1. Listen
            IPAddress address = IPAddress.Parse("127.0.0.1");
            TcpListener listener = new TcpListener(address, 8888);
            Console.WriteLine("Sever is listening...");
            listener.Start();
            Socket socket = listener.AcceptSocket();
            //2. Receive
            byte[] data = new byte[1024];
            socket.Receive(data);
            string str = Encoding.ASCII.GetString(data);
            Console.WriteLine("Client name: \"" + str + "\"");
            //3. Send
            socket.Send(Encoding.ASCII.GetBytes("Hello, " + str));
            //4. Close
            Console.WriteLine("Server is closing...");
            socket.Close();
            listener.Stop();
        }
    }
}
