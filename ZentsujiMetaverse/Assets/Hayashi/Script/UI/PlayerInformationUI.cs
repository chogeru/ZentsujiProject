using MonobitEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerInformationUI : MonobitEngine.MonoBehaviour
{
    public TMP_Text m_NameText;
    public Player m_Player;

    private void Start()
    {
        if (monobitView.isMine)
        {
            m_Player = GetComponent<Player>();
            m_NameText.text = m_Player.m_Name;
            monobitView.RPC("RpcUpdateNameText", MonobitTargets.AllBuffered, m_Player.m_Name);
        }
    }

    [MunRPC]
    void RpcUpdateNameText(string playerName)
    {
        m_NameText.text = playerName;
    }
}
