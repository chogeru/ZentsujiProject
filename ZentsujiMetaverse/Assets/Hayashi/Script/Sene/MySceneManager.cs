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
        nextScene = GetNextSceneNameFromDB(currentSceneName);
        if (!string.IsNullOrEmpty(nextScene))
        {
            NetworkManager networkManager = NetworkManager.singleton;
            if (networkManager == null)
            {
                Debug.LogError("");
                return;
            }
            networkManager.onlineScene = nextScene;
            if (!networkManager.isNetworkActive)
            {
                networkManager.StartHost();
            }
            else
            {
                networkManager.StartClient();
            }         
        }
        else
        {
            Debug.LogError("");
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
