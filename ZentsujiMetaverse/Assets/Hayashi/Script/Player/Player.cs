using Mirror;
using UnityEngine;
using System.IO;
using AbubuResouse.Log;

public class Player : NetworkBehaviour
{
    [SyncVar,SerializeField,Header("プレイヤー名")]
    public string m_PlayerName;
    private string m_DataPath;

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        // プレイヤーがホストであるかどうかを判定
        if (NetworkServer.active && NetworkClient.isConnected)
        {
            DebugUtility.Log("私ホスト");
        }
        else if (NetworkClient.isConnected)
        {
            DebugUtility.Log("私クライアント");
        }
        m_DataPath = Application.persistentDataPath + "/UserName.json";

        m_PlayerName = LoadJsonName();
        if (string.IsNullOrEmpty(m_PlayerName))
        {
            DebugUtility.LogWarning("JSONから名前を読み込めませんでした。デフォルト名を使用します。");
            m_PlayerName = "Player" + Random.Range(1000, 9999);
        }
        CmdSetPlayerName(m_PlayerName);
    }
    [Command]
    void CmdSetPlayerName(string name)
    {
        m_PlayerName = name;
    }
    private string LoadJsonName()
    {
        if (File.Exists(m_DataPath))
        {
            string json = File.ReadAllText(m_DataPath);
            UserNameData data = JsonUtility.FromJson<UserNameData>(json);
            return data.Name;
        }
        return string.Empty;
    }

    [System.Serializable]
    private class UserNameData
    {
        public string Name;
    }
}
