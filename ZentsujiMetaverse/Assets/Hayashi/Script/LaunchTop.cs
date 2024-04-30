using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaunchTop : MonoBehaviour
{
    public GameObject topPrefab; // 発射するコマのプレファブ
    public Camera mainCamera; // シーン内のメインカメラ
    public float launchForce = 1000f; // 発射する力の大きさ

    void Update()
    {
        if (Input.GetMouseButtonDown(1)) // マウスの右クリックを検出
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                GameObject topInstance = Instantiate(topPrefab, mainCamera.transform.position, Quaternion.identity);
                Rigidbody rb = topInstance.GetComponent<Rigidbody>();
                Vector3 direction = (hit.point - mainCamera.transform.position).normalized; // ヒットした地点に向けた方向ベクトル
                rb.AddForce(direction * launchForce); // ベクトルの方向に力を加える
            }
        }
    }
}
