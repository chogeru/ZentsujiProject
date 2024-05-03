using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using SQLite4Unity3d;
using UnityEngine.Rendering.Universal;

public class BGMManager : MonoBehaviour
{
    public static BGMManager instance;
    private AudioSource audioSource;
    [SerializeField]
    public SQLiteConnection connection;
    public string m_BGMName;
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
    private void Start()
    {
        PlayBGMByScene(m_BGMName);
    }

    private void PlayBGMByScene(string bgmName)
    {
        var query = connection.Table<BGM>().Where(x => x.BGMName == bgmName).FirstOrDefault();
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

    class BGM
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string BGMName { get; set; }
        public string BGMFileName { get; set; }
    }
}

