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

    [Tab("�t�F�[�h�ݒ�")]
    [SerializeField, Header("�t�F�[�h�C���ɂ����鎞��")]
    private float m_FadeInDuration=3.0f;
    [SerializeField, Header("���S�ɕs�����ɂȂ�����̕\������")]
    private float m_VisibleDuration=2.0f;
    [SerializeField, Header("�t�F�[�h�C���ɂ����鎞��")]
    private float m_FadeOutDuration = 3.0f;

    //���̃N���X����w�ǉ\�ȃC�x���g
    public Subject<Unit> OnFadeOutCompleted = new Subject<Unit>();
    private void Awake()
    {
        m_CanvasGroup=GetComponent<CanvasGroup>();
        //null�`�F�b�N
        if(m_CanvasGroup == null)
        {
            Debug.LogError("�L�����o�X�O���[�v��������܂���");
            this.enabled = false;
        }
    }

    private async void Start()
    {
        // �I�u�W�F�N�g���j�����ꂽ�Ƃ��ɃL�����Z�������CancellationToken���擾
        CancellationToken ct =this.GetCancellationTokenOnDestroy();

        //�ŏ��͓�����Ԃ�
        m_CanvasGroup.alpha = 0;

        // 3�b�����ĕs�����ɂ���
        await FadeCanvasGroup(m_CanvasGroup, 1f, m_FadeInDuration, ct);

        // �s�����̏�Ԃ�2�b�ԑ҂�
        await UniTask.Delay((int)(m_VisibleDuration * 1000), cancellationToken: ct);

        // 3�b�����čēx�����ɂ���
        await FadeCanvasGroup(m_CanvasGroup, 0f, m_FadeOutDuration, ct);

        // R3��Subject��ʂ��ăC�x���g�𔭍s
        OnFadeOutCompleted.OnNext(Unit.Default);
        OnFadeOutCompleted.OnCompleted();
    }
    private async UniTask FadeCanvasGroup(CanvasGroup cg, float targetAlpha, float duration, CancellationToken ct)
    {
        // �J�n���̓����x���L�^
        float startAlpha = cg.alpha;
        //�o�ߎ��Ԃ��i�[����ϐ�
        float time = 0;
        // �o�ߎ��Ԃ��w�肵���������ԂɒB����܂Ń��[�v
        while (time < duration)
        {
            // Lerp�֐����g�p���ē����x�����X�ɕω�������
            cg.alpha=Mathf.Lerp(startAlpha, targetAlpha, time/duration);
            // ���̃t���[���܂őҋ@���A���̊ԂɃL�����Z���������������`�F�b�N
            await UniTask.Yield(PlayerLoopTiming.Update, ct);
            // �o�ߎ��Ԃ��X�V
            time += Time.deltaTime;
        }
        // �ŏI�I�ȓ����x��ݒ�
        cg.alpha = targetAlpha;
    }
}
