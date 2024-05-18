using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class WeatherManager : MonoBehaviour
{
    private string apiKey = "00dcfad56708b44e0c2968b129c4e3ee";
    private string apiURL = "http://api.openweathermap.org/data/2.5/weather?q=Kagawa,jp&appid=00dcfad56708b44e0c2968b129c4e3ee";

    void Start()
    {
        StartCoroutine(GetWeather());
    }

    IEnumerator GetWeather()
    {
        UnityWebRequest request = UnityWebRequest.Get(apiURL + apiKey);
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
            GameObject player = GameObject.FindWithTag("Player");
            player.AddComponent<RainEffect>();
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
