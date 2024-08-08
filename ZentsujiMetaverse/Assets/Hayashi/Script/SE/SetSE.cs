using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AbubuResouse.Singleton;

public class SetSE : MonoBehaviour
{
    [SerializeField, Header("再生するSEの名前")]
    private string m_SEClipName;
    [SerializeField, Header("音量")]
    private float m_Volume;
    public void PlaySE()
    {
        SEManager.Instance.PlaySound(m_SEClipName, m_Volume);
    }

    public void PlaySEButton(string SEClipName)
    {
        SEManager.Instance.PlaySound(SEClipName, m_Volume);
    }
}
