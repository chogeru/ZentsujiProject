using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using R3;
using VInspector;
using UnityEngine.UI;

public class Gesture : MonoBehaviour
{
    [Tab("アニメーションコンポーネント")]
    [SerializeField, Header("アニメーションコンポーネントをセット")]
    private Animation m_Animation;
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
        m_Animation = GetComponent<Animation>();
        SetUpButtonClickIvent();
    }
    private void SetUpButtonClickIvent()
    {

    }
    public void PlayAnimationClip(int index,string ClipName)
    {
        m_Animation.AddClip(m_AnimationClip[index], "Angry");
        m_Animation.Play(ClipName);
    }

}
