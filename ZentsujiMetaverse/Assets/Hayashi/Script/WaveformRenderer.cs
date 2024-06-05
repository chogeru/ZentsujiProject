using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveformRenderer : MonoBehaviour
{
    public LineRenderer m_LineRenderer;
    private BGMManager m_BgmManager;
    private AudioSource m_AudioSource;
    private float[] m_Samples = new float[1024];

    void Start()
    {
        // BGMManagerのインスタンスを取得
        m_BgmManager = BGMManager.instance;
        m_AudioSource = m_BgmManager.GetComponent<AudioSource>();

        // LineRendererコンポーネントを取得
        m_LineRenderer = GetComponent<LineRenderer>();
        m_LineRenderer.positionCount = m_Samples.Length;
    }

    void Update()
    {
        if (m_AudioSource.isPlaying)
        {
            // 音声データを取得
            m_AudioSource.GetOutputData(m_Samples, 0);
            RenderWaveform();
        }
    }

    void RenderWaveform()
    {
        for (int i = 0; i < m_Samples.Length; i++)
        {
            // 波形の表示位置を設定
            float x = (float)i / m_Samples.Length;
            float y = m_Samples[i];
            m_LineRenderer.SetPosition(i, new Vector3(x, y, 0));
        }
    }
}
