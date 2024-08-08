using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class CameraControl : NetworkBehaviour
{
    public GameObject m_CameraGameObject; // アクティブにするカメラオブジェクト

    private void Start()
    {
        // オンラインでかつ自身のプレイヤーである場合、カメラをアクティブにする
        if (isLocalPlayer)
        {
            // カメラをアクティブにする
            m_CameraGameObject.SetActive(true);
        }
        else
        {
            // カメラを非アクティブにする
            m_CameraGameObject.SetActive(false);
        }
    }
}
