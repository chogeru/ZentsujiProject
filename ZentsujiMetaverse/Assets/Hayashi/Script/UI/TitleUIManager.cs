using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleUIManager : MonoBehaviour
{
    [SerializeField, Header("選択するボタン")]
    private List<Button> m_Buttons;
    //選択されているボタンのインデックス
    [SerializeField, ReadOnly]
    private int m_SelectedButtonIndex = 0;
    [SerializeField, Header("選択SE")]
    private string m_UISelectSE;
    [SerializeField,Header("自身のグループ")]
    private CanvasGroup m_CanvasGroup;

    // ボタンが押されたかどうかを管理するフラグ
    private bool m_ButtonPressed = false;
    void Start()
    {
        SelectButton(m_SelectedButtonIndex);

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            SEManager.instance.PlaySound(m_UISelectSE);
            ChangeSelectedButton(-1);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            SEManager.instance.PlaySound(m_UISelectSE);
            ChangeSelectedButton(1);
        }
       else if (Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.Return))
        {
            // ボタンが押されておらず、UIが表示されている場合のみ処理する
            if (!m_ButtonPressed && m_CanvasGroup.alpha >= 1)
            {
                PressSelectedButton();
            }
        }
    }

    void PressSelectedButton()
    {
        if (m_SelectedButtonIndex >= 0 && m_SelectedButtonIndex < m_Buttons.Count)
        {
            // ボタンを取得して押す
            m_Buttons[m_SelectedButtonIndex].onClick.Invoke();

            // ボタンが押されたフラグを立てる
            m_ButtonPressed = true;
        }
    }

    void SelectButton(int index)
    {
        foreach (Button button in m_Buttons)
        {
            // すべてのボタンを有効にする
            button.interactable = true;
            // 子オブジェクトのImageを非アクティブにする
            button.transform.GetChild(0).gameObject.SetActive(false);
        }

        // 選択されたボタンを選択状態にする
        m_Buttons[index].Select();
        // 選択されたボタンの子オブジェクトのImageをアクティブにする
        m_Buttons[index].transform.GetChild(0).gameObject.SetActive(true);
    }

    // 選択されているボタンを変更する
    void ChangeSelectedButton(int direction)
    {
        m_SelectedButtonIndex += direction;

        // リストの範囲外にならないように調整する
        if (m_SelectedButtonIndex < 0)
        {
            m_SelectedButtonIndex = m_Buttons.Count - 1;
        }
        else if (m_SelectedButtonIndex >= m_Buttons.Count)
        {
            m_SelectedButtonIndex = 0;
        }

        // ボタンを選択する
        SelectButton(m_SelectedButtonIndex);
    }

}
