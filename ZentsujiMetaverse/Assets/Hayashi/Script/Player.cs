using Mirror;
using UnityEngine;

public class Player : NetworkBehaviour
{
    [SyncVar]
    public string studentId; 

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        // プレイヤーがホストであるかどうかを判定
        if (NetworkServer.active && NetworkClient.isConnected)
        {
            Debug.Log("I am the Host.");
        }
        else if (NetworkClient.isConnected)
        {
            Debug.Log("I am a Client.");
        }
    }

}
