using Mirror;
using UnityEngine;

public class Player : NetworkBehaviour
{
    [SyncVar,SerializeField,Header("プレイヤー名")]
    public string m_Name; 

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        // プレイヤーがホストであるかどうかを判定
        if (NetworkServer.active && NetworkClient.isConnected)
        {
            Debug.Log("私ホスト");
        }
        else if (NetworkClient.isConnected)
        {
            Debug.Log("私クライアント");
        }
    }

}
