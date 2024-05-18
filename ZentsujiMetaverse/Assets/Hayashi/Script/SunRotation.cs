using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunRotation : MonoBehaviour
{
    public Light m_DirectionalLight;
    public float m_SunriseHour = 6f;
    public float m_SunsetHour = 18f;

    void Update()
    {
        // 東京の現在時刻を取得
        DateTime tokyoTime = GetTokyoTime();

        // 現在の時刻を時間単位で取得（例：14.5は14:30を意味する）
        float currentHour = tokyoTime.Hour + tokyoTime.Minute / 60f;

        // 日の出から日の入りまでの時間を計算
        float dayLength = m_SunsetHour - m_SunriseHour;

        // 現在の時刻が日の出と日の入りの間にある場合
        if (currentHour >= m_SunriseHour && currentHour <= m_SunsetHour)
        {
            // 日の出から現在の時刻までの経過時間を計算
            float timeSinceSunrise = currentHour - m_SunriseHour;

            // 経過時間を角度に変換（0度から180度の範囲）
            float sunAngle = (timeSinceSunrise / dayLength) * 180f;

            // ライトの回転を設定
            m_DirectionalLight.transform.rotation = Quaternion.Euler(new Vector3(sunAngle - 90f, 170f, 0f));
        }
        else
        {
            // 夜間の場合、ライトの角度を設定（例えば水平線下）
            m_DirectionalLight.transform.rotation = Quaternion.Euler(new Vector3(-90f, 170f, 0f));
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
}
