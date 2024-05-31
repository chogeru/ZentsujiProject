using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunRotation : MonoBehaviour
{
    [SerializeField,Header("回転させるオブジェクト")]
    private Light m_DirectionalLight;
    [SerializeField,Header("日の出の時間")]
    private float m_SunriseHour = 6f;
    [SerializeField,Header("日の入りの時間")]
    public float m_SunsetHour = 18f;
    // デバッグ用のフラグと時間
    private bool isDebugMode = false;
    private float debugHour = 0f;
    void Update()
    {
        // 東京の現在時刻を取得
        // 東京の現在時刻を取得
        DateTime tokyoTime = GetTokyoTime();

        // 現在の時刻を時間単位で取得（例：14.5は14:30を意味する）
        float currentHour = isDebugMode ? debugHour : tokyoTime.Hour + tokyoTime.Minute / 60f;

        // 現在の時刻が日の出と日の入りの間にある場合
        if (currentHour >= m_SunriseHour && currentHour <= m_SunsetHour)
        {
            // 昼間の場合、ライトの回転を設定
            m_DirectionalLight.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
        }
        else
        {
            // 夜間の場合、ライトの角度を設定
            m_DirectionalLight.transform.rotation = Quaternion.Euler(new Vector3(200f, 0f, 0f));
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
        debugHour = (m_SunriseHour + m_SunsetHour) / 2; 
    }
    // 夜間モードに切り替え
    [ContextMenu("夜モード")]
    public void SwitchToNightMode()
    {
        isDebugMode = true;
        // 夜間の時間に設定
        debugHour = (m_SunsetHour + 24f + m_SunriseHour) / 2 % 24f; 
    }
    // デバッグモードを解除
    [ContextMenu("Disable Debug Mode")]
    public void DisableDebugMode()
    {
        isDebugMode = false; // デバッグモードを解除
    }
}
