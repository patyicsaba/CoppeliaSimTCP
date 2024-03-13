using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

// TODO
// Abrupt megszakított kapcsolat esetén új kap

namespace SocketExample
{
    class Program
    {
        private static TcpClient socket1;
        private static TcpClient socket2;
        private static int counter;

        // cím
        private static string ip_address = "127.0.0.1";
        private static int port_number = 25456;

        static void Main()
        {
            socket1 = new TcpClient();
            socket2 = new TcpClient();
            counter = 0;

            Console.WriteLine("Press Enter to connect/disconnect...");
            Console.ReadLine();

            if (!socket1.Connected)
            {
                ConnectSockets();
                Console.WriteLine("Connected");
            }
            else
            {
                DisconnectSockets();
                Console.WriteLine("Disconnected");
            }

            // Keep console open
            Console.ReadLine(); ;
        }

        private static void ConnectSockets()
        {
            try
            {
                socket1.Connect(ip_address, port_number);
                socket2.Connect(ip_address, port_number);
                int timeout_ms = 100;
                if (socket1.Client.Poll(timeout_ms, SelectMode.SelectWrite) && socket2.Client.Poll(timeout_ms, SelectMode.SelectWrite))
                {
                    BeginReading(socket1, readyRead1);
                    BeginReading(socket2, readyRead2);
                    Console.WriteLine("Socket1 és Socket2 csatlakoztatva");
                }
                else
                {
                    DisconnectSockets();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error Connecting: " + ex.Message);
                DisconnectSockets();
            }
        }

        private static void DisconnectSockets()
        {
            if (socket1.Connected)
            {
                socket1.Close();
                Console.WriteLine("Socket 1 connection closed");
            }
            if (socket1.Connected)
            {
                socket2.Close();
                Console.WriteLine("Socket 2 connection closed");
            }
        }

        private static void BeginReading(TcpClient socket, Action<string> handler)
        {
            NetworkStream stream = socket.GetStream();
            byte[] buffer = new byte[1024];
            stream.BeginRead(buffer, 0, buffer.Length, (result) =>
            {
                int bytesRead = stream.EndRead(result);
                if (bytesRead > 0)
                {
                    string data = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                    Console.WriteLine("Socket olvasása");
                    handler(data);
                    BeginReading(socket, handler);  // olvasás folytatása
                }
                else
                {
                    DisconnectSockets();
                    Console.WriteLine("Socket lecsatlakoztatása");
                }
            }, null);
        }

        // Socket 1 olvasása
        private static void readyRead1(string line)
        {
            Console.WriteLine("Socket üzenet: " + line);
        }


        // Socket 2 olvasása
        private static void readyRead2(string line)
        {
            Console.WriteLine("Socket üzenet: " + line);
        }

    }
}