using Cysharp.Threading.Tasks;
using Mirror;
using R3;
using R3.Triggers;
using UnityEngine;
using UnityEngine.UIElements;
#region
/*
.Subscribe
Subscribeは、イベントが発生したときに何か特定のアクション（例えば、プレイヤーを動かす）を起こすために使う
このメソッドを使って、イベントストリームを監視し、イベントが発生するたびにアクションが実行される

.Where
Whereは、特定の条件に基づいてイベントをフィルタリングするために使う
ジャンプボタンが押されたときや,プレイヤーが地面に触れているときなど、
条件に合う場合のみアクションを起こすために使用

.Select
Selectは、元のデータを新しい形に変換するために使う
この処理だと、ユーザーの入力を新しいVector3オブジェクトに変換して、それを使用してプレイヤーを動かす

 .Share
データの重複を防ぐためのもの
観測可能なストリームを複数の場所で共有するときに、同じデータが何度も生成されるのを防ぐ

観測可能なストリーム（Observable）
時間の経過とともに値やイベントを生成するデータのストリームを表す
具体的には、マウスの移動、ボタンのクリック、キーボードの入力、外部サーバーからのデータの取得など、

*/
#endregion

public class PlayerController : NetworkBehaviour
{
    [SerializeField, Header("歩き速度")]
    private float m_WalkSpeed = 5.0f;
    [SerializeField, Header("走りスピード")]
    private float m_RunSpeed = 10.0f;
    [SerializeField, Header("ジャンプ力")]
    public float m_JumpForce = 300f;
    [SerializeField, Header("重力係数")]
    private float m_GravityMultiplier = 9.81f;
    [SerializeField]
    private Rigidbody m_Rigidbody;
    [SerializeField]
    private Transform m_CameraTransform;
    [SerializeField, Header("アニメ-ター")]
    private Animator m_Animator;
    [SerializeField]
    private LayerMask m_LayerMask;
    [SerializeField]
    private CapsuleCollider capsuleCollider;
    private ReactiveProperty<bool> isIdle = new ReactiveProperty<bool>(true);
    private ReactiveProperty<bool> isWalk = new ReactiveProperty<bool>(false);
    private ReactiveProperty<bool> isRun = new ReactiveProperty<bool>(false);
    // ローカルプレイヤーが開始した時に呼び出されるメソッド
    public override void OnStartLocalPlayer()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        capsuleCollider = GetComponent<CapsuleCollider>();  // CapsuleCollider を取得

