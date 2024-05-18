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

    private Animator[] m_Animators;

    void Start()
    {
        if (m_Buttons.Length != m_UIs.Length)
        {
            Debug.LogError("ボタンの数とUIの数が一致しません！");
            return;
        }

        m_Animators = new Animator[m_UIs.Length];
        for (int i = 0; i < m_UIs.Length; i++)
        {
            m_Animators[i] = m_UIs[i].GetComponent<Animator>();
            if (m_Animators[i] == null)
            {
                Debug.LogError("UIオブジェクトにAnimatorコンポーネントがありません: " + m_UIs[i].name);
                return;
            }
        }

        for (int i = 0; i < m_Buttons.Length; i++)
        {
            int index = i; // ローカル変数を使ってクロージャをキャプチャ
            m_Buttons[i].onClick.AddListener(() => ShowUI(index));
        }
    }

    void Update()
    {
        if (MenuUIManager.instance != null)
        {
            m_SubMenuUI.SetActive(!MenuUIManager.instance.isOpenUI);
        }
    }

    private void ShowUI(int index)
    {
        // すべてのUIのCloseアニメーションを再生する
        for (int i = 0; i < m_Animators.Length; i++)
        {
            if (m_Animators[i] != null)
            {
                m_Animators[i].Play("Close");
            }
        }

        // 指定されたUIのOpenアニメーションを再生する
        if (m_Animators[index] != null)
        {
            m_Animators[index].Play("Open");
        }
    }
}
