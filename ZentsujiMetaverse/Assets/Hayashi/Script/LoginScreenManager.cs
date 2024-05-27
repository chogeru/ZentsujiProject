using UnityEngine;
using R3;
using Cysharp.Threading.Tasks;
using System;

public class LoginScreenManager : MonoBehaviour
{
    [SerializeField,Header("LoginCanvasのLogoFadeEffect コンポーネントの参照")]
    private LogoFadeEffect m_LogoFadeEffect;
    [SerializeField,Header("ログイン画面を制御するためのCanvasGroup")]
    private CanvasGroup m_LoginCanvasGroup;
    [SerializeField,Header("フェードインに掛かる時間")]
    private float m_FadeInDuration = 3.0f;
    [SerializeField, Header("フェードイン開始までの時間")]
    private float m_FadeInStartDelay = 2.0f;

    private void Start()
    {
        m_LoginCanvasGroup.alpha = 0f;
        //初期状態でインタラクション不可に設定
        m_LoginCanvasGroup.interactable = false;
        //nullチェック
        if (m_LogoFadeEffect != null)
        {
            // LogoFadeEffect の OnFadeOutCompleted イベントに対して購読します。
            // R3を使用してイベントが発生した際に StartLoginFadeIn メソッドを呼び出します。
            // AddTo(this) は、この MonoBehaviour が破棄されたときに購読を自動的に解除します。
            m_LogoFadeEffect.OnFadeOutCompleted.Subscribe(_ => DelayedFadeIn()).AddTo(this);
        }
        else
        {
            // LogoFadeEffect が設定されていない場合、直接遅延後にフェードインを開始
            DelayedFadeIn();
        }
    }
    public void Update()
    {
        if(SkipPerformanceManager.Instance.isSlip)
        {
            SkipFadeCanvas();
        }
    }
    private async void DelayedFadeIn()
    {
        await UniTask.Delay(TimeSpan.FromSeconds(m_FadeInStartDelay));
        StartLoginFadeIn();
    }

    private async void StartLoginFadeIn()
    {
        // 指定された遅延時間後にフェードイン処理を開始
        Debug.Log("ログイン画面のフェードインを開始します。");
        // CanvasGroupの透明度を非同期的に不透明に変更。
        await FadeCanvasGroup(m_LoginCanvasGroup, 1f, m_FadeInDuration);
        //フェードイン完了後にインタラクションを有効化
        m_LoginCanvasGroup.interactable = true;
    }

    private async UniTask FadeCanvasGroup(CanvasGroup cg, float targetAlpha, float duration)
    {
        // フェード処理開始時の透明度を記録。
        float startAlpha = cg.alpha;
        //フェード処理開始時にインタラクションを無効化
        cg.interactable = false;
        float time = 0;
        // 経過時間が設定された持続時間に達するまでループを続ける。
        while (time < duration)
        {
            // Mathf.Lerp を使用して、現在の透明度から目標の透明度へと徐々に変化させる。
            cg.alpha = Mathf.Lerp(startAlpha, targetAlpha, time / duration);
            // UniTask.Yield で次のフレームまで待機。
            // PlayerLoopTiming.Update を指定して、毎フレームの更新タイミングで処理を再開。
            await UniTask.Yield(PlayerLoopTiming.Update);

            time += Time.deltaTime;
        }
        cg.alpha = targetAlpha;
        //フェード処理完了後にインタラクションを再度有効化
        cg.interactable = true;
    }
    private void SkipFadeCanvas()
    {
        m_LoginCanvasGroup.alpha = 1f;
        m_LoginCanvasGroup.interactable = true;
    }
}
