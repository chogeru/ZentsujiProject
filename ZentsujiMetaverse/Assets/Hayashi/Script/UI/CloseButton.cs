using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using AbubuResouse.Singleton;

public class CloseButton : MonoBehaviour
{
    [SerializeField, Header("ボタン押したときのSE名")]
    private string m_SEName;
    [SerializeField, Header("サウンドの音量")]
    private float m_Volume;

    //自身のボタン
    private Button m_Button;
    //閉じるキャンバス
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
            BGMManager.Instance.PlaySound(m_SEName,m_Volume);
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
