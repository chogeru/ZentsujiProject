using UnityEngine;
using UnityEngine.SceneManagement;
using SQLite4Unity3d;
using Cysharp.Threading.Tasks;

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
                    Debug.LogError("�I�[�f�B�I�N���b�v���Ȃ� " + query.ClipPath);
                }
            }
            else
            {
                Debug.LogError("�f�[�^�x�[�X�ɃT�E���h�N���b�v���Ȃ� " + clipName);
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError("�f�[�^�x�[�X�ւ̃A�N�Z�X���ɃG���[������ " + ex.Message);
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
