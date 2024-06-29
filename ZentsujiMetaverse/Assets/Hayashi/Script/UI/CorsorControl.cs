using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CorsorControl : MonoBehaviour
{
    public static CorsorControl instacne;

    [SerializeField, Header("サブメニューUI")]
    private SubMenuUI m_SubMenuUI;
    private void Update()
    {
        CursorUpdate();
    }
    public void CursorUpdate()
    {
        SetCursor();
    }
    public void SetCursor()
    {
        if (MenuUIManager.instance != null && m_SubMenuUI != null)
        {
            bool shouldShowCursor = MenuUIManager.instance.isOpenUI || m_SubMenuUI.isOpenUI;

            if (shouldShowCursor && !Cursor.visible)
            {
                Cursor.visible = true;
            }
            else if (!shouldShowCursor && Cursor.visible)
            {
                Cursor.visible = false;
            }
        }
    }
}
