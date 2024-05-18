using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class WeatherManager : MonoBehaviour
{
    private string m_ApiKey = "5d77d1140fecebe5b59765e991114c7b";
    private string m_ApiURL = "http://api.openweathermap.org/data/2.5/weather?q=Kagawa,jp&appid=";

    [SerializeField, Header("雨のプレハブ")]
    private GameObject m_RainEffectPrefabs;
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
