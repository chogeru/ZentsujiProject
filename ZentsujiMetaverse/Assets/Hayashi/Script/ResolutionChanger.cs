using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ResolutionChanger : MonoBehaviour
{

    public Dropdown m_ResolutionDropdown;

    void Start()
    {
        // 利用可能な解像度を取得してドロップダウンに追加
        m_ResolutionDropdown.ClearOptions(); // 既存のオプションをクリア
        List<string> options = new List<string>();
        options.Add("フルスクリーン"); // フルスクリーンオプションの追加

        // 利用可能なすべての解像度を取得してオプションに追加
        foreach (Resolution res in Screen.resolutions)
        {
            string option = res.width + "x" + res.height;
            if (!options.Contains(option)) // 重複を避ける
            {
                options.Add(option);
            }
        }

        m_ResolutionDropdown.AddOptions(options); // ドロップダウンにオプションを追加
        m_ResolutionDropdown.onValueChanged.AddListener(delegate { ChangeResolution(m_ResolutionDropdown.value); });
    }

    void ChangeResolution(int index)
    {
        if (index == 0)
        {
            // フルスクリーンモード
            Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, true);
        }
        else
        {
            // オプションから解像度をパースして設定
            string[] dimensions = m_ResolutionDropdown.options[index].text.Split('x');
            int width = int.Parse(dimensions[0]);
            int height = int.Parse(dimensions[1]);
            Screen.SetResolution(width, height, false);
        }
    }
}
