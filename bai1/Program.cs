using System;
using System.Net;

namespace bai1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string name = args.Length < 1 ? Dns.GetHostName(): args[0]; 
            try
            {
                IPAddress[] ips = Dns.Resolve(name).AddressList;
                foreach (IPAddress ip in ips)
                {
                    Console.WriteLine("{0}-{1}", name, ip);
                }
            }
            catch (Exception ex) 
            {
                Console.WriteLine(ex);
            }

            Console.ReadLine();
        }
    }
}
