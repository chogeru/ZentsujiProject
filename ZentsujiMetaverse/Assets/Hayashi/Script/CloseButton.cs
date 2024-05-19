using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

public class CloseButton : MonoBehaviour
{
    private Button m_Button;
    private CanvasGroup m_CanvasGroup;

    void Start()
    {
        m_Button = GetComponent<Button>();
        m_Button.onClick.AddListener(() => CloseParentPanel().Forget()); // 非同期メソッドの呼び出し
        m_CanvasGroup = transform.parent.GetComponent<CanvasGroup>();
        if (m_CanvasGroup == null)
        {
            m_CanvasGroup = transform.parent.gameObject.AddComponent<CanvasGroup>();
        }
    }

    private async UniTask CloseParentPanel()
    {
        if (m_CanvasGroup != null)
        {
            // CanvasGroupが存在する場合、透明度を0にして非表示にする
            m_CanvasGroup.alpha = 0;
            m_CanvasGroup.blocksRaycasts = false;
            await UniTask.Delay(200); // 適切な遅延時間を設定
        }
        Cursor.visible = false;
    }

    public void CursorFalse()
    {
        Cursor.visible = false;
    }
}
