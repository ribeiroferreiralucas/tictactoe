using System;
using TicTacToeServer;
using UnityEngine;

public static class ClientHandle
{
    public enum ServerPackets
    {
        Welcome = 1,
        RegisteredAsPlayer = 2,
        RegisterAsPlayerError = 3,
        SendStatus = 4,
        SendStatusForAll = 5

    }

    public static void AddHandlers()
    {
        Client.PacketHandlers.Clear();
        Client.ConnectionErrorCallback = ConnectionError;
        Client.Disconnected = DisconnectedFromServer;

        Client.PacketHandlers.Add((int)ServerPackets.Welcome, WelcomeHandle);
        Client.PacketHandlers.Add((int)ServerPackets.RegisteredAsPlayer, RegisteredAsPlayer);
        Client.PacketHandlers.Add((int)ServerPackets.RegisterAsPlayerError, RegisterAsPlayerError);
        Client.PacketHandlers.Add((int)ServerPackets.SendStatus, SendStatus);
        Client.PacketHandlers.Add((int)ServerPackets.SendStatusForAll, SendStatusForAll);
    }

    private static void ConnectionError()
    {
        GameServices.ConnectToServerError();
    }

    private static void DisconnectedFromServer()
    {
        Debug.Log($"Player as disconnected from server");
        GameServices.Disconnected();
    }
    public static void WelcomeHandle(Packet packet)
    {
        string msg = packet.ReadString();
        int myId = packet.ReadInt();
        Client.Instance.MyId = myId;

        Debug.Log($"Message from server: {msg}");
        ClientSend.WelcomeReceived();
        GameServices.ConnectedToServer();
    }

    public static void RegisteredAsPlayer(Packet packet)
    {
        int myId = packet.ReadInt();
        int myPlayerId = packet.ReadInt();

        Debug.Log($"Registered as Player: {myPlayerId}");

        GameServices.RegisteredAsPlayer(myPlayerId);
    }

    public static void RegisterAsPlayerError(Packet packet)
    {
        int myId = packet.ReadInt();
        string errorCode = packet.ReadString();

        Debug.Log($"Error to register as player: {errorCode}");

        GameServices.ErrorToRegisterAsPlayer(errorCode);
        Client.Instance.Disconnect();
    }

    private static void SendStatus(Packet packet)
    {
        int myId = packet.ReadInt();
        GameServices.CurrentStatus status = (GameServices.CurrentStatus) packet.ReadInt();
        
        int[] slots = new int[9];
        for (int i = 0; i < 9; i++)
        {
            slots[i] = packet.ReadInt();
        }

        GameServices.CurrentGameStatus(status, slots);
        
    }
    private static void SendStatusForAll(Packet packet)
    {
        GameServices.CurrentStatus status = (GameServices.CurrentStatus) packet.ReadInt();
        
        int[] slots = new int[9];
        for (int i = 0; i < 9; i++)
        {
            slots[i] = packet.ReadInt();
        }

        GameServices.CurrentGameStatus(status, slots);
    }

}