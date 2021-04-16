using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System;
using TicTacToeServer;

public class Client : MonoBehaviour
{
    public static Client Instance;
    public static int DataBufferSize = 4098;

    public string Ip = "127.0.0.1";
    public int Port = 26950;
    public int MyId = 0;
    public TCP Tcp;

    private bool _isConnected;

    public delegate void PacketHandler(Packet packet);
    public static readonly Dictionary<int, PacketHandler> PacketHandlers = new Dictionary<int, PacketHandler>();
    public static Action ConnectionErrorCallback;
    public static Action Disconnected;

    private void Awake() 
    {
        if( Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.Log("Instance already existis, destroing object!");
            Destroy(this);
        }
    }
    private void OnDestroy()
    {
        Disconnect();
    }

    public void Disconnect()
    {
        if(_isConnected)
        {
            _isConnected = false;

            Tcp.Socket.Close();

            Debug.Log("Disconnected");
            Disconnected?.Invoke();
        }
    }

    private void Start()
    {
        Tcp = new TCP();
    }

    public void ConnectToServer()
    {
        _isConnected = true;
        Tcp.Connect();
    }

    public class TCP
    {
        public TcpClient Socket;
        private NetworkStream _stream;
        private Packet _receivedData;
        private byte[] _receiveBuffer;

        public void Connect()
        {
            Socket = new TcpClient
            {
                ReceiveBufferSize = DataBufferSize,
                SendBufferSize = DataBufferSize
            };

            _receiveBuffer = new byte[DataBufferSize];
            Socket.BeginConnect(Instance.Ip, Instance.Port, ConnectCallback, Socket);
        }

        private void ConnectCallback(IAsyncResult result)
        {
            Socket.EndConnect(result);
            if(!Socket.Connected)
            {

                return;   
            }

            _stream = Socket.GetStream();

            _receivedData = new Packet();

            _stream.BeginRead(_receiveBuffer, 0, DataBufferSize, ReceiveCallback, null);
        }

        private void ReceiveCallback(IAsyncResult result)
        {
            try
            {
                int byteLenght = _stream.EndRead(result);
                if(byteLenght <= 0 )
                {
                    Instance.Disconnect();
                    return;
                }

                byte[] data = new byte[byteLenght];
                Array.Copy(_receiveBuffer, data, byteLenght);

                _receivedData.Reset(HandleData(data));
                _stream.BeginRead(_receiveBuffer, 0, DataBufferSize, ReceiveCallback, null);
            }
            catch (System.Exception ex)
            {
                Console.WriteLine($"Error receiving TCP data: {ex}");
                Disconnect();
            }
        }

        private bool HandleData(byte[] data)
        {
            int packetLenght = 0;

            _receivedData.SetBytes(data);

            if(_receivedData.UnreadLength() >= 4)
            {
                packetLenght = _receivedData.ReadInt();
                if(packetLenght <= 0)
                {
                    return true;
                }
            }

            while(packetLenght> 0 && packetLenght <= _receivedData.UnreadLength())
            {
                byte[] packetBytes = _receivedData.ReadBytes(packetLenght);
                ThreadManager.ExecuteOnMainThread(()=>
                {
                    using (Packet packet = new Packet(packetBytes))
                    {
                        int packetId = packet.ReadInt();
                        Debug.Log($"A packet with id {packetId} has arive.");
                        PacketHandlers[packetId](packet);
                    }
                });

            }
            packetLenght = 0;
            if(_receivedData.UnreadLength() >= 4)
            {
                packetLenght = _receivedData.ReadInt();
                if(packetLenght <= 0)
                {
                    return true;
                }
            }

            return packetLenght <= 1;
        }

        internal void SendData(Packet packet)
        {
            try
            {
                if(Socket == null)
                    return;
                
                _stream.BeginWrite(packet.ToArray(), 0, packet.Length(), null, null);
            }
            catch (System.Exception ex)
            {
                Debug.Log($"Error sending data to server via TCP: {ex}");
            }
        }

        public void Disconnect()
        {
            Instance.Disconnect();

            _stream = null;
            _receivedData = null;
            _receiveBuffer = null;
            Socket = null;
        }
    }

}
