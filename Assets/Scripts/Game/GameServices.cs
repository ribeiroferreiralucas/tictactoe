using System;

internal class GameServices
{
    public enum CurrentStatus
    {
        WaitingOtherPlayerToConnect = 0,
        Player1Turn = 1,
        Player2Turn = 2,
        FinishedWithTie = 3,
        FinishedWithPlayer1Victory = 4, 
        FinishedWithPlayer2Victory = 5,
        
    }
    public static int PlayerId { get; private set; }
    public static int[] Slots { get; private set; }

    public static CurrentStatus Status {get ; private set; }
    public static GameBehaviour GameBehaviour;
    public static ViewerBehaviour ViewerBehaviour;

    private static Action _successfullCallback;
    private static Action<string> _errorCallback;


    internal static void ConnectToServer(Action successfullCallback, Action<string> errorCallback)
    {
        ClientHandle.AddHandlers();
        PlayerId = 0;

        _successfullCallback = successfullCallback;
        _errorCallback = errorCallback;

        Client.Instance.ConnectToServer();
    }

    public static void ConnectedToServer()
    {
        _successfullCallback?.Invoke();
    }

    internal static void RequestStatus()
    {
        ClientSend.RequestGameStatusUpdate();
    }

    internal static void Disconnect()
    {
        Client.Instance.Disconnect();
        PlayerId = 0;
    }
    internal static void Disconnected()
    {
        if(PlayerId == 0)
            ViewerBehaviour.Disconnected();
        else
            GameBehaviour.Disconnected();

        PlayerId = 0;
    }

    internal static void ConnectToServerError()
    {
        PlayerId = 0;
        _errorCallback?.Invoke("connection failed");
    }


    internal static void RegisterAsPlayer(Action successfullCallback, Action<string> errorCallback)
    {
        _successfullCallback = successfullCallback;
        _errorCallback = errorCallback;

        ClientSend.RegisterAsPlayer();
    }


    internal static void RegisteredAsPlayer(int myPlayerId)
    {
        PlayerId = myPlayerId;
        _successfullCallback?.Invoke();
    }
    internal static void ErrorToRegisterAsPlayer(string errorCode)
    {
        _errorCallback?.Invoke(errorCode);
    }

#region PlayInSlot
    internal static void PlayInSlot(int slotIndex)
    {
        ClientSend.PlayInSlot(slotIndex);
    }

    public static void CurrentGameStatus(CurrentStatus status, int[] slots)
    {
        Slots = slots;
        Status = status;

        
        switch (status)
        {
            case CurrentStatus.Player1Turn:
                if(PlayerId == 0)
                    ViewerBehaviour?.UpdateTurn();
                else if(PlayerId == 1)
                    GameBehaviour?.YourTurn();
                else
                    GameBehaviour?.OtherPlayerTurn();
                break;
            case CurrentStatus.Player2Turn:
                if(PlayerId == 0)
                    ViewerBehaviour?.UpdateTurn();
                if(PlayerId == 2)
                    GameBehaviour?.YourTurn();
                else
                    GameBehaviour?.OtherPlayerTurn();
                break;
            case CurrentStatus.FinishedWithTie:
                if(PlayerId == 0)
                    ViewerBehaviour?.Tie();
                else
                    GameBehaviour?.Tie();
                break;
            case CurrentStatus.FinishedWithPlayer1Victory:
                if(PlayerId == 0)
                    ViewerBehaviour?.VictoryPlayer1();
                if(PlayerId == 1)
                    GameBehaviour?.Victory();
                else
                    GameBehaviour?.Lose();
                break;            
            case CurrentStatus.FinishedWithPlayer2Victory:
                if(PlayerId == 0)
                    ViewerBehaviour?.VictoryPlayer2();
                if(PlayerId == 2)
                    GameBehaviour?.Victory();
                else
                    GameBehaviour?.Lose();
                break;
            case CurrentStatus.WaitingOtherPlayerToConnect:
                if(PlayerId == 0)
                    ViewerBehaviour?.WaitingOtherPlayer();
                else 
                    GameBehaviour?.WaitingOtherPlayer();
                break;
            default:
                return;
        }

    }
#endregion
}