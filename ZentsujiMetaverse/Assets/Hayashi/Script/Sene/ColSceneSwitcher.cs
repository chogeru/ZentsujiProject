using Mirror;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ColSceneSwitcher : NetworkBehaviour
{
    [SerializeField]
    private string m_SceneName; // 移動したいシーンの名前を設定

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (isServer)
            {
                SwitchScene().Forget();
            }
        }
    }

    private async UniTaskVoid SwitchScene()
    {
        LoadCanvas.instance.SetUI();
        // シーンの切り替え
        NetworkManager.singleton.ServerChangeScene(m_SceneName);

        // 新しいシーンのロード完了を待つ
        await SceneManager.LoadSceneAsync(m_SceneName);
        LoadCanvas.instance.CloseUI();
        // 新しいシーンのネットワークマネージャーを取得して初期化
        NetworkManager newNetworkManager = FindObjectOfType<NetworkManager>();
        if (newNetworkManager != null)
        {
            // すべてのクライアントにプレイヤーをスポーンする
            SpawnPlayersAtStartPosition(newNetworkManager);
        }
        else
        {
            // 新しいネットワークマネージャーが見つからない場合は警告を出す
            Debug.LogWarning("New NetworkManager not found in the scene.");
        }
    }

    private void SpawnPlayersAtStartPosition(NetworkManager networkManager)
    {
        foreach (var conn in NetworkServer.connections.Values)
        {
            Transform startPos = networkManager.GetStartPosition();
            GameObject playerPrefab = networkManager.playerPrefab;
            if (startPos != null && playerPrefab != null)
            {
                GameObject player = Instantiate(playerPrefab, startPos.position, startPos.rotation);
                NetworkServer.AddPlayerForConnection(conn, player);
            }
            else
            {
                Debug.LogError("Start position or player prefab is not set.");
            }
        }
    }
}
