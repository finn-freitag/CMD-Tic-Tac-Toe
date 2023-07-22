using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CMD_Tic_Tac_Toe
{
    public static class TcpExtensions
    {
        public delegate string CreateResponse(string request);

        public static string MakeRequest(string host, int port, string content)
        {
            TcpClient client = new TcpClient();
            client.Connect(host, port);
            if (client.Connected)
            {
                NetworkStream ns = client.GetStream();
                byte[] bytes = Encoding.UTF8.GetBytes(content);
                ns.Write(bytes);
                Thread.Sleep(500);
                byte[] recbytes = ns.ReadBytes();
                return Encoding.UTF8.GetString(recbytes);
            }
            else
            {
                throw new CannotConnectToServerException(host, port);
            }
        }

        public static void HostServer(int port, CreateResponse createResponse)
        {
            TcpListener listener = TcpListener.Create(port);
            listener.Start();
            while (true)
            {
                if (listener.Pending())
                {
                    TcpClient client = listener.AcceptTcpClient();
                    NetworkStream ns = client.GetStream();
                    byte[] bytes = ns.ReadBytes();
                    string response = createResponse(Encoding.UTF8.GetString(bytes));
                    ns.Write(Encoding.UTF8.GetBytes(response));
                    ns.Flush();
                    ns.Close();
                    ns.Dispose();
                    client.Close();
                    client.Dispose();
                }
            }
        }

        public static byte[] ReadBytes(this NetworkStream ns)
        {
            int i = 0;
            while (!ns.DataAvailable)
            {
                Thread.Sleep(50);
                if (i * 50 > 5000) throw new TimeoutException();
                i++;
            }
            byte[] bytes = new byte[50000];
            byte[] finalbytes = new byte[0];
            bool run = true;
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            while (run && ns.DataAvailable)
            {
                int length = ns.Read(bytes, 0, bytes.Length);
                byte[] b = new byte[length];
                for (int l = 0; l < length; l++)
                {
                    if (bytes[l] != 3)
                    {
                        b[l] = bytes[l];
                    }
                    else
                    {
                        run = false;
                        l = length;
                    }
                }
                finalbytes = Combine(finalbytes, b);
                if (sw.ElapsedMilliseconds > 5000)
                {
                    sw.Stop();
                    throw new TimeoutException();
                }
                //if (!ns.DataAvailable) Thread.Sleep(2000);
            }
            List<byte> final = finalbytes.ToList();
            final.RemoveAt(final.Count - 1);
            finalbytes = final.ToArray();
            return finalbytes;
        }

        public static void Write(this NetworkStream ns, byte[] bytes)
        {
            byte[] res = Combine(bytes, new byte[] { 3 });
            ns.Write(res, 0, res.Length);
        }

        private static byte[] Combine2(params byte[][] arrays) // to slow
        {
            byte[] rv = new byte[arrays.Sum(a => a.Length)];
            int offset = 0;
            foreach (byte[] array in arrays)
            {
                System.Buffer.BlockCopy(array, 0, rv, offset, array.Length);
                offset += array.Length;
            }
            return rv;
        }

        private static byte[] Combine(params byte[][] arrays) // to slow
        {
            MemoryStream ms = new MemoryStream();
            foreach (byte[] b in arrays)
            {
                ms.Write(b, 0, b.Length);
            }
            return ms.ToArray();
        }

        private static byte[] Combine3(params byte[][] arrays)
        {
            byte[] finalbytes = new byte[0];
            foreach (byte[] b in arrays)
            {
                finalbytes = finalbytes.Concat(b).ToArray();
            }
            return finalbytes;
        }
    }

    public class CannotConnectToServerException : Exception
    {
        public string Host;
        public int Port;

        public CannotConnectToServerException(string host, int port)
        {
            this.Host = host;
            this.Port = port;
        }

        public override string Message
        {
            get
            {
                return "Server " + Host + ":" + Port + " isn't available!";
            }
        }

        public override IDictionary Data
        {
            get
            {
                IDictionary data = base.Data;
                data.Add("Host", Host);
                data.Add("Port", Port);
                return data;
            }
        }
    }
}
