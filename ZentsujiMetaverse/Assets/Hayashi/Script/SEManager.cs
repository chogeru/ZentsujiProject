using UnityEngine;
using UnityEngine.SceneManagement;
using SQLite4Unity3d;
using Cysharp.Threading.Tasks;
using System;

public class SEManager : MonoBehaviour
{
    public static SEManager instance;
    private AudioSource audioSource;
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
        var databasePath = System.IO.Path.Combine(Application.streamingAssetsPath, "se_data.db").Replace("\\", "/");
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

    public void PlaySound(string clipName)
    {
        try
        {
            var query = connection.Table<SoundClip>().Where(x => x.ClipName == clipName).FirstOrDefault();
            if (query != null)
            {
                AudioClip clip = Resources.Load<AudioClip>("SE/"+ query.ClipPath);
                if (clip != null)
                {
                    audioSource.PlayOneShot(clip);
                }
                else
                {
                    Debug.LogError("オーディオクリップがない " + query.ClipPath);
                }
            }
            else
            {
                Debug.LogError("データベースにサウンドクリップがない " + clipName);
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError("データベースへのアクセス中にエラーが発生 " + ex.Message);
        }
    }

    class SoundClip
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string ClipName { get; set; }
        public string ClipPath { get; set; }
    }
}
