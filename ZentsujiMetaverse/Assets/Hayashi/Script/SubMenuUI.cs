using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubMenuUI : MonoBehaviour
{
    [SerializeField, Header("サブメニューUI")]
    private GameObject m_SubMenuUI;
    public void Update()
    {
        if(MenuUIManager.instance != null)
            m_SubMenuUI.SetActive(!MenuUIManager.instance.isOpenUI);
    }
}
