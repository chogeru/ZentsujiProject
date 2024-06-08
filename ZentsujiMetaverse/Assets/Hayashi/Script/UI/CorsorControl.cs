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
        CloseCursor();
    }
    public void SetCursor()
    {
       if(MenuUIManager.instance != null||m_SubMenuUI!=null)
        {
           if(MenuUIManager.instance.isOpenUI==true||
              m_SubMenuUI.isOpenUI==true)
            {
                Cursor.visible = true;
            }
        }
    }
    public void CloseCursor()
    {
        if (MenuUIManager.instance != null || m_SubMenuUI!= null)
        {
            if (MenuUIManager.instance.isOpenUI == false ||
               m_SubMenuUI == false)
            {
                Cursor.visible = false;
            }
        }
    }
}
