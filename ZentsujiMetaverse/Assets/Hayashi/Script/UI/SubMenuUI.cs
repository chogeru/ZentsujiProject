using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SubMenuUI : MonoBehaviour
{
    [SerializeField, Header("エモートUI")]
    private GameObject m_GestureUI;
    [ReadOnly]
    public bool isOpenUI=false;

    void Start()
    {
        m_GestureUI.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) || (Gamepad.current != null && Gamepad.current.buttonEast.isPressed))
        {
            isOpenUI = !isOpenUI; // isOpenUIの状態を反転させる

            // isOpenUIがtrueのときはUIを表示する、falseのときは非表示にする
            m_GestureUI.SetActive(isOpenUI);
        }
    }
}
