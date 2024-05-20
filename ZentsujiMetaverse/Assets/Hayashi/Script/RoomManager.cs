using Mirror;
using UnityEngine;
using UnityEngine.UI;
using VInspector;
using TMPro;

public class RoomManager : NetworkManager
{
    [SerializeField,Header("ルーム名入力フィールド")]
    private TMP_InputField m_RoomNameInput;
    [SerializeField,Header("パスワード入力フィールド")]
    private TMP_InputField m_PasswordInput;
    [SerializeField,Header("最大人数入力フィールド")]
    private TMP_InputField m_MaxPlayersInput;
    
    [SerializeField,Header("ルーム作成ボタン")]
    private Button m_CreateRoomButton;
    [SerializeField,Header("ルーム入出ボタン")]
    private Button m_JoinRoomButton;

    private string m_RoomName;
    private string m_RoomPassword;
    private int m_MaxPlayers;

    public override void Start()
    {
        base.Start();

        m_CreateRoomButton.onClick.AddListener(CreateRoom);
        m_JoinRoomButton.onClick.AddListener(JoinRoom);

        NetworkClient.RegisterHandler<CustomNetworkMessage>(OnClientReceiveMessage);
        NetworkClient.OnConnectedEvent += OnClientConnected;
    }

    public void CreateRoom()
    {
        m_RoomName = m_RoomNameInput.text;
        m_RoomPassword = m_PasswordInput.text;
        int.TryParse(m_MaxPlayersInput.text, out m_MaxPlayers);

        // カスタムのロジックでルーム名とパスワードを設定
        NetworkServer.Listen(7777);
        NetworkManager.singleton.StartHost();
    }

    public void JoinRoom()
    {
        m_RoomName = m_RoomNameInput.text;
        m_RoomPassword = m_PasswordInput.text;

        // カスタムのロジックでルーム名とパスワードをチェック
        NetworkManager.singleton.networkAddress = "localhost";
        NetworkManager.singleton.StartClient();
    }

    private void OnClientConnected()
    {
        Debug.Log("サーバーに接続されました。");

        // クライアントがサーバーに接続したときにルーム名とパスワードを送信する
        CustomNetworkMessage message = new CustomNetworkMessage
        {
            roomName = m_RoomName,
            password = m_RoomPassword
        };
        NetworkClient.Send(message);
    }

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        if (NetworkServer.connections.Count <= m_MaxPlayers)
        {
            // プレイヤーを追加する
            GameObject player = Instantiate(playerPrefab);
            NetworkServer.AddPlayerForConnection(conn, player);
        }
        else
        {
            // 最大人数に達しているため、接続を拒否する
            conn.Disconnect();
            Debug.Log("プレイヤーの最大人数に達しました。接続を拒否しました。");
        }
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        NetworkServer.RegisterHandler<CustomNetworkMessage>(OnReceiveCustomNetworkMessage);
    }

    private void OnReceiveCustomNetworkMessage(NetworkConnectionToClient conn, CustomNetworkMessage message)
    {
        Debug.Log("受信したルーム名: " + message.roomName);
        Debug.Log("受信したパスワード: " + message.password);

        // ここでルーム名とパスワードの検証ロジックを追加する
        if (message.roomName == m_RoomName && message.password == m_RoomPassword)
        {
            // 正しいルーム名とパスワード
            Debug.Log("ルーム名とパスワードが正しいです。");
        }
        else
        {
            // 間違ったルーム名またはパスワード
            conn.Disconnect();
            Debug.Log("ルーム名またはパスワードが間違っています。接続を拒否しました。");
        }
    }

    private void OnClientReceiveMessage(CustomNetworkMessage message)
    {
        Debug.Log("サーバーから受信したメッセージ: ルーム名=" + message.roomName + ", パスワード=" + message.password);
    }
}

public struct CustomNetworkMessage : NetworkMessage
{
    public string roomName;
    public string password;
}
