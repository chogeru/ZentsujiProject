using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnlinePlayerSpawner : MonoBehaviour
{
    [SerializeField,Header("プレイヤースポーンプレハブ")]
    private GameObject m_PlayerPrefabs;
    [SerializeField,Header("スポーンポイント")]
    private Transform m_PlyerSpownPoint;
    void Start()
    {
        if (NetworkClient.isConnected)
        {
            // オンライン用プレイヤープレハブを生成
            Instantiate(m_PlayerPrefabs,m_PlyerSpownPoint);
        }
        else
        {
            Debug.Log("接続できていない");
        }
    }
}
