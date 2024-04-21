using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VInspector;
using R3;
using Cysharp.Threading.Tasks;

/// <summary>
/// ネットワーク対応のチャットUIを管理するクラス。
/// </summary>
public class ChatUI : NetworkBehaviour
{
    [Tab("UI要素")]
    [SerializeField,Header("チャット履歴を表示するUIテキスト")] 
    private Text m_ChatHistory;
    [SerializeField,Header("チャット履歴のスクロールバー")]
    private Scrollbar m_Scrollbar;
    [SerializeField,Header("ユーザーがメッセージを入力するフィールド")]
    private InputField m_ChatMessage;
    [SerializeField,Header("メッセージ送信ボタン")]
    private Button m_SendButton;
    [EndTab]

    //ローカルプレイヤーの名前
    internal static string m_LocalPlayerName;
    //チャットメッセージの履歴を保持するリスト
    private List<string> m_Messages = new List<string>();
    //保持する最大メッセージ数
    private const int m_MaxMessages = 150;
    internal static readonly Dictionary<NetworkConnectionToClient, string> m_ConnNames = new Dictionary<NetworkConnectionToClient, string>();

    private void Awake()
    {
        // 送信ボタンのクリックイベントを購読
        m_SendButton.OnClickAsObservable()
            .Subscribe(_ => SendMessage())
            .AddTo(this); //ボタンが破棄された時に購読も自動的に解除される

        // 入力フィールドの内容が変わった際の活性/非活性の切り替え
        m_ChatMessage.OnValueChangedAsObservable()
            .Select(input => !string.IsNullOrWhiteSpace(input))
            .Subscribe(interactable => m_SendButton.interactable = interactable)
            .AddTo(this);

        // エンターキーが押された際にメッセージ送信
        m_ChatMessage.OnEndEditAsObservable()
            .Where(_ => Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetButtonDown("Submit"))
            .Subscribe(_ => SendMessage())
            .AddTo(this);
    }

    /// <summary>
    /// サーバーが開始されたときに呼ばれる
    /// </summary>
    public override void OnStartServer()
    {
        m_ConnNames.Clear();
    }

    /// <summary>
    /// クライアントが開始されたときに呼ばれる
    /// </summary>
    public override void OnStartClient()
    {
        // チャット履歴をクリア
        m_ChatHistory.text = "";
    }

    /// <summary>
    /// サーバーへメッセージを送信するコマンド
    /// </summary>
    /// <param name="message">送信するメッセージ</param>
    /// <param name="sender">メッセージの送信者の接続情報</param>
    [Command(requiresAuthority = false)]
    void CmdSend(string message, NetworkConnectionToClient sender = null)
    {
        if (!m_ConnNames.ContainsKey(sender))
            m_ConnNames.Add(sender, sender.identity.GetComponent<Player>().studentId);

        if (!string.IsNullOrWhiteSpace(message))
            RpcReceive(m_ConnNames[sender], message.Trim());
    }

    /// <summary>
    /// 全クライアントで受信したメッセージを処理するRPC
    /// </summary>
    /// <param name="playerName">メッセージの送信者名</param>
    /// <param name="message">受信したメッセージ</param>
    [ClientRpc]
    void RpcReceive(string playerName, string message)
    {
        string prettyMessage = playerName == m_LocalPlayerName ?
            $"<color=red>{playerName}:</color> {message}" :
            $"<color=blue>{playerName}:</color> {message}";
        AppendMessage(prettyMessage);
    }

    /// <summary>
    /// メッセージをUIに追加する
    /// </summary>
    /// <param name="message">追加するメッセージ</param>
    void AppendMessage(string message)
    {
        AppendAndScroll(message).Forget();
    }

    /// <summary>
    /// メッセージを追加し、自動でスクロールする処理を行うコルーチン
    /// </summary>
    /// <param name="message">追加するメッセージ</param>
    async UniTaskVoid AppendAndScroll(string message)
    {
        //リストにメッセージを追加
        m_Messages.Add(message);
        //メッセージが最大数を超えたら
        if(m_Messages.Count>m_MaxMessages)
        {
            //一番古いものから削除
            m_Messages.RemoveAt(0);
        }
        //UIを更新
        UpdateChatHistory(); 

        //一旦待機し、UIの更新を行う
        await UniTask.Yield();
        //スクロールバーを一番下に設定
        m_Scrollbar.value = 0;
    }

    ///  <summary>
    /// チャット履歴テキストを最新のメッセージリストに基づいて更新
    /// </summary>
    private void UpdateChatHistory()
    {
        //メッセージリストを結合してテキストに設定
        m_ChatHistory.text = string.Join("\n", m_Messages); 
    }

    /// <summary>
    /// 退出ボタンがクリックされた際の処理
    /// </summary>
    public void ExitButtonOnClick()
    {
        //ネットワークホストを停止
        NetworkManager.singleton.StopHost();
    }


    /// <summary>
    /// メッセージを送信する
    /// </summary>
    public void SendMessage()
    {
        //メッセージフィールドが空でない場合
        if (!string.IsNullOrWhiteSpace(m_ChatMessage.text))
        {
            //コマンドを使ってサーバーにメッセージを送信
            CmdSend(m_ChatMessage.text.Trim());
            //入力フィールドをクリア
            m_ChatMessage.text = string.Empty;
            //入力フィールドを再度アクティブにする
            m_ChatMessage.ActivateInputField();
        }
    }
}

