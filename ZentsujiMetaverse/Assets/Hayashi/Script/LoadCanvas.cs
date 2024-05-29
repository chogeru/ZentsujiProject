using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadCanvas : MonoBehaviour
{
    public static LoadCanvas instance;
    [SerializeField,Header("ロード時に表示されるオブジェクト")]
    public GameObject m_LoadUI;
    private void Awake()
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
    }
    private void Start()
    {
        m_LoadUI.SetActive(false);
    }
    public void SetUI()
    {
        m_LoadUI.SetActive(true);
    }
    public void CloseUI()
    {
        m_LoadUI.SetActive(false);
    }
}
