using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using SQLite4Unity3d;

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
        
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PlayBGMByScene(scene.name);
    }

    private void PlayBGMByScene(string sceneName)
    {
        var query = connection.Table<SceneBGM>().Where(x => x.SceneName == sceneName).FirstOrDefault();
        if (query != null)
        {
            Debug.Log(query);
            AudioClip clip = Resources.Load<AudioClip>("BGM/" + query.BGMFileName);
            if (clip != null)
            {
                audioSource.clip = clip;
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

    class SceneBGM
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string SceneName { get; set; }
        public string BGMFileName { get; set; }
    }
}

