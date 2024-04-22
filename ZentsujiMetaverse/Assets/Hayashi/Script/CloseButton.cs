using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CloseButton : MonoBehaviour
{
    private Button button;

    void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(CloseParentPanel);
    }

    private void CloseParentPanel()
    {
        // 親パネルを非表示にする
        transform.parent.gameObject.SetActive(false);
    }
    public void CursorFalse()
    {
        Cursor.visible = false;
    }
}
