using System;
using System.IO;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections.Concurrent;
using System.Threading;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using Packets;

namespace Server
{
    class Client
    {
        private Socket socket;
        private NetworkStream stream;
        private BinaryReader reader;
        private BinaryWriter writer;
        private BinaryFormatter formatter;
        private object readLock;
        private object writeLock;
        private RSACryptoServiceProvider m_RSAProvider;
        private RSAParameters m_PublicKey;
        private RSAParameters m_PrivateKey;
        private RSAParameters m_ServerKey;
        private object encryptLock;
        private object decryptLock;
        public IPEndPoint m_udpEndPoint;
        public string m_clientName;

        public Client(Socket socket)
        {
            readLock = new object();
            writeLock = new object();
            m_RSAProvider = new RSACryptoServiceProvider(1024);
            m_PublicKey = m_RSAProvider.ExportParameters(false);
            m_PrivateKey = m_RSAProvider.ExportParameters(true);
            formatter = new BinaryFormatter();
            stream = new NetworkStream(socket);
            reader = new BinaryReader(stream, Encoding.UTF8);
            writer = new BinaryWriter(stream);
        }
        public void Close()
        {
            reader.Close();
            writer.Close();
            stream.Close();
            
        }
        public Packet TCPRead()
        {
            lock (readLock)
            {
                try
                {
                    int numberOfBytes;
                    if ((numberOfBytes = reader.ReadInt32()) != -1)
                    {
                        byte[] buffer = reader.ReadBytes(numberOfBytes);
                        MemoryStream memStream = new MemoryStream(buffer);
                        Packet packet = formatter.Deserialize(memStream) as Packet;
                        return packet;
                    }
                }
                catch (IOException e)
                {
                    Console.Write("TCP Recieve Failed " + e.Message);
                }
                return null;
            }
        }
        public void TCPSend(Packet packet)
        {
            lock (writeLock)
            {
                MemoryStream memstream = new MemoryStream();
                formatter.Serialize(memstream, packet);
                byte[] buffer = memstream.GetBuffer(); //.GetArray()
                writer.Write(buffer.Length);
                writer.Write(buffer);
                writer.Flush();
            }
        }
        private byte[] Encrypt(byte[] data)
        {
            lock(encryptLock)
            {
                m_RSAProvider.ImportParameters(m_ServerKey);
                return m_RSAProvider.Encrypt(data, true);
            }
        }

        private byte[] Decrypt(byte[] data)
        {
            lock (decryptLock)
            {
                m_RSAProvider.ImportParameters(m_ServerKey);
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
    }
    class Server
    {
        private TcpListener _tcpListener;
        private ConcurrentDictionary<int,Client> m_Clients;
        private UdpClient m_udpListener;
        private string tempLogin;
        private string temp;
        public Server(string ipAddress, int port)
        {
            tempLogin = "Unconnected";
            IPAddress _ipaddress;
            _ipaddress = IPAddress.Parse(ipAddress);
            _tcpListener = new TcpListener(_ipaddress, port);
            m_udpListener = new UdpClient(port);
            Thread udpThread = new Thread(() => { UdpListen(); });
            udpThread.Start();
        }
        public void Start()
        {
            m_Clients = new ConcurrentDictionary<int, Client>();
            int clientIndex = 0;
            while (true)
            {
                int index = clientIndex;
                clientIndex++;
                _tcpListener.Start();
                Socket socket = _tcpListener.AcceptSocket();
                Client client = new Client(socket);
                m_Clients.TryAdd(index, client);
                foreach (KeyValuePair<int, Client> c in m_Clients)
                {
                    if (c.Value.m_clientName == null)
                        c.Value.m_clientName = tempLogin;
                }
                Thread thread = new Thread(() => { ClientMethod(index); });
                thread.Start();
            }
        }   
        public void Stop()
        {
            _tcpListener.Stop();
        }
        private void UdpListen()
        {
            try
            {
                IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, 0);
                while (true)
                {
                    byte[] bytes = m_udpListener.Receive(ref endPoint);
                    MemoryStream memStream = new MemoryStream(bytes);
                    Packet packet = new BinaryFormatter().Deserialize(memStream) as Packet;
                    Console.WriteLine("Packet recieved");
                    foreach (KeyValuePair<int, Client> c in m_Clients)
                    {
                        if (c.Value.m_udpEndPoint == null)
                            break;
                        if (endPoint.ToString() != c.Value.m_udpEndPoint.ToString())
                        {
                            if (packet.packetType == PacketType.DISCONNECT)
                            {
                                DisconnectPacket disconnectPacket = (DisconnectPacket)packet;
                                MemoryStream memstream = new MemoryStream();
                                new BinaryFormatter().Serialize(memstream, disconnectPacket);
                                byte[] buffer = memstream.GetBuffer();
                                m_udpListener.Send(buffer, buffer.Length, c.Value.m_udpEndPoint);
                            }
                        }
                    }
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine("Client UDP Read Method exception: ", e.Message);
            }
        }
        private void ClientMethod(int index)
        {
            Packet packet;
            while (m_Clients.ContainsKey(index) == true)
            {
                if ((packet = m_Clients[index].TCPRead()) != null)
                {
                    Console.WriteLine("Recieved...");
                    switch (packet.packetType)
                    {
                        case PacketType.CHATMESSAGE:
                            ChatMessagePacket chatPacket = (ChatMessagePacket)packet;
                            foreach (int i in m_Clients.Keys)
                            {
                                if (i != index)
                                    m_Clients[i].TCPSend(chatPacket);
                            }
                        break;
                        case PacketType.PRIVATEMESSAGE:
                            PrivateMessagePacket privatePacket = (PrivateMessagePacket)packet;
                            foreach(int i in m_Clients.Keys)
                            {
                                if (m_Clients[i].m_clientName == privatePacket.m_name)
                                    m_Clients[i].TCPSend(new PrivateMessagePacket(privatePacket.m_message,m_Clients[index].m_clientName));
                            }
                        break;
                        case PacketType.CLIENTNAME:
                            ClientNamePacket namePacket = (ClientNamePacket)packet;
                            m_Clients[index].m_clientName = namePacket.m_newName;
                            foreach (int i in m_Clients.Keys)
                            {  
                                m_Clients[i].TCPSend(new ClientNamePacket(m_Clients[index].m_clientName, namePacket.m_oldName));
                            }
                        break;
                        case PacketType.LOGIN:
                            LoginPacket loginPacket = (LoginPacket)packet;
                            m_Clients[index].m_clientName = loginPacket.m_name;
                            foreach (int i in m_Clients.Keys)
                            {
                                temp = m_Clients[i].m_clientName;
                                m_Clients[index].TCPSend(new ClientNamePacket(temp, m_Clients[index].m_clientName));
                            }
                            m_Clients[index].m_udpEndPoint = IPEndPoint.Parse(loginPacket.m_endPoint);
                        break;
                        case PacketType.DISCONNECT:
                            DisconnectPacket disconnectPacket = (DisconnectPacket)packet;
                            m_Clients[index].TCPSend(disconnectPacket);
                            m_Clients[index].Close();
                            Client c;
                            m_Clients.Remove(index, out c);
                        break;
                    case PacketType.EMPTY:
                            break;
                    }
                }
            }
        }
    }
}
