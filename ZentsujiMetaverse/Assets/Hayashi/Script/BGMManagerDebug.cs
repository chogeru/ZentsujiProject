using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMManagerDebug : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        // BGMManagerが存在するか確認
        BGMManager bgmManager = FindObjectOfType<BGMManager>();
        if (bgmManager != null)
        {
            Debug.Log("BGMManager found.");
            // BGMManagerが持つ情報をログに表示
            Debug.Log("BGM Name: " + bgmManager.m_BGMName);
            Debug.Log("Audio Source: " + bgmManager.GetComponent<AudioSource>());
            Debug.Log("Database Connection: " + bgmManager.m_Connection);
          //  Debug.Log(bgmManager.m_Storage);
          Debug.Log(bgmManager.m_StorageReference);
        }
        else
        {
            Debug.LogError("BGMManager not found.");
        }

    }
}
