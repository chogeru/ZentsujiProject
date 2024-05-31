using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerInformationUI : NetworkBehaviour
{
    public TMP_Text studentIdText;

    public Player player;

    private void Start()
    {
        studentIdText.text = player.studentId;
    }
}
