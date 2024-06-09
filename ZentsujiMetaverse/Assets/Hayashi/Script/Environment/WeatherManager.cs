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

    public event Action<string> OnWeatherChanged;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }

    void Start()
    {
        StartCoroutine(GetWeather());
    }

    IEnumerator GetWeather()
    {
        UnityWebRequest request = UnityWebRequest.Get(m_ApiURL + m_ApiKey);
        yield return request.SendWebRequest();

        if (request.isNetworkError || request.isHttpError)
        {
            Debug.LogError(request.error);
        }
        else
        {
            ProcessWeatherData(request.downloadHandler.text);
        }
    }

    void ProcessWeatherData(string json)
    {
        WeatherInfo weatherInfo = JsonUtility.FromJson<WeatherInfo>(json);
        Debug.Log("現在の天気: " + weatherInfo.weather[0].main);
        OnWeatherChanged?.Invoke(weatherInfo.weather[0].main);

        if (weatherInfo.weather[0].main == "Rain")
        {
            if (m_RainEffectPrefabs != null)
            {
                m_RainEffectPrefabs.SetActive(true);
            }
            else
            {
                Debug.Log("雨のプレハブが設定されてないよ");
            }
            if(m_SkyMaterial!=null)
            {
                m_SkyMaterial.SetFloat("Cloudiness", 0.8f);
                m_SkyMaterial.SetFloat("CloudFalloff", 0f);

            }
        }
        if (m_SkyMaterial != null)
        {
            m_SkyMaterial.SetFloat("Cloudiness", 0.4f);
            m_SkyMaterial.SetFloat("CloudFalloff", 0.4f);
        }

        if (m_RainEffectPrefabs != null)
        {
            m_RainEffectPrefabs.SetActive(false);
        }
        else
        {
            Debug.Log("プレハブが設定されていない");
        }

    }
    [ContextMenu("天気通常=Clouds")]
    public void WeatherCloud()
    {
        if (m_RainEffectPrefabs != null)
        {
            m_RainEffectPrefabs.SetActive(false);
            m_SkyMaterial.SetFloat("_Cloudiness", 0.4f);
            m_SkyMaterial.SetFloat("_CloudFalloff", 0.4f);
        }
        else
        {
            Debug.Log("プレハブが設定されていない");
        }
    }

    [ContextMenu("天気雨=Rain")]
    public void WeathwerRain()
    {
        if (m_RainEffectPrefabs != null)
        {
            m_RainEffectPrefabs.SetActive(true);
            m_SkyMaterial.SetFloat("_Cloudiness", 0.8f);
            m_SkyMaterial.SetFloat("_CloudFalloff", 0f);
        }
        else
        {
            Debug.Log("プレハブが設定されていない");
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
