using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using System.ComponentModel;
using Unity.Collections;
using Unity.XR.CoreUtils;
using VInspector;
using System;
using R3;

public class LogoFadeEffect : MonoBehaviour
{
    [Tab("CanvasGroup")]
    [SerializeField]
    private CanvasGroup m_CanvasGroup;
    [EndTab]

    [Tab("フェード設定")]
    [SerializeField, Header("フェードインにかかる時間")]
    private float m_FadeInDuration=3.0f;
    [SerializeField, Header("完全に不透明になった後の表示時間")]
    private float m_VisibleDuration=2.0f;
    [SerializeField, Header("フェードインにかかる時間")]
    private float m_FadeOutDuration = 3.0f;

    //他のクラスから購読可能なイベント
    public Subject<Unit> OnFadeOutCompleted = new Subject<Unit>();
    private void Awake()
    {
        m_CanvasGroup=GetComponent<CanvasGroup>();
        //nullチェック
        if(m_CanvasGroup == null)
        {
            Debug.LogError("キャンバスグループが見つかりません");
            this.enabled = false;
        }
    }

    private async void Start()
    {
        // オブジェクトが破棄されたときにキャンセルされるCancellationTokenを取得
        CancellationToken ct =this.GetCancellationTokenOnDestroy();

        //最初は透明状態に
        m_CanvasGroup.alpha = 0;

        // 3秒かけて不透明にする
        await FadeCanvasGroup(m_CanvasGroup, 1f, m_FadeInDuration, ct);

        // 不透明の状態で2秒間待つ
        await UniTask.Delay((int)(m_VisibleDuration * 1000), cancellationToken: ct);

        // 3秒かけて再度透明にする
        await FadeCanvasGroup(m_CanvasGroup, 0f, m_FadeOutDuration, ct);

        // R3のSubjectを通じてイベントを発行
        OnFadeOutCompleted.OnNext(Unit.Default);
        OnFadeOutCompleted.OnCompleted();
    }
    private async UniTask FadeCanvasGroup(CanvasGroup cg, float targetAlpha, float duration, CancellationToken ct)
    {
        // 開始時の透明度を記録
        float startAlpha = cg.alpha;
        //経過時間を格納する変数
        float time = 0;
        // 経過時間が指定した持続時間に達するまでループ
        while (time < duration)
        {
            // Lerp関数を使用して透明度を徐々に変化させる
            cg.alpha=Mathf.Lerp(startAlpha, targetAlpha, time/duration);
            // 次のフレームまで待機し、この間にキャンセルが発生したかチェック
            await UniTask.Yield(PlayerLoopTiming.Update, ct);
            // 経過時間を更新
            time += Time.deltaTime;
        }
        // 最終的な透明度を設定
        cg.alpha = targetAlpha;
    }
}
