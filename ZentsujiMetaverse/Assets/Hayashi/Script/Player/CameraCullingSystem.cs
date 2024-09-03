using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCullingSystem : MonoBehaviour
{
    public LayerMask playerLayerMask; // プレイヤーレイヤーのマスク

    private Camera m_MainCamera;
    private int m_DefaultCullingMask;

    private void Start()
    {
        m_MainCamera = GetComponent<Camera>();
        m_DefaultCullingMask = m_MainCamera.cullingMask;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            m_MainCamera.cullingMask &= ~playerLayerMask;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            m_MainCamera.cullingMask = m_DefaultCullingMask;
        }
    }
}
