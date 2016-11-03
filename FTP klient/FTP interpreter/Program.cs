using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace FTP_interpreter
{
    class Program
    {

        static void Main(string[] args)
        {
            TextWriter output = Console.Out;
            using (output = new StreamWriter("log.txt"))
            {
                var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socket.Connect(new IPEndPoint(IPAddress.Parse("192.168.1.219"), 12345));

                var data = new byte[1024];

                DateTime prevPoint = DateTime.Now;
                int kbSent = 0;
                BigInteger kbsTotal = 0;

                while (true)
                {
                    try
                    {
                        socket.Send(data);
                        kbSent++;
                        kbsTotal++;
                    }
                    catch
                    {
                        break;
                    }

                    var now = DateTime.Now;
                    var timeDelta = now - prevPoint;
                    if (timeDelta.TotalMilliseconds >= 1000)
                    {
                        output.WriteLine($"{kbSent} kB/s at {now} total sent data {kbsTotal} kB");
                        Console.WriteLine($"{kbSent} kB/s at {now} total sent data {kbsTotal} kB");
                        kbSent = 0;
                        prevPoint = now;
                    }
                }
            }

        }

        private void Run()
        {
            Socket s2 = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            s2.ReceiveTimeout = 2000;
            s2.SendTimeout = 1000;

            IPAddress ip = IPAddress.Parse("88.86.117.153");

            IPEndPoint ep = new IPEndPoint(ip, 21);
            s2.Connect(ep);

            string command = null;
            byte[] response = new byte[128];
            int length = 0;

            try
            {
                length = s2.Receive(response);
            }
            catch (SocketException)
            {
                Console.WriteLine("receive timeout reached!");
            }

            Console.WriteLine(Encoding.ASCII.GetString(response, 0, length));

            while ((command = Console.ReadLine()) != "exit")
            {

                Socket dataConnection = null;

                switch (command)
                {
                    case "nlst":
                    case "list":
                        dataConnection = CreateDataConnection();
                        break;
                }

                try
                {
                    s2.Send(Encoding.ASCII.GetBytes(command + "\r\n")); //!!!!
                }
                catch (SocketException)
                {
                    Console.WriteLine("send time out reached!");
                    continue;
                }


                try
                {
                    length = s2.Receive(response);
                }
                catch (SocketException)
                {
                    Console.WriteLine("receive timeout reached!");
                    continue;
                }

                switch (command)
                {
                    case "pasv":
                        EnterPassiveMode(Encoding.ASCII.GetString(response, 0, length));
                        break;
                    case "nlst":
                    case "list":

                        var buffer = new byte[1024];
                        int datLen = dataConnection.Receive(buffer);

                        Console.WriteLine(Encoding.ASCII.GetString(buffer, 0, datLen));
                        dataConnection.Shutdown(SocketShutdown.Both);
                        dataConnection.Close();

                        s2.Receive(response);
                        break;
                }


                Console.WriteLine(Encoding.ASCII.GetString(response, 0, length));

                response = new byte[128];
            }

            Console.WriteLine("closing connection...");

            s2.Shutdown(SocketShutdown.Both);
            s2.Close();
        }


        bool passiveMode = false;
        IPEndPoint serverDataPort = null;

        private void EnterPassiveMode(string pasvResponse)
        {
            int first = pasvResponse.IndexOf('(');
            int last = pasvResponse.IndexOf(')');

            string ipp = pasvResponse.Substring(first + 1, last - first - 1);

            var ipq = ipp.Split(new char[] { ',' });

            IPAddress target = IPAddress.Parse(string.Format("{0}.{1}.{2}.{3}", ipq[0], ipq[1], ipq[2], ipq[3]));

            passiveMode = true;
            serverDataPort = new IPEndPoint(target, int.Parse(ipq[4]) * 256 + int.Parse(ipq[5]));
        }

        private Socket CreateDataConnection()
        {
            if (!passiveMode)
                throw new InvalidOperationException("Can not created data connection in activeMode");

            Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            s.ReceiveTimeout = 2000;
            s.SendTimeout = 1000;

            s.Connect(serverDataPort);

            return s;
        }

    }


}
