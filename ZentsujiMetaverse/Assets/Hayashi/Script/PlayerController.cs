using Cysharp.Threading.Tasks;
using Mirror;
using R3;
using R3.Triggers;
using UnityEngine;
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

    private Rigidbody m_Rigidbody;
    [SerializeField]
    private Transform m_CameraTransform;

    // ローカルプレイヤーが開始した時に呼び出されるメソッド
    public override void OnStartLocalPlayer()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        m_CameraTransform = Camera.main.transform;
        m_Rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        base.OnStartLocalPlayer();
        InitializeMovement();
    }
    // プレイヤーの動きを初期化するためのメソッド
    void InitializeMovement()
    {
        // UIが開かれた瞬間にプレイヤーの速度をゼロにリセット
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

    }

    // プレイヤーを指定の速度で移動
    public void Move(Vector3 movement, float speed)
    {
        if (MenuUIManager.instance.isOpenUI)
        {
            m_Rigidbody.velocity = Vector3.zero;
            return;
        }

        Vector3 relativeMovement = m_CameraTransform.TransformDirection(movement);
        relativeMovement.y = 0;

        if (movement != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(relativeMovement, Vector3.up);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 10);
        }

        m_Rigidbody.velocity = relativeMovement * speed;
    }

    // プレイヤーが地面に触れているかどうかを判断
    bool IsGrounded()
    {
        
        Vector3 start = transform.position;
        Vector3 end = start - Vector3.up * 0.5f; // 距離0.5fで地面を確認

        bool isGrounded = Physics.Raycast(start, -Vector3.up, 1.5f);
        Color lineColor = isGrounded ? Color.green : Color.red;

        Debug.DrawLine(start, end, lineColor);
        return isGrounded;
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
            m_Player.Move(m_Direction, m_Player.m_RunSpeed);
        }
    }
}
