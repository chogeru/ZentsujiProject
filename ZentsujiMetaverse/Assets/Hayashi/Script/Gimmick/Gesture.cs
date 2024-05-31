using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using R3;
using VInspector;
using UnityEngine.UI;

public class Gesture : MonoBehaviour
{
    [Tab("アニメーター")]
    [SerializeField, Header("アニメーションコンポーネントをセット")]
    private Animator m_Animator;
    [EndTab]
    [Tab("アニメーションクリップ")]
    [SerializeField,Header("セットするクリップ")]
    private AnimationClip[] m_AnimationClip;
    [EndTab]
    [Tab("ボタン")]
    [SerializeField,Header("各ジェスチャー用のButton")]
    private Button[] m_GustureButton;


    private void Start()
    {
        m_Animator = GetComponent<Animator>();
        SetUpButtonClickIvent();
    }
    private void SetUpButtonClickIvent()
    {
        for(int i=0; i<m_GustureButton.Length; i++)
        {
            int index = i;
            m_GustureButton[i].onClick.AddListener(() => PlayAnimation(index));
        }
    }
    async void PlayAnimation(int index)
    {
        //アニメーションクリップを直接再生
        AnimationClip clip = m_AnimationClip[index];
        PlayClip(clip);
        // プレイヤーが移動を開始したかどうかを監視するためのTaskを作成
        var playerMovementTask = WaitForMovement();
        //10秒待機
        await UniTask.Delay(10000);
        // 5秒待つTask
        var delayTask = UniTask.Delay(10000);

        // プレイヤーが移動するか、5秒経過するのを待つ
        await UniTask.WhenAny(playerMovementTask, delayTask);

        // アニメーションをデフォルトに戻す
        m_Animator.Play("Idle");
    }
    async UniTask WaitForMovement()
    {
        Vector3 startPosition = transform.position;
        while (true)
        {
            await UniTask.Yield();
            if (Vector3.Distance(startPosition, transform.position) > 0.01f) // 0.1fは移動とみなす最小距離
            {
                m_Animator.Play("Idle");
                break;
            }
        }
    }
    void PlayClip(AnimationClip clip)
    {
        if (clip != null)
        {
            m_Animator.Play(clip.name);
        }
        else
        {
            Debug.Log("null");
        }
    }

}
