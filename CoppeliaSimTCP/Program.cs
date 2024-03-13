using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SocketExample
{
    class Program
    {
        private static TcpClient socket;
        private static int counter;

        // cím
        private static string ip_address = "127.0.0.1";
        private static int port_number = 25455;

        static void Main()
        {
            socket = new TcpClient();
            counter = 0;

            Console.WriteLine("Press Enter to connect/disconnect...");
            Console.ReadLine();

            if (!socket.Connected)
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
            Console.ReadLine();
        }

        private static void ConnectSockets()
        {
            try
            {
                socket.Connect(ip_address, port_number);
                int timeout_ms = 100;
                if (socket.Client.Poll(timeout_ms, SelectMode.SelectWrite))
                {
                    Task.Run(() => BeginReading(socket, readyRead));
                    Console.WriteLine("Socket connected");
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
            if (socket.Connected)
            {
                socket.Close();
                Console.WriteLine("Socket connection closed");
            }
        }

        private static async void BeginReading(TcpClient socket, Action<string> handler)
        {
            try
            {
                NetworkStream stream = socket.GetStream();
                byte[] buffer = new byte[1024];

                while (socket.Connected)
                {
                    int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                    if (bytesRead > 0)
                    {
                        string data = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                        Console.WriteLine("Socket reading...");
                        handler(data);
                    }
                    else
                    {
                        DisconnectSockets();
                        Console.WriteLine("Socket disconnected");
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error reading from socket: " + ex.Message);
                DisconnectSockets();
            }
        }

        // Socket olvasása
        private static void readyRead(string line)
        {
            Console.WriteLine("Socket message: " + line);
        }
    }
}
