using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Secoket
{
    class Program
    {

        private static byte[] m_DataBuffer = new byte[1024];
        //设置端口号
        private static int m_Port = 8099;
        static Socket serverSocket;
        static void Main(string[] args)
        {
            //为了方便在本机上同时运行Client和server，使用回环地址为服务的监听地址
            IPAddress ip = IPAddress.Loopback;
            //实例化一个Socket对象，确定网络类型、Socket类型、协议类型
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //Socket对象绑定IP和端口号
            serverSocket.Bind(new IPEndPoint(ip, m_Port));
            //挂起连接队列的最大长度为15，启动监听
            serverSocket.Listen(1);

            Console.WriteLine("启动监听{0}成功", serverSocket.LocalEndPoint.ToString());
            //一个客户端连接服务器时创建一个新的线程
            Thread myThread = new Thread(ListenClientConnect);
            myThread.Start();
        }

        /// <summary>
        /// 接收连接
        /// </summary>
        private static void ListenClientConnect()
        {
            while (true)
            {
                //运行到Accept()方法是会阻塞程序(同步Socket)，
                //收到客户端请求创建一个新的Socket Client对象继续执行
                Socket clientSocket = serverSocket.Accept();
                clientSocket.Send(Encoding.UTF8.GetBytes("Server说：Client 你好！"));
                //创建一个接受客户端发送消息的线程
                Thread reciveThread = new Thread(ReciveMessage);
                reciveThread.Start(clientSocket);
            }
        }

        /// <summary>
        /// 接收信息
        /// </summary>
        /// <param name="clientSocket">包含客户端信息的套接字</param>
        private static void ReciveMessage(Object clientSocket)
        {
            if (clientSocket != null)
            {
                Socket m_ClientSocket = clientSocket as Socket;
                while (true)
                {
                    try
                    {
                        //通过clientSocket接收数据
                        int reciverNumber = m_ClientSocket.Receive(m_DataBuffer);
                        Console.WriteLine("接收客户端：{0}消息：{1}", m_ClientSocket.RemoteEndPoint.ToString(), Encoding.UTF8.GetString(m_DataBuffer, 0, reciverNumber));
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("111"+ex.Message);
                        m_ClientSocket.Shutdown(SocketShutdown.Both);
                        m_ClientSocket.Close();
                        break;
                    }
                }
            }
        }
    }


}

   

