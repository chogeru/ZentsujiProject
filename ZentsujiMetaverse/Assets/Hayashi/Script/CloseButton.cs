using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

public class CloseButton : MonoBehaviour
{
    private Button m_Button;
    private Animator m_Animator;

    void Start()
    {
        m_Button = GetComponent<Button>();
        m_Button.onClick.AddListener(() => CloseParentPanel().Forget()); // 非同期メソッドの呼び出し
        m_Animator = transform.parent.GetComponent<Animator>();
    }

    private async UniTask CloseParentPanel()
    {
        if (m_Animator != null)
        {
            // アニメーターが存在する場合、クローズアニメーションを再生
            m_Animator.Play("Close");
            await UniTask.Delay((int)(m_Animator.GetCurrentAnimatorStateInfo(0).length * 1000)); // ミリ秒で待機
        }
        Cursor.visible = false;
    }
    public void CursorFalse()
    {
        Cursor.visible = false;
    }
}
