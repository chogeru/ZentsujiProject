using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetBGM : MonoBehaviour
{
    [SerializeField, Header("開始時にセットするBGM")]
    private string m_BGMName;
    [SerializeField, Header("音量")]
    private float m_Volume;
    private void Start(){ BGMManager.instance.PlayBGMByScene(m_BGMName, m_Volume);}
}
