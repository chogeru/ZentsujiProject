using Org.BouncyCastle.Asn1.X509;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class NameSettingUI : MonoBehaviour
{
    [SerializeField, Header("名前入力用インプットフィールド")]
    private InputField m_NameSettingField;
    [SerializeField, Header("名前保存ボタン")]
    private Button m_SaveButton;
    [SerializeField, Header("名前")]
    private string m_UserName;

    private string m_DataPath;
    private void Awake()
    {
        m_DataPath = Application.persistentDataPath + "/UserName.json";
        m_SaveButton.onClick.AddListener(SaveName);
    }
    public void SaveName()
    {
        m_UserName = m_NameSettingField.text;

        Debug.Log("名前: " + m_UserName);
        if (!string.IsNullOrEmpty(m_UserName))
        {
            SaveJson();
            PlayerData.Instance.PlayerName = m_UserName; 
        }
        else
        {
            Debug.LogWarning("名前が空です。保存しない");
        }
    }
    public void SaveJson()
    {
        UserNameData data = new UserNameData { Name=m_UserName };
        string json=JsonUtility.ToJson(data);
        Debug.Log("保存: " + json);
        File.WriteAllText(m_DataPath, json);
    }
    public string LoadJson()
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
    public class UserNameData
    {
        public string Name;
    }
}

public class PlayerData
{
    private static PlayerData instance;
    public static PlayerData Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new PlayerData();
            }
            return instance;
        }
    }

    public string PlayerName { get; set; }
}