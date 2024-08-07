using MonobitEngine;
using UnityEngine;
using System.IO;

public class Player : MonobitEngine.MonoBehaviour
{
    [SerializeField, Header("プレイヤー名")]
    public string m_Name;
    private string m_DataPath;

    private void Start()
    {
        // ローカルプレイヤーかどうかを判定
        if (monobitView.isMine)
        {
            DebugUtility.Log("私クライアント");
            m_DataPath = Application.persistentDataPath + "/UserName.json";

            m_Name = LoadJsonName();
            if (string.IsNullOrEmpty(m_Name))
            {
                DebugUtility.LogWarning("JSONから名前を読み込めませんでした。デフォルト名を使用します。");
                m_Name = "Player" + Random.Range(1000, 9999);
            }
            monobitView.RPC("RpcSetPlayerName", MonobitTargets.AllBuffered, m_Name);
            ChatUI.m_LocalPlayerName = m_Name;
        }
    }

    [MunRPC]
    void RpcSetPlayerName(string name)
    {
        m_Name = name;
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
