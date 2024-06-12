using UnityEngine;
using Cysharp.Threading.Tasks;
using System;
using System.Threading;

public class NPCMove : MonoBehaviour
{
    [SerializeField, Header("待機ポイント")]
    private Transform[] m_WayPoints;
    [SerializeField, Header("移動速度")]
    private float m_MoveSpeed = 3f;
    [SerializeField, Header("回転速度")]
    private float m_RotationSpeed = 5f;
    [SerializeField, Header("移動時のブレンドシェイプインデックス")]
    private int m_MoveBlendShapeIndex = 0;
    [SerializeField, Header("移動停止時のブレンドシェイプインデックス")]
    private int m_StopBlendShapeIndex = 0;
    [SerializeField, Header("ブレンドシェイプの変化速度")]
    private float m_BlendShapeChangesSpeed = 1f;

    private int m_CurrentWayPointIndex = 0;
    private Transform m_TargetWaypoint;
    private CancellationTokenSource m_CancellationTokenSource;
    private Animator m_Animator;
    [SerializeField, Header("歩行時のパラメーター名")]
    private string m_WalkParameterName = "";

    private SkinnedMeshRenderer m_SkinnedMeshRenderer;
    private float m_CurrentBlendShapeWeight = 0f;

    void Start()
    {
        m_Animator = GetComponent<Animator>();
        m_SkinnedMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        if (m_WayPoints.Length == 0)
        {
            return;
        }
        m_TargetWaypoint = m_WayPoints[m_CurrentWayPointIndex];
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
                m_Animator.SetBool(m_WalkParameterName, true);
                await ChangeBlendShapeWeight(m_MoveBlendShapeIndex, 100f, token);
                await MoveAndRotateTowards(m_TargetWaypoint, token);
                m_Animator.SetBool(m_WalkParameterName, false);
                await ChangeBlendShapeWeight(m_StopBlendShapeIndex, 100f, token);

                Waypoint waypoint = m_TargetWaypoint.GetComponent<Waypoint>();
                if (!waypoint.isDoNotStop)
                {
                    Quaternion targetRotation = Quaternion.Euler(waypoint.waitRotation);
                    await SmoothRotateTo(targetRotation, token);
                    await UniTask.Delay(TimeSpan.FromSeconds(waypoint.waitTime), cancellationToken: token);
                }
                m_CurrentWayPointIndex = (m_CurrentWayPointIndex + 1) % m_WayPoints.Length;
                m_TargetWaypoint = m_WayPoints[m_CurrentWayPointIndex];
            }

                await UniTask.Yield(token);
        }
    }

    private async UniTask MoveAndRotateTowards(Transform target, CancellationToken token)
    {
        while (Vector3.Distance(transform.position, target.position) > 0.1f && !token.IsCancellationRequested)
        {
            Vector3 direction = (target.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * m_RotationSpeed * 0.5f); // 回転速度を半分に調整

            transform.position = Vector3.MoveTowards(transform.position, target.position, m_MoveSpeed * Time.deltaTime);

            await UniTask.Yield(token);
        }
    }

    private async UniTask SmoothRotateTo(Quaternion targetRotation, CancellationToken token)
    {
        while (Quaternion.Angle(transform.rotation, targetRotation) > 0.1f && !token.IsCancellationRequested)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * m_RotationSpeed * 0.5f); // 回転速度を半分に調整
            await UniTask.DelayFrame(1, cancellationToken: token); // フレームごとに遅延を挿入して滑らかにする
        }
        transform.rotation = targetRotation;
    }

    private async UniTask ChangeBlendShapeWeight(int index, float targetWeight, CancellationToken token)
    {
        while(Mathf.Abs(m_CurrentBlendShapeWeight-targetWeight)>0.1f&&!token.IsCancellationRequested)
        {
            m_CurrentBlendShapeWeight = Mathf.MoveTowards(m_CurrentBlendShapeWeight, targetWeight, m_BlendShapeChangesSpeed * Time.deltaTime);
            m_SkinnedMeshRenderer.SetBlendShapeWeight(index, m_CurrentBlendShapeWeight);
            await UniTask.Yield(token);
        }
        m_SkinnedMeshRenderer.SetBlendShapeWeight(index, targetWeight);
    }
    void OnDestroy()
    {
        m_CancellationTokenSource?.Cancel();
    }
}