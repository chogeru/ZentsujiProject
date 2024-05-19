using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SubMenuUI : MonoBehaviour
{
    [SerializeField, Header("サブメニューUI")]
    private GameObject m_SubMenuUI;

    [SerializeField, Header("ボタンリスト")]
    private Button[] m_Buttons;

    [SerializeField, Header("ボタンに対応するUIリスト")]
    private GameObject[] m_UIs;

    private CanvasGroup[] m_CanvasGroups;

    void Start()
    {
        if (m_Buttons.Length != m_UIs.Length)
        {
            Debug.LogError("ボタンの数とUIの数が一致しません！");
            return;
        }

        m_CanvasGroups = new CanvasGroup[m_UIs.Length];
        for (int i = 0; i < m_UIs.Length; i++)
        {
            m_CanvasGroups[i] = m_UIs[i].GetComponent<CanvasGroup>();
            if (m_CanvasGroups[i] == null)
            {
                m_CanvasGroups[i] = m_UIs[i].AddComponent<CanvasGroup>();
            }
            // 初期状態を設定する
            m_CanvasGroups[i].alpha = 0;
            m_CanvasGroups[i].blocksRaycasts = false;
            m_CanvasGroups[i].interactable = false;

        }

        for (int i = 0; i < m_Buttons.Length; i++)
        {
            int index = i;
            m_Buttons[i].onClick.AddListener(() => ShowUI(index));
        }
    }

    void Update()
    {
        if (MenuUIManager.instance != null)
        {
            m_SubMenuUI.SetActive(!MenuUIManager.instance.isOpenUI);
            if(MenuUIManager.instance.isOpenUI)
            {
                for(int i=0; i < m_CanvasGroups.Length; i++)
                {
                    if (m_CanvasGroups[i] != null)
                    {
                        m_CanvasGroups[i].alpha = 0;
                        m_CanvasGroups[i].blocksRaycasts = false;
                        m_CanvasGroups[i].interactable = false;
                    }
                }
            }
        }
    }

    private void ShowUI(int index)
    {
        
        for (int i = 0; i < m_CanvasGroups.Length; i++)
        {
            if (m_CanvasGroups[i] != null)
            {
                m_CanvasGroups[i].alpha = 0;
                m_CanvasGroups[i].blocksRaycasts = false;
                m_CanvasGroups[i].interactable = false; // これを追加
            }
        }
        
        if (m_CanvasGroups[index] != null)
        {
            m_CanvasGroups[index].alpha = 1;
            m_CanvasGroups[index].blocksRaycasts = true;
            m_CanvasGroups[index].interactable = true; // これを追加
        }
    }
}
