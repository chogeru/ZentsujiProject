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
            Debug.Log("データベース接続が確立されました。");
        }
        catch (Exception ex)
        {
            Debug.LogError("データベースのオープンに失敗しました: " + ex.Message);
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

            if (!networkManager.isNetworkActive)
            {
                // ネットワークがアクティブでない場合、ホストとして開始
                Debug.Log("ホストとして開始します...");
                networkManager.StartHost();
            }
            else if (NetworkServer.active)
            {
                // すでにサーバーとして動作している場合、シーンを変更
                Debug.Log("サーバーがアクティブです。シーンを変更します...");
                NetworkServer.SendToAll(new SceneMessage { sceneName = nextScene, sceneOperation = SceneOperation.LoadAdditive });
            }
            else if (NetworkClient.isConnected)
            {
                // クライアントとして接続済みの場合、シーンの変更を要求
                Debug.Log("クライアントは接続済みです。シーンの変更を要求します...");
                NetworkClient.Send(new SceneMessage { sceneName = nextScene, sceneOperation = SceneOperation.LoadAdditive });
            }
            else
            {
                // それ以外の場合（ネットワークはアクティブだがサーバーでもクライアントでもない）、クライアントとして接続
                Debug.Log("クライアントとして開始します...");
                networkManager.StartClient();
            }

        }
        else
        {
            Debug.LogError("次のシーン名が見つかりません");
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
            Debug.LogError("データベースクエリエラー: " + ex.Message);
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
