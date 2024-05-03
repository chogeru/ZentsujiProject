using Firebase;
using Firebase.Storage;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using SQLite4Unity3d;

public class BGMManager : MonoBehaviour
{
    public static BGMManager instance;
    private AudioSource audioSource;
    public SQLiteConnection connection;
    public string m_BGMName;

    private FirebaseStorage storage;
    private StorageReference storageReference;

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

        // Initialize Firebase Storage
        storage = FirebaseStorage.DefaultInstance;
        storageReference = storage.GetReferenceFromUrl("gs://zentsujimetaverse-421417.appspot.com/");
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
            StartCoroutine(DownloadAndPlay(query.BGMFileName));
        }
        else
        {
            Debug.Log("queryがnull");
        }
    }

    IEnumerator DownloadAndPlay(string fileName)
    {
        var path = $"BGM/{fileName}";
        var audioFileRef = storageReference.Child(path);
        var downloadTask = audioFileRef.GetDownloadUrlAsync();

        yield return new WaitUntil(() => downloadTask.IsCompleted);

        if (downloadTask.Exception != null)
        {
            Debug.LogError($"Failed to download {fileName}: {downloadTask.Exception}");
        }
        else
        {
            var audioUrl = downloadTask.Result.ToString();
            using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(audioUrl, AudioType.MPEG))
            {
                yield return www.SendWebRequest();
                if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
                {
                    Debug.LogError(www.error);
                }
                else
                {
                    AudioClip clip = DownloadHandlerAudioClip.GetContent(www);
                    audioSource.clip = clip;
                    audioSource.Play();
                }
            }
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
