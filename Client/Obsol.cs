using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace Client
{
    
    class Client
    {
        /* HELP
         TcpClient ourClient = (TcpClient)addedClient;
         NetworkStream stream = ourClient.GetStream();
         NetworkStream stream = new NetworkStream(socket);
         StreamReader reader = new StreamReader(stream, Encoding.UTF8);
         StreamWriter writer = new StreamWriter(stream, Encoding.UTF8);
        */
        private TcpClient _tcpClient;
        private NetworkStream _stream;
        private StreamWriter _writer;
        private StreamReader _reader;
        public Client()
        {
            _tcpClient = new TcpClient();
        }
        public bool Connect(string ip, int port)
        {
            try
            {
                _tcpClient.Connect(ip,port);
                _stream = _tcpClient.GetStream();
                _writer = new StreamWriter(_stream, Encoding.UTF8);
                _reader = new StreamReader(_stream, Encoding.UTF8);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
                return false;
            }
            
        }
        public void Run()
        {
            string userInput;
            ProcessServerResponse();
            Console.WriteLine(" Awaiting Input \n");
            while ((userInput = Console.ReadLine()) != null)
            {
                _writer.WriteLine(userInput);
                _writer.Flush();
                if (userInput.ToLower() == "close")
                    break;
                ProcessServerResponse();
                
            }
            _tcpClient.Close();
            Console.WriteLine("Press Enter to close");
        }
        private void ProcessServerResponse()
        {
            Console.WriteLine("Server Response Recieved: ");
            Console.WriteLine(_reader.ReadLine());
            Console.WriteLine();
        }
    }
}
