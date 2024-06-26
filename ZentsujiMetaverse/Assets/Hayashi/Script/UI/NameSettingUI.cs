using Org.BouncyCastle.Asn1.X509;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NameSettingUI : MonoBehaviour
{
    [SerializeField, Header("名前入力用インプットフィールド")]
    private InputField m_NameSettingField;
    [SerializeField, Header("名前")]
    private string m_Name;

    private string m_DataPath;
    private void Awake()
    {
        m_DataPath = Application.dataPath + "/UserName.json";
    }
    public void SetName()
    {
        m_NameSettingField.text = m_Name;
        SaveJson();
    }
    public void SaveJson()
    {

    }
}

