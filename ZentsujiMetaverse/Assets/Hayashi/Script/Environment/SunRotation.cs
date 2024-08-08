using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using VInspector;
using AbubuResouse.Log;

public class SunRotation : MonoBehaviour
{
    #region ライトオブジェクト
    [Tab("ライトオブジェクト")]
    [SerializeField,Header("回転させるオブジェクト")]
    private Light m_DirectionalLight;
    [EndTab]
    #endregion

    #region 時間設定
    [Tab("時間設定")]
    [SerializeField,Header("日の出の時間")]
    private float m_SunriseHour = 6f;
    [SerializeField,Header("日の入りの時間")]
    public float m_SunsetHour = 18f;
    [EndTab]
    #endregion

    #region オブジェクト設定
    [Tab("時間経過時のオブジェクト")]
    [SerializeField, Header("昼に設置するオブジェクト")]
    private GameObject m_DaytimeObjects;
    [SerializeField, Header("夜に表示するオブジェクト")]
    private GameObject m_NightObjects;
    [EndTab]
    #endregion

    #region フラグ
    // デバッグ用のフラグと時間
    private bool isDebugMode = false;
    private float isDebugHour = 0f;
    #endregion
    private void Start()
    {
        if (m_DaytimeObjects != null && m_NightObjects != null)
        {
            m_DaytimeObjects.SetActive(false);
            m_NightObjects.SetActive(false);
        }
    }
    void Update()
    {
        // 東京の現在時刻を取得
        // 東京の現在時刻を取得
        DateTime tokyoTime = GetTokyoTime();

        // 現在の時刻を時間単位で取得（例：14.5は14:30を意味する）
        float currentHour = isDebugMode ? isDebugHour : tokyoTime.Hour + tokyoTime.Minute / 60f;

        // 現在の時刻が日の出と日の入りの間にある場合
        if (currentHour >= m_SunriseHour && currentHour <= m_SunsetHour)
        {
            // 昼間の場合、ライトの回転を設定
            m_DirectionalLight.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
            if(m_DaytimeObjects != null&&m_NightObjects != null)
            {
                m_DaytimeObjects.SetActive(true);
                m_NightObjects.SetActive(false);
            }
        }
        else
        {
            // 夜間の場合、ライトの角度を設定
            m_DirectionalLight.transform.rotation = Quaternion.Euler(new Vector3(200f, 0f, 0f));
            if (m_DaytimeObjects != null && m_NightObjects != null)
            {
                m_DaytimeObjects.SetActive(false);
                m_NightObjects.SetActive(true);
            }
        }
        if(m_DaytimeObjects == null||m_NightObjects ==null)
        {
            DebugUtility.Log("昼夜のオブジェクトが設定されていない");
        }
    }

    DateTime GetTokyoTime()
    {
        // UTC現在時刻を取得
        DateTime utcNow = DateTime.UtcNow;

        // 東京のタイムゾーン（UTC+9）に変換
        TimeZoneInfo tokyoTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Tokyo Standard Time");
        DateTime tokyoTime = TimeZoneInfo.ConvertTimeFromUtc(utcNow, tokyoTimeZone);

        return tokyoTime;
    }

    ///////デバック用//////

    //昼間に切り替え
    [ContextMenu("昼モード")]
    public void SwitchToDayMode()
    {
        isDebugMode = true;
        // 昼間の時間に設定
        isDebugHour = (m_SunriseHour + m_SunsetHour) / 2;
        m_DaytimeObjects.SetActive(true);
        m_NightObjects.SetActive(false);
    }
    // 夜間モードに切り替え
    [ContextMenu("夜モード")]
    public void SwitchToNightMode()
    {
        isDebugMode = true;
        // 夜間の時間に設定
        isDebugHour = (m_SunsetHour + 24f + m_SunriseHour) / 2 % 24f;
        m_DaytimeObjects.SetActive(false);
        m_NightObjects.SetActive(true);
    }
    // デバッグモードを解除
    [ContextMenu("Disable Debug Mode")]
    public void DisableDebugMode()
    {
        isDebugMode = false; // デバッグモードを解除
    }
}
