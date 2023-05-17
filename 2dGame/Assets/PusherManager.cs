using System;
using System.Threading.Tasks;
using PusherClient;
using UnityEngine;
using TMPro;

public enum State
{
    RUNLEFT,
    RUNRIGHT,
    IDLE,
    ATTACK
}

public class Character
{
    public int playerNumber;
}

public class ChatMessage
{
    public int playerNumber;
    public string chatMessage;
}

public class PusherManager : MonoBehaviour
{
    // A mutation of https://unity3d.com/learn/tutorials/projects/2d-roguelike-tutorial/writing-game-manager
    public static PusherManager instance = null;
    private Pusher _pusher;
    private Channel _channel;
    private const string APP_KEY = "APP_KEY";
    private const string APP_CLUSTER = "APP_CLUSTER";
    public State[] _moveStateArray = new State[4];
    public TMP_Text _player1UI;
    public TMP_Text _player2UI;
    public TMP_Text _player3UI;
    public TMP_Text _player4UI;
    private String[] _playerStatus = { "P1 OFFLINE", "P2 OFFLINE", "P3 OFFLINE", "P4 OFFLINE" };
    public TMP_Text _chatWindow;
    private String _chatHistory = "Chat <br>";

    async Task Start()
    {
        _moveStateArray[0] = State.IDLE;
        _moveStateArray[1] = State.IDLE;
        _moveStateArray[2] = State.IDLE;
        _moveStateArray[3] = State.IDLE;


        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);

        await InitialisePusher();
    }

    private async Task InitialisePusher()
    {
        //Environment.SetEnvironmentVariable("PREFER_DNS_IN_ADVANCE", "true");

        if (_pusher == null && (APP_KEY != "APP_KEY") && (APP_CLUSTER != "APP_CLUSTER"))
        {
            _pusher = new Pusher(APP_KEY, new PusherOptions()
            {
                Cluster = APP_CLUSTER,
                Encrypted = true,
                UserAuthenticator = new HttpUserAuthenticator("http://127.0.0.1:3000/users/game-auth")
            });

            _pusher.Error += OnPusherOnError;
            _pusher.ConnectionStateChanged += PusherOnConnectionStateChanged;
            _pusher.Connected += PusherOnConnected;
            _channel = await _pusher.SubscribeAsync("my-channel");
            _pusher.Subscribed += OnChannelOnSubscribed;
            _pusher.User.Signin();
            _pusher.User.Watchlist.Bind("online", OnWatchlistOnlineEvent);
            _pusher.User.Watchlist.Bind("offline", OnWatchlistOfflineEvent);
            await _pusher.ConnectAsync();
            await _pusher.User.SigninDoneAsync();
        }
        else
        {
            Debug.LogError("APP_KEY and APP_CLUSTER must be correctly set. Find how to set it at https://dashboard.pusher.com");
        }
    }

    private void PusherOnConnected(object sender)
    {
        Character character;

        _channel.Bind("run-left", (PusherEvent eventData) =>
        {
            character = JsonUtility.FromJson<Character>(eventData.Data);
            _moveStateArray[character.playerNumber - 1] = State.RUNLEFT;
        });

        _channel.Bind("run-right", (PusherEvent eventData) =>
        {
            character = JsonUtility.FromJson<Character>(eventData.Data);
            _moveStateArray[character.playerNumber - 1] = State.RUNRIGHT;
        });

        _channel.Bind("idle", (PusherEvent eventData) =>
        {
            character = JsonUtility.FromJson<Character>(eventData.Data);
            _moveStateArray[character.playerNumber - 1] = State.IDLE;
        });

        _channel.Bind("attack", (PusherEvent eventData) =>
        {
            character = JsonUtility.FromJson<Character>(eventData.Data);
            _moveStateArray[character.playerNumber - 1] = State.ATTACK;
        });
        Debug.Log("Connected");

        _channel.Bind("chat", (PusherEvent eventData) =>
        {
            ChatMessage incomingMessage = JsonUtility.FromJson<ChatMessage>(eventData.Data);
            _chatHistory = _chatHistory + incomingMessage.playerNumber + ":" + incomingMessage.chatMessage + "<br>";
        });
    }

    private void PusherOnConnectionStateChanged(object sender, ConnectionState state)
    {
        Debug.Log("Connection state changed");
    }

    private void OnPusherOnError(object s, PusherException e)
    {
        Debug.Log($"Pusher Error: {e.Message} {e}");
    }

    private void OnChannelOnSubscribed(object s, Channel channel)
    {
        Debug.Log("Subscribed");
    }

    public void Message(string message)
    {
        _channel?.Trigger("time has occured", message);
    }

    public State CurrentState(int playerNumber)
    {
        Debug.Log(playerNumber);
        return _moveStateArray[playerNumber - 1];
    }

    async Task OnApplicationQuit()
    {
        if (_pusher != null)
        {
            await _pusher.DisconnectAsync();
        }
    }

    void OnWatchlistOnlineEvent(WatchlistEvent watchlistEvent)
    {
        Debug.Log($"{Environment.NewLine} OnWatchlistOnlineEvent {watchlistEvent}");
        foreach (string userId in watchlistEvent.UserIDs)
        {
            int x = Int32.Parse(userId);
            _playerStatus[x - 1] = ($"P{x} ONLINE");
        }
    }
    void OnWatchlistOfflineEvent(WatchlistEvent watchlistEvent)
    {
        Debug.Log($"{Environment.NewLine} OnWatchlistOfflineEvent {watchlistEvent}");
        foreach (string userId in watchlistEvent.UserIDs)
        {
            int x = Int32.Parse(userId);
            _playerStatus[x - 1] = ($"P{x} OFFLINE");
        }
    }
    static void OnWatchlistEvent(string eventName, WatchlistEvent watchlistEvent)
    {
        Debug.Log($"{Environment.NewLine} OnWatchlistEvent {eventName} {watchlistEvent.Name}");
        foreach (var id in watchlistEvent.UserIDs)
        {
            Debug.Log($"{Environment.NewLine} OnWatchlistEvent {eventName} {watchlistEvent.Name} {id}");
        }
    }
    void Update()
    {
        _player1UI.text = _playerStatus[0];
        _player2UI.text = _playerStatus[1];
        _player3UI.text = _playerStatus[2];
        _player4UI.text = _playerStatus[3];
        _chatWindow.text = _chatHistory;
    }
}
