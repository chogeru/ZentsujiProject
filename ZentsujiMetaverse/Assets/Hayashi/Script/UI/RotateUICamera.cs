using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateUICamera : MonoBehaviour
{
    [SerializeField]
    private Camera mainCamera;


    void Update()
    {
        // カメラのY軸回転を取得
        float cameraRotationY = mainCamera.transform.eulerAngles.y;

        // UI要素のY軸回転をカメラのY軸回転に合わせる
        transform.eulerAngles = new Vector3(0f, cameraRotationY, 0f);
    }
}
