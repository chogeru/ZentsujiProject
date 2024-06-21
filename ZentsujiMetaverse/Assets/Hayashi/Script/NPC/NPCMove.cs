using UnityEngine;
using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using VInspector;
//UniTask分からない人向け
//https://qiita.com/IShix/items/dcf86cb5ca1b587a88ad
public class NPCMove : MonoBehaviour
{
    #region 各ポイント
    [Tab("各ポイント")]
    [SerializeField, Header("待機ポイント")]
    public Transform[] m_WayPoints;
    [EndTab]
    #endregion

    #region プロパティ
    [Tab("プロパティ")]
    [SerializeField, Header("移動速度")]
    public float m_MoveSpeed = 3f;
    [SerializeField, Header("回転速度")]
    public float m_RotationSpeed = 5f;
    [EndTab]
    #endregion

    #region　アニメーション
    [Tab("アニメーション")]
    [SerializeField, Header("アニメター")]
    public Animator m_Animator;
    [SerializeField, Header("歩行時のパラメーター名")]
    public string m_WalkParameterName = "";
    [EndTab]
    #endregion

    //現在の移動ポイントのインデックス
    private int m_CurrentWayPointIndex = 0;
    //現在ターゲットのポイント
    private Transform m_TargetWaypoint;
    //キャンセルトークンソース
    private CancellationTokenSource m_CancellationTokenSource;

    void Start()
    {
        //アニメター所得
        m_Animator = GetComponent<Animator>();
        if (m_WayPoints.Length == 0)
        {
            //待機用ポイントが設定されてない場合終了
            return;
        }
        //最初のターゲットの設定
        m_TargetWaypoint = m_WayPoints[m_CurrentWayPointIndex];
        //非同期移動処理の開始
        MoveToNextWaypointAsync().Forget();
    }

    private async UniTaskVoid MoveToNextWaypointAsync()
    {
        m_CancellationTokenSource = new CancellationTokenSource();
        CancellationToken token = m_CancellationTokenSource.Token;

        while (true)
        {
            if (m_TargetWaypoint != null)
            {
                //アニメーション開始
                m_Animator.SetBool(m_WalkParameterName, true);
                //ターゲットウェイトポイントに移動
                await MoveAndRotateTowards(m_TargetWaypoint, token);
                //歩行アニメーションを停止
                m_Animator.SetBool(m_WalkParameterName, false);

                Waypoint waypoint = m_TargetWaypoint.GetComponent<Waypoint>();

                if (!waypoint.isDoNotStop)
                {
                    Quaternion targetRotation = Quaternion.Euler(waypoint.waitRotation);
                    //指定された回転にスムーズに回転
                    await SmoothRotateTo(targetRotation, token);
                    //ウェイトポイントで指定された時間待機
                    await UniTask.Delay(TimeSpan.FromSeconds(waypoint.waitTime), cancellationToken: token);
                }
                //次のウェイトポイントに移動
                m_CurrentWayPointIndex = (m_CurrentWayPointIndex + 1) % m_WayPoints.Length;
                m_TargetWaypoint = m_WayPoints[m_CurrentWayPointIndex];
            }
            //次のフレームまで待機
            await UniTask.Yield(token);
        }
    }

    //ターゲットに向かって移動および回転する非同期関数
    private async UniTask MoveAndRotateTowards(Transform target, CancellationToken token)
    {
        while (Vector3.Distance(transform.position, target.position) > 0.1f && !token.IsCancellationRequested)
        {
            Vector3 direction = (target.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            RaycastHit hit;
            if(Physics.Raycast(new Ray(transform.position+Vector3.up,direction),out hit,1.0f))
            {
                if(hit.collider.CompareTag("NPC"))
                {

                }
            }
            //回転速度を半分に調整
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * m_RotationSpeed * 0.5f);
            //移動
            transform.position = Vector3.MoveTowards(transform.position, target.position, m_MoveSpeed * Time.deltaTime);
            await UniTask.Yield(token);
        }
    }

    //指定の回転方向にスムーズに回転する非同期関数
    private async UniTask SmoothRotateTo(Quaternion targetRotation, CancellationToken token)
    {
        while (Quaternion.Angle(transform.rotation, targetRotation) > 0.1f && !token.IsCancellationRequested)
        {
            //回転速度を半分に調整
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * m_RotationSpeed * 0.5f);
            //フレームごとに遅延を挿入して滑らかにする
            await UniTask.DelayFrame(1, cancellationToken: token);
        }
        //回転を設定
        transform.rotation = targetRotation;
    }

    void OnDestroy()
    {
        //オブジェクトが破壊されるときにキャンセル
        m_CancellationTokenSource?.Cancel();
        m_CancellationTokenSource?.Dispose();
    }
}