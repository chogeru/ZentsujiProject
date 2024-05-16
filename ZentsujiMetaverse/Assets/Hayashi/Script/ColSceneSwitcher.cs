using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColSceneSwitcher : NetworkBehaviour
{

    [SerializeField]
    private string m_SceneName; // 移動したいシーンの名前を設定

    private void OnTriggerEnter(Collider other)
    {
        // プレイヤーオブジェクトにタグを付けることをお勧めします（例：Player）
        if (other.CompareTag("Player"))
        {
            // サーバー側でのみシーンの切り替えを行います
            if (isServer)
            {
                NetworkManager.singleton.ServerChangeScene(m_SceneName);
            }
        }
    }
}
