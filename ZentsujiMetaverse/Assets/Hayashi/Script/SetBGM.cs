using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetBGM : MonoBehaviour
{
    [SerializeField, Header("開始時にセットするBGM")]
    private string m_BGMName;
    [SerializeField, Header("音量")]
    private float m_Volume;
    [SerializeField, Header("ランダムBGM")]
    private string[] m_RandomBGMNames;
    [SerializeField, Header("ランダム再生をオンにするかどうか")]
    private bool isRondomBGM;
    private void Start()
    {
        BGMManager.instance.GetComponent<AudioSource>().clip=null;
        if (m_BGMName != null)
        {
            BGMManager.instance.PlayBGMByScene(m_BGMName, m_Volume);
        }
    }

    private void Update()
    {
        if (isRondomBGM)
        {
            AudioSource audioSouce = BGMManager.instance.GetComponent<AudioSource>();
            audioSouce.loop = false;
            if(!audioSouce.isPlaying)
            {
                var index=Random.Range(0, m_RandomBGMNames.Length);
                BGMManager.instance.PlayBGMByScene(m_RandomBGMNames[index],m_Volume);
            }
        }
      
    }
}
