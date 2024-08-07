using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MonobitEngine;

public class CameraControl : MonobitEngine.MonoBehaviour
{
    public GameObject m_CameraGameObject; // アクティブにするカメラオブジェクト

    private void Start()
    {
        // オンラインでかつ自身のプレイヤーである場合、カメラをアクティブにする
        if (monobitView.isMine)
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
