using UnityEngine;
using R3;
using Cysharp.Threading.Tasks;
using System;

public class LoginScreenManager : MonoBehaviour
{
    [SerializeField,Header("LoginCanvas��LogoFadeEffect �R���|�[�l���g�̎Q��")]
    private LogoFadeEffect m_LogoFadeEffect;
    [SerializeField,Header("���O�C����ʂ𐧌䂷�邽�߂�CanvasGroup")]
    private CanvasGroup m_LoginCanvasGroup;
    [SerializeField,Header("�t�F�[�h�C���Ɋ|���鎞��")]
    private float m_FadeInDuration = 3.0f;

    private void Start()
    {
        //null�`�F�b�N
        if (m_LogoFadeEffect != null)
        {
            // LogoFadeEffect �� OnFadeOutCompleted �C�x���g�ɑ΂��čw�ǂ��܂��B
            // R3���g�p���ăC�x���g�����������ۂ� StartLoginFadeIn ���\�b�h���Ăяo���܂��B
            // AddTo(this) �́A���� MonoBehaviour ���j�����ꂽ�Ƃ��ɍw�ǂ������I�ɉ������܂��B
            m_LogoFadeEffect.OnFadeOutCompleted.Subscribe(_ => StartLoginFadeIn()).AddTo(this);
        }
    }

    private async void StartLoginFadeIn()
    {
        Debug.Log("���O�C����ʂ̃t�F�[�h�C�����J�n���܂��B");
        // CanvasGroup�̓����x��񓯊��I�ɕs�����ɕύX�B
        await FadeCanvasGroup(m_LoginCanvasGroup, 1f, m_FadeInDuration);
    }

    private async UniTask FadeCanvasGroup(CanvasGroup cg, float targetAlpha, float duration)
    {
        // �t�F�[�h�����J�n���̓����x���L�^�B
        float startAlpha = cg.alpha;
        float time = 0;
        // �o�ߎ��Ԃ��ݒ肳�ꂽ�������ԂɒB����܂Ń��[�v�𑱂���B
        while (time < duration)
        {
            // Mathf.Lerp ���g�p���āA���݂̓����x����ڕW�̓����x�ւƏ��X�ɕω�������B
            cg.alpha = Mathf.Lerp(startAlpha, targetAlpha, time / duration);
            // UniTask.Yield �Ŏ��̃t���[���܂őҋ@�B
            // PlayerLoopTiming.Update ���w�肵�āA���t���[���̍X�V�^�C�~���O�ŏ������ĊJ�B
            await UniTask.Yield(PlayerLoopTiming.Update);

            time += Time.deltaTime;
        }
        cg.alpha = targetAlpha;
    }
}
