using Cysharp.Threading.Tasks;
using Mirror;
using R3;
using R3.Triggers;
using UnityEngine;
#region
/*
.Subscribe
Subscribe�́A�C�x���g�����������Ƃ��ɉ�������̃A�N�V�����i�Ⴆ�΁A�v���C���[�𓮂����j���N�������߂Ɏg��
���̃��\�b�h���g���āA�C�x���g�X�g���[�����Ď����A�C�x���g���������邽�тɃA�N�V���������s�����

.Where
Where�́A����̏����Ɋ�Â��ăC�x���g���t�B���^�����O���邽�߂Ɏg��
�W�����v�{�^���������ꂽ�Ƃ���,�v���C���[���n�ʂɐG��Ă���Ƃ��ȂǁA
�����ɍ����ꍇ�̂݃A�N�V�������N�������߂Ɏg�p

.Select
Select�́A���̃f�[�^��V�����`�ɕϊ����邽�߂Ɏg��
���̏������ƁA���[�U�[�̓��͂�V����Vector3�I�u�W�F�N�g�ɕϊ����āA������g�p���ăv���C���[�𓮂���

 .Share
�f�[�^�̏d����h�����߂̂���
�ϑ��\�ȃX�g���[���𕡐��̏ꏊ�ŋ��L����Ƃ��ɁA�����f�[�^�����x�����������̂�h��

�ϑ��\�ȃX�g���[���iObservable�j
���Ԃ̌o�߂ƂƂ��ɒl��C�x���g�𐶐�����f�[�^�̃X�g���[����\��
��̓I�ɂ́A�}�E�X�̈ړ��A�{�^���̃N���b�N�A�L�[�{�[�h�̓��́A�O���T�[�o�[����̃f�[�^�̎擾�ȂǁA
*/
#endregion

public class PlayerController : NetworkBehaviour
{
    [SerializeField, Header("�������x")]
    private float m_WalkSpeed = 5.0f;
    [SerializeField, Header("����X�s�[�h")]
    private float m_RunSpeed = 10.0f;
    [SerializeField, Header("�W�����v��")]
    public float m_JumpForce = 300f;

    private Rigidbody m_Rigidbody;
    [SerializeField]
    private Transform m_CameraTransform;

    // ���[�J���v���C���[���J�n�������ɌĂяo����郁�\�b�h
    public override void OnStartLocalPlayer()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        m_CameraTransform = Camera.main.transform;
        base.OnStartLocalPlayer();
        InitializeMovement();
    }
    // �v���C���[�̓��������������邽�߂̃��\�b�h
    void InitializeMovement()
    {
        // �v���C���[�̈ړ����͂����A�N�e�B�u�ɊĎ�
        var moveStream = this.UpdateAsObservable()
             .Select(_ => new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")))
             .Share();
        // Shift�L�[�������Ȃ���̑��s���w��
        moveStream
             .Where(_ => Input.GetKey(KeyCode.LeftShift))
             .Subscribe(movement => new RunCommand(this, movement).Execute());
        // �ʏ�̕��s���w��
        moveStream
            .Where(_ => !Input.GetKey(KeyCode.LeftShift))
            .Subscribe(movement => new WalkCommand(this, movement).Execute());
        // �W�����v�̓��͂ƒn�ʂɐG��Ă��邩�̊m�F
        this.UpdateAsObservable()
            .Where(_ => Input.GetButtonDown("Jump"))
            .Where(_ => IsGrounded())
            .Subscribe(_ => m_Rigidbody.AddForce(new Vector3(0.0f, m_JumpForce, 0.0f)));
    }

    // �v���C���[���w��̑��x�ňړ�
    public void Move(Vector3 movement, float speed)
    {
        Vector3 relativeMovement = m_CameraTransform.TransformDirection(movement);
        relativeMovement.y = 0; 

        if (movement != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(relativeMovement, Vector3.up);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 10);
        }

        m_Rigidbody.MovePosition(transform.position + relativeMovement * speed * Time.deltaTime);
    }

    // �v���C���[���n�ʂɐG��Ă��邩�ǂ����𔻒f
    bool IsGrounded()
    {
        Vector3 start = transform.position;
        Vector3 end = start - Vector3.up * 0.5f; // ����0.5f�Œn�ʂ��m�F

        bool isGrounded = Physics.Raycast(start, -Vector3.up, 1.5f);
        Color lineColor = isGrounded ? Color.green : Color.red;

        Debug.DrawLine(start, end, lineColor);
        return isGrounded;
    }
    // �R�}���h�p�^�[�����`����C���^�[�t�F�[�X
    private interface ICommand
    {
        void Execute();
    }
    // ���s���Ǘ�����R�}���h�N���X
    // ���̃N���X�̓v���C���[�̕��s�s�����J�v�Z�������A
    // �Ăяo���ꂽ�ۂɃv���C���[���w�肳�ꂽ�����Ƒ��x�ňړ�������

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
    // ���s���Ǘ�����R�}���h�N���X
    // ���̃N���X�̓v���C���[�̑��s�s�����J�v�Z�������A
    // �Ăяo���ꂽ�ۂɃv���C���[���w�肳�ꂽ�����ɍ����ňړ�������
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
