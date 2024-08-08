using Mirror;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RoomManager : NetworkManager
{
    [SerializeField, Header("ルーム名入力フィールド")]
    private TMP_InputField m_RoomNameInput;
    [SerializeField, Header("パスワード入力フィールド")]
    private TMP_InputField m_PasswordInput;
    [SerializeField, Header("最大人数入力フィールド")]
    private TMP_InputField m_MaxPlayersInput;

    [SerializeField, Header("ルーム作成ボタン")]
    private Button m_CreateRoomButton;
    [SerializeField, Header("ルーム入出ボタン")]
    private Button m_JoinRoomButton;

    [SerializeField, Header("オンラインシーン名")]
    private string m_OnlineSceneName;

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
        NetworkManager.singleton.ServerChangeScene(m_OnlineSceneName);
    }

    public void JoinRoom()
    {
        m_RoomName = m_RoomNameInput.text;
        m_RoomPassword = m_PasswordInput.text;

        // カスタムのロジックでルーム名とパスワードをチェック
        NetworkManager.singleton.networkAddress = "localhost";
        NetworkManager.singleton.StartClient();
        NetworkManager.singleton.ServerChangeScene(m_OnlineSceneName);
    }

    private void OnClientConnected()
    {
        Debug.Log("サーバーに接続");

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
            Debug.Log("プレイヤーの最大人数に達した。接続を拒否した。");
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

    public void SwitchToAnotherRoom(string newRoomName, string newPassword, string newServerAddress)
    {
        // 既存の接続を終了
        NetworkManager.singleton.StopClient();

        // 新しいサーバーの情報を設定
        m_RoomName = newRoomName;
        m_RoomPassword = newPassword;
        NetworkManager.singleton.networkAddress = newServerAddress;

        // 新しいサーバーに接続
        NetworkManager.singleton.StartClient();
    }
    [ContextMenu("Debug Room Info")]
    private void DebugRoomInfo()
    {
        Debug.Log($"現在オンラインに接続しているか: {NetworkClient.isConnected}");
        Debug.Log($"ルーム名: {m_RoomName}");
        Debug.Log($"パスワード: {m_RoomPassword}");
        Debug.Log($"ルームの最大人数: {m_MaxPlayers}");
        Debug.Log($"現在の接続数: {NetworkServer.connections.Count}");
    }
}

public struct CustomNetworkMessage : NetworkMessage
{
    public string roomName;
    public string password;
}
