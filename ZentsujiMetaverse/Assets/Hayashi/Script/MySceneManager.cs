using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using SQLite4Unity3d;
using Cysharp.Threading.Tasks;
using Mirror;

public class MySceneManager : MonoBehaviour
{
    public static MySceneManager Instance { get; private set; }
    private SQLiteConnection connection;
    private string nextScene;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        var databasePath = System.IO.Path.Combine(Application.streamingAssetsPath, "scene_data.db").Replace("\\", "/");
        connection = new SQLiteConnection(databasePath, SQLiteOpenFlags.ReadOnly);
    }

    public void TriggerSceneLoad(string currentSceneName)
    {
        LoadNextSceneAsync(currentSceneName).Forget();
    }

    private async UniTaskVoid LoadNextSceneAsync(string currentSceneName)
    {
        nextScene = GetNextSceneNameFromDB(currentSceneName);
        if (!string.IsNullOrEmpty(nextScene))
        {
            // Mirrorを使用してネットワーク接続を開始
            NetworkManager networkManager = NetworkManager.singleton;
            if (networkManager == null)
            {
                Debug.LogError("NetworkManagerが見つかりませんでした。");
                return;
            }

            if (!networkManager.isNetworkActive)
            {
                // サーバーがない場合、自身がホストとして接続
                networkManager.StartHost();
            }
            else
            {
                // サーバーがある場合、クライアントとして接続
                networkManager.StartClient();
            }

            // シーンを非同期でロード
         //   await UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(nextScene, LoadSceneMode.Single);
        }
        else
        {
            Debug.LogError("次のsceneがnullか指定されていない");
        }
    }

    private string GetNextSceneNameFromDB(string currentSceneName)
    {
        try
        {
            var query = connection.Table<SceneTransition>().Where(x => x.CurrentScene == currentSceneName).FirstOrDefault();
            if (query != null)
            {
                return query.NextScene;
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError("データベースアクセス時にエラー: " + ex.Message);
        }
        return null;
    }

    class SceneTransition
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string CurrentScene { get; set; }
        public string NextScene { get; set; }
    }
}
