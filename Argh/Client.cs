using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Security.Cryptography;
using Packets;

namespace ClientClasses
{
    
    public class Client
    {
        private ClientForm clientForm;
        private TcpClient m_tcpClient;
        private UdpClient m_udpClient;
        private NetworkStream m_stream;
        private BinaryReader m_reader;
        private BinaryWriter m_writer;
        private BinaryFormatter m_formatter;
        private RSACryptoServiceProvider m_RSAProvider;
        private RSAParameters m_PublicKey;
        private RSAParameters m_PrivateKey;
        private RSAParameters m_ClientKey;
        private object encryptLock;
        private object decryptLock;

        public Client()
        {
            m_tcpClient = new TcpClient();
            m_udpClient = new UdpClient();
            m_RSAProvider = new RSACryptoServiceProvider(1024);
            m_PublicKey = m_RSAProvider.ExportParameters(false);
            m_PrivateKey = m_RSAProvider.ExportParameters(true);
        }
        public bool Connect(string ip, int port)
        {
            try
            {
                m_udpClient.Connect(ip, port);
                // 
                m_tcpClient.Connect(ip,port);
                m_stream = m_tcpClient.GetStream();
                m_formatter = new BinaryFormatter();
                m_writer = new BinaryWriter(m_stream);
                m_reader = new BinaryReader(m_stream, Encoding.UTF8);
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
            //string userInput;
            clientForm = new ClientForm(this);
            Thread thread = new Thread(TcpProcessServerResponse);
            thread.Start();
            Thread threadUDP = new Thread(UdpProcessServerResponse);
            threadUDP.Start();
            clientForm.ShowDialog();
        }
        private void TcpProcessServerResponse()
        {
            while (m_reader != null)
            {
                try
                {
                    int numberOfBytes;
                    if ((numberOfBytes = m_reader.ReadInt32()) != -1)
                    {
                        byte[] buffer = m_reader.ReadBytes(numberOfBytes);
                        MemoryStream memStream = new MemoryStream(buffer);
                        Packet packet = m_formatter.Deserialize(memStream) as Packet;
                        switch (packet.packetType)
                        {
                            case PacketType.CHATMESSAGE:
                                ChatMessagePacket chatPacket = (ChatMessagePacket)packet;
                                clientForm.UpdateChatWindow(chatPacket.m_message);
                                break;
                            case PacketType.PRIVATEMESSAGE:
                                PrivateMessagePacket privatePacket = (PrivateMessagePacket)packet;
                                clientForm.UpdateChatWindow(privatePacket.m_name + " : " + privatePacket.m_message);
                                break;
                            case PacketType.CLIENTNAME:
                                ClientNamePacket namePacket = (ClientNamePacket)packet;
                                clientForm.UpdateClientList(namePacket.m_newName,namePacket.m_oldName);
                                clientForm.UpdateChatWindow(namePacket.m_newName + " was " + namePacket.m_oldName);
                                break;
                            case PacketType.DISCONNECT:
                                DisconnectPacket disconnectPacket = (DisconnectPacket)packet;
                                clientForm.Close();
                                Close();
                                break;
                            case PacketType.EMPTY:
                                break;
                        }
                    }
                }
                catch
                {
                    Console.WriteLine("Client has been forcefully closed!");
                }
            }
        }
        public void SendMessage(Packet packet)
        {
            MemoryStream memstream = new MemoryStream();
            m_formatter.Serialize(memstream,packet);
            byte[] buffer = memstream.GetBuffer();
            m_writer.Write(buffer.Length);
            m_writer.Write(buffer);
            m_writer.Flush();
        }
        public void SendData(string message,string name,int option)
        {
            switch (option)
            {
                case 0:
                    SendMessage(new ChatMessagePacket(message));
                    break;
                case 1:
                    SendMessage(new PrivateMessagePacket(message,name));
                    break;
                case 2:
                    SendMessage(new ClientNamePacket(message,name));
                    break;
                case 3:
                    if (message == "Left")
                        SendMessage(new DisconnectPacket(message, name));
                    UdpSendMessage(new DisconnectPacket(message, name));
                    break;

            }
            
        }
        public void Login(string name)
        {
            SendMessage(new LoginPacket(name, (IPEndPoint)m_udpClient.Client.LocalEndPoint));
        }
        public void UdpSendMessage(Packet packet)
        {
            MemoryStream memstream = new MemoryStream();
            m_formatter.Serialize(memstream, packet);
            byte[] buffer = memstream.GetBuffer();
            m_udpClient.Send(buffer, buffer.Length);
        }
        public void UdpProcessServerResponse()
        {
            try
            {

                IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, 0);
                while(true)
                {
                    byte[] bytes = m_udpClient.Receive(ref endPoint);
                    MemoryStream memStream = new MemoryStream(bytes);
                    Packet packet = m_formatter.Deserialize(memStream) as Packet;
                    if (packet.packetType == PacketType.DISCONNECT)
                    {
                        DisconnectPacket disconnectPacket = (DisconnectPacket)packet;
                        clientForm.UpdateClientList(disconnectPacket.m_message, disconnectPacket.m_name);
                        clientForm.UpdateChatWindow(disconnectPacket.m_name + " has " + disconnectPacket.m_message);
                    }
                    
                }
            }
            catch (SocketException e)
            {

                Console.WriteLine("Client UDP Read Method exception: " ,e.Message);

            }
            
        }
        private byte[] Encrypt(byte[] data)
        {
            lock (encryptLock)
            {
                m_RSAProvider.ImportParameters(m_ClientKey);
                return m_RSAProvider.Encrypt(data, true);
            }

        }

        private byte[] Decrypt(byte[] data)
        {
            lock (decryptLock)
            {
                m_RSAProvider.ImportParameters(m_ClientKey);
                return m_RSAProvider.Decrypt(data, true);
            }
        }

        private byte[] EncryptString(string message)
        {
            byte[] deByte = Encoding.UTF8.GetBytes(message);
            return deByte;
        }

        private string DecryptString(byte[] message)
        {
            Decrypt(message);
            string deMessage = Encoding.UTF8.GetString(message);
            return deMessage;
        }
        public void Close()
        {
            m_reader.Close();
            m_reader = null;
            m_writer.Close();
            m_writer = null;
            m_stream.Close();
            m_stream = null;
            m_udpClient.Close();
            m_tcpClient.Close();
        }
    }
}
