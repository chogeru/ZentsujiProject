using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Audio;
using Cysharp.Threading.Tasks;
using R3;

public class AudioVolumeSetting : MonoBehaviour
{
    [SerializeField] 
    AudioMixer m_AudioMixer;
    [SerializeField,Header("音量設定用スライダー")]
    Slider m_BGMSlider;
    [SerializeField,Header("効果音設定用スライダー")] 
    Slider m_SESlider;

    private async void Start()
    {
        //非同期に初期化処理を行う
        await InitializeVolumeSettings();
        //https://qiita.com/sasakitaku/items/f9fb6891a907966e3052
        //BGMスライダーの値が変更されたときにSetBGMメソッドを呼び出す
        m_BGMSlider.OnValueChangedAsObservable()
            .Subscribe(SetBGM)
            .AddTo(this);
        //SEスライダーの値が変更されたときにSetSEメソッドを呼び出す
        m_BGMSlider.OnValueChangedAsObservable()
            .Subscribe(SetSE) 
            .AddTo(this);
    }
    //音量設定を初期化する
    private async UniTask InitializeVolumeSettings()
    {
        // udioMixerからBGMの音量を取得し、その値をBGMスライダーに設定
        m_AudioMixer.GetFloat("BGM", out float bgmVolume);
        m_BGMSlider.value = bgmVolume;

        //AudioMixerからSEの音量を取得し、その値をSEスライダーに設定
        m_AudioMixer.GetFloat("SE", out float seVolume);
        m_SESlider.value = seVolume;

        await UniTask.Yield();
    }
    //BGMの音量を設定するためのメソッド
    public void SetBGM(float volume)
    {
        m_AudioMixer.SetFloat("BGM", volume);
    }
    //SEの音量を設定するためのメソッド
    public void SetSE(float volume)
    {
        m_AudioMixer.SetFloat("SE", volume);
    }
}
