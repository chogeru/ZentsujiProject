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
            // Mirror���g�p���ăl�b�g���[�N�ڑ����J�n
            NetworkManager networkManager = NetworkManager.singleton;
            if (networkManager == null)
            {
                Debug.LogError("NetworkManager��������܂���ł����B");
                return;
            }

            if (!networkManager.isNetworkActive)
            {
                // �T�[�o�[���Ȃ��ꍇ�A���g���z�X�g�Ƃ��Đڑ�
                networkManager.StartHost();
            }
            else
            {
                // �T�[�o�[������ꍇ�A�N���C�A���g�Ƃ��Đڑ�
                networkManager.StartClient();
            }

            // �V�[����񓯊��Ń��[�h
         //   await UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(nextScene, LoadSceneMode.Single);
         
        }
        else
        {
            Debug.LogError("����scene��null���w�肳��Ă��Ȃ�");
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
            Debug.LogError("�f�[�^�x�[�X�A�N�Z�X���ɃG���[: " + ex.Message);
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
