using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpownPlayer : NetworkBehaviour
{
    [SerializeField, Header("スポーン地点")]
    private Transform m_SpownPos;
    [SerializeField, Header("プレイヤープレハブ")]
    private GameObject m_PlayerPrefabs;
    public override void OnStartLocalPlayer()
    {
        Instantiate(m_PlayerPrefabs, m_SpownPos);
    }
}