        m_Rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        base.OnStartLocalPlayer();
        InitializeMovement().Forget();
        BindAnimations();

    }

    // プレイヤーの動きを非同期的に初期化
    private async UniTaskVoid InitializeMovement()
    {
        // UIが開かれている間はプレイヤーの速度をゼロに設定
        this.UpdateAsObservable()
        .Where(_ => MenuUIManager.instance.isOpenUI)
        .Subscribe(_ => m_Rigidbody.velocity = Vector3.zero);

        // プレイヤーの移動入力をリアクティブに監視
        var moveStream = this.UpdateAsObservable()
         .Where(_ => !MenuUIManager.instance.isOpenUI)
         .Select(_ => new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")))
         .Share();
        // Shiftキーを押しながらの走行を購読
        moveStream
             .Where(_ => Input.GetKey(KeyCode.LeftShift))
             .Subscribe(movement => new RunCommand(this, movement).Execute());
        // 通常の歩行を購読
        moveStream
            .Where(_ => !Input.GetKey(KeyCode.LeftShift))
            .Subscribe(movement => new WalkCommand(this, movement).Execute());

        // ジャンプの入力と地面に触れているかの確認
        this.UpdateAsObservable()
            .Where(_ => !MenuUIManager.instance.isOpenUI)
            .Where(_ => Input.GetButtonDown("Jump"))
            .Where(_ => IsGrounded())
            .Subscribe(_ => m_Rigidbody.AddForce(new Vector3(0.0f, m_JumpForce, 0.0f)));

        await UniTask.Yield();

    }
    void Update()
    {
        UseGravity();
        // 段差登りを試みる
        TryStepUp();
    }
    private void UseGravity()
    {
        // 重力の追加
        m_Rigidbody.AddForce(Physics.gravity * m_Rigidbody.mass * m_GravityMultiplier);
    }
    void TryStepUp()
    {
        // プレイヤーの前方0.3ユニットの位置から、カプセルコライダーの高さに基づくレイキャストを設定
        Vector3 forward = transform.forward * 0.3f;
        Vector3 rayStart = transform.position + forward + Vector3.up * capsuleCollider.height * 0.5f; // レイキャスト開始点はカプセルの中心点より
        Vector3 rayEnd = transform.position + forward + Vector3.down * 0.1f; // カプセルコライダーの底近くまで

        // レイキャストの終了点を地面すぐ上に設定
        RaycastHit hitInfo;
        if (Physics.Raycast(rayStart, Vector3.down, out hitInfo, capsuleCollider.height * 0.6f,m_LayerMask))
        {
            // レイキャストが何かにヒットし、その高さが適切であればプレイヤーを移動
            float stepHeight = hitInfo.point.y - transform.position.y;
            if (stepHeight <= capsuleCollider.height / 3 && stepHeight > 0)
            {
                // プレイヤーの位置を段差の上に更新
                transform.position = new Vector3(transform.position.x, hitInfo.point.y + stepHeight, transform.position.z);
            }
        }

        Debug.DrawLine(rayStart, rayEnd, Color.blue); // デバッグ用のレイキャスト表示
    }

    // アニメーションの状態をバインドするメソッド
    private void BindAnimations()
    {
        // ReactivePropertyを使ってアニメータの各状態を購読し、変化があるたびにAnimatorへ反映
        isIdle.Subscribe(idle => m_Animator.SetBool("Idle", idle)).AddTo(this);
        isWalk.Subscribe(walk => m_Animator.SetBool("Walk", walk)).AddTo(this);
        isRun.Subscribe(run => m_Animator.SetBool("Run", run)).AddTo(this);

        // 入力軸に基づくアニメーション変数もリアクティブに更新
        this.UpdateAsObservable()
            .Select(_ => Input.GetAxis("Horizontal"))
            .Subscribe(horizontal =>
            {
                m_Animator.SetFloat("左右", horizontal);
                m_Animator.SetFloat("走り左右", horizontal);
            }).AddTo(this);

        this.UpdateAsObservable()
            .Select(_ => Input.GetAxis("Vertical"))
            .Subscribe(vertical =>
            {
                m_Animator.SetFloat("前後", vertical);
                m_Animator.SetFloat("走り前後", vertical);
            }).AddTo(this);
    }
    // プレイヤーを指定の速度で移動
    public void Move(Vector3 movement, float speed)
    {
        if (MenuUIManager.instance.isOpenUI)
        {
            m_Rigidbody.velocity = Vector3.zero;
            UpdateState(true, false, false);
            return;
        }

        Vector3 relativeMovement = m_CameraTransform.TransformDirection(movement);
        relativeMovement.y = 0;

        if (movement != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(relativeMovement, Vector3.up);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 10);
            // 移動時の状態
            UpdateState(false, !isRun.Value, isRun.Value);
        }
        else
        {
            UpdateState(true, false, false);
        }

        m_Rigidbody.velocity = relativeMovement * speed;
    }

    // プレイヤーの状態を更新するメソッド
    private void UpdateState(bool idle, bool walk, bool run)
    {
        isIdle.Value = idle;
        isWalk.Value = walk;
        isRun.Value = run;
    }

    // 地面に触れているかどうかを確認するメソッド
    bool IsGrounded()
    {
        Vector3 start = transform.position + Vector3.up * 0.1f;
        Vector3 end = start - Vector3.up * 0.5f;

        bool isGrounded = Physics.Raycast(start, -Vector3.up, out RaycastHit hit, 0.5f);
        Debug.DrawLine(start, end, isGrounded ? Color.green : Color.red);
        return isGrounded && hit.normal.y > 0.5;  // 地面が十分に水平であることを確認
    }
    // コマンドパターンを定義するインターフェース
    private interface ICommand
    {
        void Execute();
    }
    // 歩行を管理するコマンドクラス
    // このクラスはプレイヤーの歩行行動をカプセル化し、
    // 呼び出された際にプレイヤーを指定された方向と速度で移動させる

    private class WalkCommand : ICommand
    {
        private PlayerController m_Player;
        private Vector3 m_Direction;

        public WalkCommand(PlayerController player, Vector3 direction)
        {
            this.m_Player = player;
            this.m_Direction = direction;
        }

        public void Execute()
        {
            m_Player.UpdateState(false, true, false);
            m_Player.Move(m_Direction, m_Player.m_WalkSpeed);
        }
    }
    // 走行を管理するコマンドクラス
    // このクラスはプレイヤーの走行行動をカプセル化し、
    // 呼び出された際にプレイヤーを指定された方向に高速で移動させる
    private class RunCommand : ICommand
    {
        private PlayerController m_Player;
        private Vector3 m_Direction;

        public RunCommand(PlayerController player, Vector3 direction)
        {
            this.m_Player = player;
            this.m_Direction = direction;
        }

        public void Execute()
        {
            m_Player.UpdateState(false, false, true);
            m_Player.Move(m_Direction, m_Player.m_RunSpeed);
        }
    }

}
