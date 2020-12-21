using System;
using System.Net;

namespace Packets
{
    [Serializable]
    public enum PacketType
    {
        EMPTY,
        CHATMESSAGE,
        PRIVATEMESSAGE,
        CLIENTNAME,
        LOGIN,
        DISCONNECT

    }
    [Serializable]
    public abstract class Packet
    {
        public PacketType packetType { get; protected set; }
    }
    [Serializable]
    public class ChatMessagePacket : Packet
    {
        public string m_message { get; private set; }
        public ChatMessagePacket(string message)
        {
            m_message = message;
            packetType = PacketType.CHATMESSAGE;
        }
    }
    [Serializable]
    public class PrivateMessagePacket : Packet
    {
        public string m_name { get; private set; }
        public string m_message { get; private set; }
        public PrivateMessagePacket(string message, string name)
        {
            m_message = message;
            m_name = name;
            packetType = PacketType.PRIVATEMESSAGE;
        }
    }
    [Serializable]
    public class ClientNamePacket : Packet
    {
        public string m_oldName { get; private set; }
        public string m_newName { get; private set; }
        public ClientNamePacket(string name, string oldname)
        {
            m_newName = name;
            m_oldName = oldname;
            packetType = PacketType.CLIENTNAME;
        }
    }

    [Serializable] 
    public class LoginPacket : Packet
    {
        public string m_name { get; private set; }
        public string m_endPoint { get; private set; }
        public LoginPacket(string name , IPEndPoint endPoint)
        {
            m_name = name;
            m_endPoint = endPoint.ToString();

            packetType = PacketType.LOGIN;
        }
    }
    [Serializable]
    public class DisconnectPacket : Packet
    {
        public string m_name { get; private set; }
        public string m_message { get; private set; }
        public DisconnectPacket(string message ,string name)
        {
            m_name = name;
            m_message = message;

            packetType = PacketType.DISCONNECT;
        }
    }
}