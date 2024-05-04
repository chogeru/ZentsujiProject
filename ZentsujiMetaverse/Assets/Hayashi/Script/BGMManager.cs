using Firebase;
using Firebase.Storage;
using Firebase.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using SQLite4Unity3d;

public class BGMManager : MonoBehaviour
{
    // シングルトンインスタンス
    public static BGMManager instance; 
    // オーディオソースコンポーネント
    private AudioSource m_AudioSource;   
    // SQLiteデータベースへの接続
    public SQLiteConnection m_Connection;
    // 再生するBGMの名前
    public string m_BGMName;           
    // Firebase Storageへの参照
    public FirebaseStorage m_Storage;     
    // ストレージ内の特定の位置への参照
    public StorageReference m_StorageReference;

    void Awake()
    {
        Debug.Log("BGMManager found.");

        // シングルトンパターンの実装
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeDatabase();
            m_Storage = FirebaseStorage.DefaultInstance;
            m_StorageReference = m_Storage.GetReferenceFromUrl("gs://zentsujimetaverse-421417.appspot.com/");
        }
        else
        {
            // 既にインスタンスがあれば削除
            Destroy(gameObject);
            return;
        }
        Debug.Log("BGMManager found.");

        // オーディオソースコンポーネントの取得
        m_AudioSource = GetComponent<AudioSource>();
    }
    void Start()
    {
        PlayBGMByScene(m_BGMName);
    }

    void InitializeDatabase()
    {
        // データベースのパスの設定と接続の試み
        var databasePath = System.IO.Path.Combine(Application.streamingAssetsPath, "bgm_data.db").Replace("\\", "/");
        Debug.Log("Database path: " + databasePath);

        try
        {
            m_Connection = new SQLiteConnection(databasePath, SQLiteOpenFlags.ReadOnly);
            Debug.Log("Database connected successfully.");
        }
        catch (Exception ex)
        {
            Debug.LogError("Database connection failed: " + ex.Message);
        }
    }

    // 指定されたBGM名に基づいてBGMを再生するメソッド
    public void PlayBGMByScene(string bgmName)
    {
        if (m_Connection == null)
        {
            Debug.LogError("Database connection is not established.");
            return;
        }
        // データベースを検索して指定されたBGM名のデータを取得
        var query = m_Connection.Table<BGM>().Where(x => x.BGMName == bgmName).FirstOrDefault();
        if (query != null)
        {
            // 対応するBGMファイル名でBGMのダウンロードと再生を開始
            StartCoroutine(DownloadAndPlay(query.BGMFileName));
        }
        else
        {
            // データベースにBGM名が見つからない場合はエラーログを出力
            Debug.LogError("データベースにサウンドクリップがない " + bgmName);
        }
    }

    // Firebase StorageからBGMファイルをダウンロードし、再生するコルーチン
    IEnumerator DownloadAndPlay(string fileName)
    {
        // Firebase Storageのパスを構築
        var path = $"BGM/{fileName}";
        var audioFileRef = m_StorageReference.Child(path);
        // ファイルのダウンロードURLを非同期で取得開始
        var downloadTask = audioFileRef.GetDownloadUrlAsync();
        Debug.Log("Attempting to download file: " + path);

        // ダウンロードタスクの完了を待機
        yield return new WaitUntil(() => downloadTask.IsCompleted);

        if (downloadTask.Exception != null)
        {
            // ダウンロード中にエラーが発生した場合、エラーログを出力
            Debug.LogError($"Failed to download {fileName}: {downloadTask.Exception}");
        }
        else
        {
            // ダウンロードが成功した場合、ダウンロードしたファイルのURLを取得
            var audioUrl = downloadTask.Result.ToString();
            Debug.Log("File downloaded successfully: " + audioUrl);

            // URLからオーディオクリップを非同期でロード
            using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(audioUrl, AudioType.MPEG))
            {
                Debug.Log("Loading audio clip from URL: " + audioUrl);
                yield return www.SendWebRequest(); // リクエストの送信と完了を待機

                if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
                {
                    Debug.LogError("Error loading audio clip: " + www.error);
                    // ネットワークエラーまたはプロトコルエラーが発生した場合、エラーログを出力
                    Debug.LogError(www.error);
                }
                else
                {
                    AudioClip clip = DownloadHandlerAudioClip.GetContent(www);
                    if (clip != null)
                    {
                        m_AudioSource.clip = clip;
                        m_AudioSource.Play();
                        Debug.Log("Audio clip played successfully.");
                    }
                    else
                    {
                        Debug.LogError("Failed to load audio clip from downloaded file.");
                    }
                }
            }
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
