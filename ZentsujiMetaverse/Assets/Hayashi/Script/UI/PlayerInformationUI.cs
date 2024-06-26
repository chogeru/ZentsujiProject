using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerInformationUI : NetworkBehaviour
{
    public TMP_Text m_NameText;

    public Player m_Player;

    private void Start()
    {
        m_Player = GetComponent<Player>();
        m_NameText.text = m_Player.m_Name;
    }
}
