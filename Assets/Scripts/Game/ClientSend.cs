using TicTacToeServer;
using UnityEngine;

public static class ClientSend
{

    /// <summary>Sent from client to server.</summary>
    public enum ClientPackets
    {
        WelcomeReceived = 1,
        RegisterAsPlayer = 2,
        PlayInSlot = 3,
        RequestGameStatusUpdate = 4
    }

    private static void SendTCPData(Packet packet)
    {
        packet.WriteLength();
        Client.Instance.Tcp.SendData(packet);
    }

    public static void WelcomeReceived()
    {
        Debug.Log($"Received welcome message. Returning response with client id {Client.Instance.MyId}");
        using(Packet packet = new Packet((int)ClientPackets.WelcomeReceived))
        {
            packet.Write($"Client é {Client.Instance.MyId}");
            packet.Write(Client.Instance.MyId);
            SendTCPData(packet);
        }
    }

    public static void RegisterAsPlayer()
    {
        Debug.Log($"Trying to register as player with client id {Client.Instance.MyId}");
        using(Packet packet = new Packet((int)ClientPackets.RegisterAsPlayer))
        {
            packet.Write(Client.Instance.MyId);
            SendTCPData(packet);
        }
    }
    
    internal static void PlayInSlot(int slotIndex)
    {  
        Debug.Log($"Sending played slot. Client {Client.Instance.MyId} : Slot {slotIndex}");
        using(Packet packet = new Packet((int)ClientPackets.PlayInSlot))
        {
            packet.Write(Client.Instance.MyId);
            packet.Write(slotIndex);
            SendTCPData(packet);
        }
    }    
    
    internal static void RequestGameStatusUpdate()
    {  
        Debug.Log($"RequestGameStatusUpdate. Client {Client.Instance.MyId}");
        using(Packet packet = new Packet((int)ClientPackets.RequestGameStatusUpdate))
        {
            packet.Write(Client.Instance.MyId);
            SendTCPData(packet);
        }
    }
}
