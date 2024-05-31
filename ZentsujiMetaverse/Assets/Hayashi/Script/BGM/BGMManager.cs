using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using SQLite4Unity3d;
using UnityEngine.SceneManagement;

public class BGMManager : MonoBehaviour
{
    public static BGMManager instance;
    private AudioSource audioSource;
    [SerializeField]
    public SQLiteConnection connection;
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        audioSource = GetComponent<AudioSource>();
        var databasePath = System.IO.Path.Combine(Application.streamingAssetsPath, "bgm_data.db").Replace("\\", "/");
        connection = new SQLiteConnection(databasePath, SQLiteOpenFlags.ReadOnly);
        Debug.Log("Database path: " + databasePath);
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

    public void PlayBGMByScene(string bgmName ,float volume)
    {
        var query = connection.Table<BGM>().Where(x => x.BGMName == bgmName).FirstOrDefault();
        if (query != null)
        {
            Debug.Log(query);
            AudioClip clip = Resources.Load<AudioClip>("BGM/" + query.BGMFileName);
            if (clip != null)
            {
                audioSource.clip = clip;
                audioSource.volume = volume;
                audioSource.Play();
            }
            else
            {
                Debug.Log(query.BGMFileName);
            }
        }
        else
        {
            Debug.Log("queryがnull");
        }
    }
    // BGM情報を保持するクラス
    class BGM
    {
        [PrimaryKey, AutoIncrement] // 主キー、自動インクリメント
        public int Id { get; set; }
        public string BGMName { get; set; }  // BGMの名前
        public string BGMFileName { get; set; } // BGMファイル名
    }
}
