using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnlinePlayerSpawner : MonoBehaviour
{
    [SerializeField,Header("�v���C���[�X�|�[���v���n�u")]
    private GameObject m_PlayerPrefabs;
    [SerializeField,Header("�X�|�[���|�C���g")]
    private Transform m_PlyerSpownPoint;
    void Start()
    {
        if (NetworkClient.isConnected)
        {
            // �I�����C���p�v���C���[�v���n�u�𐶐�
            Instantiate(m_PlayerPrefabs,m_PlyerSpownPoint);
        }
        else
        {
            Debug.Log("�ڑ��ł��Ă��Ȃ�");
        }
    }
}
