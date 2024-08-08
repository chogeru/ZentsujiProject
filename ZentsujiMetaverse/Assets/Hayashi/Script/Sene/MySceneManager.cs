using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using SQLite4Unity3d;
using Cysharp.Threading.Tasks;
using Mirror;
using System;

public class MySceneManager : MonoBehaviour
{
    public static MySceneManager Instance { get; private set; }
    private SQLiteConnection connection;
    private string nextScene;
    private bool isSceneLoading = false;

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
        try
        {
            connection = new SQLiteConnection(databasePath, SQLiteOpenFlags.ReadOnly);
            Debug.Log("Database connection established.");
        }
        catch (Exception ex)
        {
            Debug.LogError("Failed to open database: " + ex.Message);
        }
    }

    public void TriggerSceneLoad(string currentSceneName)
    {
        LoadNextSceneAsync(currentSceneName);
    }

    private void LoadNextSceneAsync(string currentSceneName)
    {
        if (isSceneLoading)
        {
            DebugUtility.LogWarning("シーンがすでにロード中");
            return;
        }
        isSceneLoading = true;
        nextScene = GetNextSceneNameFromDB(currentSceneName);
        if (!string.IsNullOrEmpty(nextScene))
        {
            NetworkManager networkManager = NetworkManager.singleton;
            if (networkManager == null)
            {
                DebugUtility.LogError("NetworkManagerのシングルトン無い！");
                isSceneLoading = false;
                return;
            }
            networkManager.onlineScene = nextScene;

            if (!networkManager.isNetworkActive)
            {
                // ネットワークがアクティブでない場合、ホストとして開始
                networkManager.StartHost();
            }
            else if (NetworkServer.active)
            {
                // すでにサーバーとして動作している場合、シーンを変更
                NetworkManager.singleton.ServerChangeScene(nextScene);
            }
            else if (NetworkClient.isConnected)
            {
                // クライアントとして接続済みの場合、何もしない（サーバーの変更を待つ）
                DebugUtility.Log("クライアントはサーバーのシーン変更を待機中");
            }
            else
            {
                // それ以外の場合（ネットワークはアクティブだがサーバーでもクライアントでもない）、クライアントとして接続
                networkManager.StartClient();
            }
        }
        else
        {
            DebugUtility.LogError("次のシーン名が見つからん");
        }
        isSceneLoading = false;
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
            Debug.LogError("" + ex.Message);
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
