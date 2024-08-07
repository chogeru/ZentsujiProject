using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using SQLite4Unity3d;
using Cysharp.Threading.Tasks;
using MonobitEngine;
using System;

public class MySceneManager : UnityEngine.MonoBehaviour
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
        LoadNextSceneAsync(currentSceneName).Forget();
    }

    private async UniTaskVoid LoadNextSceneAsync(string currentSceneName)
    {
        if (isSceneLoading)
        {
            Debug.LogWarning("シーンがすでにロード中です");
            return;
        }
        isSceneLoading = true;

        if (connection == null)
        {
            Debug.LogError("データベース接続が確立されていません");
            isSceneLoading = false;
            return;
        }

        nextScene = await GetNextSceneNameFromDBAsync(currentSceneName);
        if (string.IsNullOrEmpty(nextScene))
        {
            Debug.LogError("次のシーン名が見つかりません");
            isSceneLoading = false;
            return;
        }

        if (MonobitNetwork.isConnect)
        {
            if (MonobitNetwork.inRoom)
            {
                MonobitNetwork.LeaveRoom();
                while (MonobitNetwork.inRoom)
                {
                    await UniTask.Yield();
                }
            }
            JoinOrCreateRoom(nextScene);
        }
        else
        {
            MonobitNetwork.autoJoinLobby = true;
            MonobitNetwork.ConnectServer("Zentuuji");
        }
        isSceneLoading = false;
    }

    private void OnDestroy()
    {
        if (MonobitNetwork.isConnect)
        {
            MonobitEngine.MonobitNetwork.LeaveLobby();
        }
    }

    private void OnJoinedLobby()
    {
        if (!MonobitNetwork.inRoom)
        {
            JoinOrCreateRoom(nextScene);
        }
    }

    private void JoinOrCreateRoom(string sceneName)
    {
        RoomSettings roomSettings = new RoomSettings();
        roomSettings.maxPlayers = 10;
        LobbyInfo lobbyInfo = new LobbyInfo();
        MonobitNetwork.JoinOrCreateRoom(sceneName, roomSettings, lobbyInfo);
        Debug.Log("ルームの作成または参加を試みています: " + sceneName); // 追加
    }

    private void OnJoinedRoom()
    {
        Debug.Log("ルームに入室しました: " + MonobitNetwork.room.name);
        if (!string.IsNullOrEmpty(nextScene))
        {
            SceneManager.LoadScene(nextScene);
        }
        else
        {
            Debug.LogError("次のシーン名が設定されていません");
        }
    }

    private async UniTask<string> GetNextSceneNameFromDBAsync(string currentSceneName)
    {
        try
        {
            return await UniTask.RunOnThreadPool(() =>
            {
                var query = connection.Table<SceneTransition>()
                                       .Where(x => x.CurrentScene == currentSceneName)
                                       .FirstOrDefault();
                return query?.NextScene;
            });
        }
        catch (System.Exception ex)
        {
            DebugUtility.LogError(ex.Message);
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
