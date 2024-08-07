using Cysharp.Threading.Tasks;
using MonobitEngine;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ColSceneSwitcher : MonobitEngine.MonoBehaviour
{
    [SerializeField]
    private string m_SceneName;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (MonobitNetwork.isHost)
            {
                SwitchScene().Forget();
            }
        }
    }

    private async UniTaskVoid SwitchScene()
    {
        LoadCanvas.instance.SetUI();

        // シーンの切り替え
        MonobitNetwork.LoadLevel(m_SceneName);

        // 新しいシーンのロード完了を待つ
        await SceneManager.LoadSceneAsync(m_SceneName);
        LoadCanvas.instance.CloseUI();
    }
}
