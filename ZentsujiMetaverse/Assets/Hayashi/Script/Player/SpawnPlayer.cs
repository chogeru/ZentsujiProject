using MonobitEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPlayer : MonobitEngine.MonoBehaviour
{
    [SerializeField, Header("スポーン地点")]
    private Transform m_SpawnPos;
    [SerializeField, Header("プレイヤープレハブ")]
    private GameObject m_PlayerName;

    private void Start()
    {
        MonobitNetwork.Instantiate(m_PlayerName.name, m_SpawnPos.position, m_SpawnPos.rotation, 0);
    }
}
