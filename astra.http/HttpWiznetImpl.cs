/*
 * HttpLibrary implementation for a TCP Socket, such as the one from Netduino Plus
 *      
 * Use this code for whatever you want. Modify it, redistribute it, at will
 * Just keep this header intact, however, and add your own modifications to it!
 * 
 * 29 Jan 2011  -- Quiche31 - Initial release, tested OK with Netduino Plus
 * 
 * */
using System;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using Microsoft.SPOT;

namespace astra.http
{
    public class HttpWiznetImpl : HttpImplementation
    {
        private Socket m_listeningSocket = null;
        private Socket m_clientSocket = null;
        private HttpImplementationClient.RequestReceivedDelegate m_requestReceived = null;
        private const int maxRequestSize = 1024;

        public HttpWiznetImpl(HttpImplementationClient.RequestReceivedDelegate requestReceived, int localPort = 80)
        {
            m_requestReceived = requestReceived;
            m_listeningSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            m_listeningSocket.Bind(new IPEndPoint(IPAddress.Any, localPort));
            m_listeningSocket.Listen(10);
        }

        public void Write(String response)
        {
            BinaryWrite(Encoding.UTF8.GetBytes(response));
        }
        public void BinaryWrite(byte[] response)
        {
            m_clientSocket.Send(response);
        }
        public void Close()
        {
            m_clientSocket.Close();
        }

        public String getIP()
        {
            return m_listeningSocket.LocalEndPoint.ToString();
        }

        public void Listen()
        {
            while (true)
            {
                using (Socket clientSocket = m_listeningSocket.Accept())
                {
                    int wait = 2;
                    m_clientSocket = clientSocket;
                    StringBuilder line = new StringBuilder();
                    HttpRequest request = new HttpRequest();
                    IPEndPoint clientIP = clientSocket.RemoteEndPoint as IPEndPoint;
                    //Debug.Print("\nReceived request from " + clientIP.ToString());
                    // Need to read from socket more than once, as POST requests can be issued in multiple writes
                    while(wait-- >=0 )
                    {
                        int availableBytes = clientSocket.Available;
                        //Debug.Print(DateTime.Now.ToString() + " " + availableBytes.ToString() + " request bytes available");

                        int bytesReceived = (availableBytes > maxRequestSize ? maxRequestSize : availableBytes);
                        if (bytesReceived > 0)
                        {
                            byte[] buffer = new byte[bytesReceived]; // Buffer probably should be larger than this.
                            int readByteCount = clientSocket.Receive(buffer, bytesReceived, SocketFlags.None);
                            String contents = new String(Encoding.UTF8.GetChars(buffer));
                            line.Append(contents);
                        }                        
                        Thread.Sleep(10);
                    }
                    String[] lines = line.ToString().Split('\n');
                    line.Clear();
                    new HttpRequestParser().parse(request, new HttpRequestLines(lines));
                    if (m_requestReceived != null)
                        m_requestReceived(new HttpContext(request, new HttpResponse(this)));
                }
            }
        }
    }
}
