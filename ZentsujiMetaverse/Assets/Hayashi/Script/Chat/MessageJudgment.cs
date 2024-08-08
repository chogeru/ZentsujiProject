using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageJudgment : NetworkBehaviour
{
    [SerializeField, Header("表示するオブジェクト")]
    private GameObject m_Indicator;
    [SerializeField, Header("検出範囲")]
    private float m_DetectionRadius = 5f;

    private void Update()
    {
        //自分のオブジェクトの場合のみ検出を行う
        if(isLocalPlayer)
        {
            CheckForNeartyPlayers();
        }
    }

    private void CheckForNeartyPlayers()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, m_DetectionRadius);
        bool playerNearby = false;

        foreach(var hitCollider in hitColliders)
        {
            NetworkIdentity networkIdentity = hitCollider.GetComponent<NetworkIdentity>();
            if (networkIdentity != null && networkIdentity != GetComponent<NetworkIdentity>())
            {
                playerNearby = true;
                break;
            }
        }
        m_Indicator.SetActive(playerNearby);
    }
}
