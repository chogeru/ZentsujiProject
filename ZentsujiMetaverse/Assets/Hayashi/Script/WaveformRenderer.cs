using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveformRenderer : MonoBehaviour
{
    [SerializeField]
    private LineRenderer m_LineRenderer;
    private BGMManager m_BgmManager;
    private AudioSource m_AudioSource;
    //オーディオデータ
    private float[] m_Samples = new float[1024];

    void Start()
    {
        // BGMManagerのインスタンスを取得
        m_BgmManager = BGMManager.instance;
        m_AudioSource = m_BgmManager.GetComponent<AudioSource>();

        // LineRendererコンポーネントを取得
        m_LineRenderer = GetComponent<LineRenderer>();
        //ラインレンダラーの調点数をサンプル配列の長さに
        m_LineRenderer.positionCount = m_Samples.Length;
    }

    void Update()
    {
        if (m_AudioSource.isPlaying)
        {
            //オーディオデータを所得し、サンプル配列に格納
            m_AudioSource.GetOutputData(m_Samples, 0);
            //サンプル配列をもとに波形を描画
            RenderWaveform();
        }
    }

    void RenderWaveform()
    {
        //サンプル配列のデータを使って波形を描画
        for (int i = 0; i < m_Samples.Length; i++)
        {
            //X座標をサンプルのインデックスに基づいて計算
            float x = (float)i / m_Samples.Length;
            //Y座標をサンプルの値に基づいて計算
            float y = m_Samples[i];
            //ラインレンダラーに頂点を設定
            m_LineRenderer.SetPosition(i, new Vector3(x, y, 0));
        }
    }
}
