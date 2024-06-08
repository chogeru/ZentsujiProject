using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SubMenuUI : MonoBehaviour
{
    [SerializeField, Header("エモートUI")]
    private GameObject m_GestureUI;
    private bool isKeyOrButtonPressed = false;
    [ReadOnly]
    public bool isOpenUI=false;

    void Start()
    {
        m_GestureUI.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) || Gamepad.current?.buttonEast.isPressed == true && !isKeyOrButtonPressed)
        {
            isOpenUI = true;
            m_GestureUI.SetActive(true);
        }
        if (Input.GetKeyUp(KeyCode.E) | Gamepad.current?.wasUpdatedThisFrame == true)
        {
            isKeyOrButtonPressed = false;
        }
    }
}
