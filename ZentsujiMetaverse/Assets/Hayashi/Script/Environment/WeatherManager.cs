using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;

public class WeatherManager : MonoBehaviour
{
    public static WeatherManager instance;

    private string m_ApiKey = "5d77d1140fecebe5b59765e991114c7b";
    private string m_ApiURL = "http://api.openweathermap.org/data/2.5/weather?q=Kagawa,jp&appid=";

    [SerializeField, Header("雨のプレハブ")]
    private GameObject m_RainEffectPrefabs;
    [SerializeField,Header("スカイボックスマテリアル")]
    private Material m_SkyMaterial;
    //天気が変更されたときのイベント
    public event Action<string> OnWeatherChanged;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }
    //開始時に天気情報を所得
    async void Start()
    {
       await GetWeather();
    }
    //天気情報を所得する非同期関数
    private async UniTask GetWeather()
    {
        UnityWebRequest request = UnityWebRequest.Get(m_ApiURL + m_ApiKey);
        await request.SendWebRequest();

        //ネットワークエラー等のチェック
        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            DebugUtility.LogError(request.error);
        }
        else
        {
            //所得した天気の情報の処理
            ProcessWeatherData(request.downloadHandler.text);
        }
    }

    private void ProcessWeatherData(string json)
    {
        //デシリアライズ
        WeatherInfo weatherInfo = JsonUtility.FromJson<WeatherInfo>(json);
        //天気情報のログ
        DebugUtility.Log("現在の天気: " + weatherInfo.weather[0].main);
        //天気変更
        OnWeatherChanged?.Invoke(weatherInfo.weather[0].main);

        if (weatherInfo.weather[0].main == "Rain")
        {
            if (m_RainEffectPrefabs != null)
            {
                //雨のエフェクトをアクティブに
                m_RainEffectPrefabs.SetActive(true);
            }
            else
            {
                DebugUtility.Log("雨のプレハブが設定されてないよ");
            }
            if(m_SkyMaterial!=null)
            {
                //スカイボックスの雲のシェーダーを曇り調整
                m_SkyMaterial.SetFloat("Cloudiness", 0.8f);
                m_SkyMaterial.SetFloat("CloudFalloff", 0f);

            }
        }
        if (m_SkyMaterial != null)
        {               
            //スカイボックスの雲のシェーダーを通常(晴れ)に調整
            m_SkyMaterial.SetFloat("Cloudiness", 0.4f);
            m_SkyMaterial.SetFloat("CloudFalloff", 0.4f);
        }

        if (m_RainEffectPrefabs != null)
        {
            m_RainEffectPrefabs.SetActive(false);
        }
        else
        {
            DebugUtility.Log("プレハブが設定されていない");
        }

    }
    [ContextMenu("天気通常=Clouds")]
    public void WeatherCloud()
    {
        if (m_RainEffectPrefabs != null)
        {
            //雨のプレハブの無効化とシェーダーを晴れに
            m_RainEffectPrefabs.SetActive(false);
            m_SkyMaterial.SetFloat("_Cloudiness", 0.4f);
            m_SkyMaterial.SetFloat("_CloudFalloff", 0.4f);
        }
        else
        {
            DebugUtility.Log("プレハブが設定されていない");
        }
    }

    [ContextMenu("天気雨=Rain")]
    public void WeathwerRain()
    {
        if (m_RainEffectPrefabs != null)
        {
            //雨のプレハブのアクティブ化とシェーダーを曇り
            m_RainEffectPrefabs.SetActive(true);
            m_SkyMaterial.SetFloat("_Cloudiness", 0.8f);
            m_SkyMaterial.SetFloat("_CloudFalloff", 0f);
        }
        else
        {
            DebugUtility.Log("プレハブが設定されていない");
        }
    }

}

[System.Serializable]
public class Weather
{
    public string main;
}

[System.Serializable]
public class WeatherInfo
{
    public Weather[] weather;
}
