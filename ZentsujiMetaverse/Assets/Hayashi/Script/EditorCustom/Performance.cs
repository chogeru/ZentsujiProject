using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Profiling;
using System.Net.NetworkInformation;
using AbubuResouse.Editor;

public class Performance : MonoBehaviour
{
    static class CpuDll
    {
        [DllImport("CPU-DLL", CallingConvention = CallingConvention.StdCall)]
        public static extern int Start();

        [DllImport("CPU-DLL", CallingConvention = CallingConvention.StdCall)]
        public static extern int Stop();

        [DllImport("CPU-DLL", CallingConvention = CallingConvention.StdCall)]
        public static extern double GetCpuUsage();
    }

    public static Performance Instance;

    // GPUとCPUの情報を保持する変数
    [SerializeField,ReadOnly]
    string m_GpuInfo;
    [SerializeField, ReadOnly]
    string m_CpuInfo;
    [SerializeField, ReadOnly]
    long m_TotalReservedMemory;
    [SerializeField, ReadOnly]
    long m_TotalAllocatedMemory;
    [SerializeField, ReadOnly]
    long m_TotalUnusedReservedMemory;
    [SerializeField, ReadOnly]
    long m_MonoHeapSize;
    [SerializeField, ReadOnly]
    long m_MonoUsedSize;
    [SerializeField, ReadOnly]
    long m_UsedHeapSize;
    [SerializeField, ReadOnly]
    long m_TempAllocatorSize;
    [SerializeField, ReadOnly]
    string m_NetworkInterfaceInfo;
    GUIStyle m_LeftGuiStyle = new GUIStyle();
    GUIStyle m_RightGuiStyle = new GUIStyle();

    [SerializeField, Header("テキストカラー左")]
    private Color m_LeftGUIColor;
    [SerializeField,Header("テキストカラー右")]
    private Color m_RightGUIColor;
    float m_DeltaTime = 0.0f;

    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }
    void Start()
    {
        CpuDll.Start();

        // GPUとCPUの情報を取得
        m_GpuInfo = SystemInfo.graphicsDeviceName;
        m_CpuInfo = SystemInfo.processorType;

        // フォントサイズ
        m_LeftGuiStyle.fontSize = 35;
        m_RightGuiStyle.fontSize = 28;
        //フォントのカラー
        m_LeftGuiStyle.normal.textColor = m_LeftGUIColor;
        m_RightGuiStyle.normal.textColor = m_RightGUIColor;

        m_RightGuiStyle.alignment = TextAnchor.MiddleRight;
        DontDestroyOnLoad(this.gameObject);
    }

    private void OnApplicationQuit()
    {
        CpuDll.Stop();
    }

    void Update()
    {
        m_DeltaTime += (Time.unscaledDeltaTime - m_DeltaTime) * 0.1f;
        m_TotalReservedMemory = Profiler.GetTotalReservedMemoryLong();
        m_TotalAllocatedMemory = Profiler.GetTotalAllocatedMemoryLong();
        m_TotalUnusedReservedMemory = Profiler.GetTotalUnusedReservedMemoryLong();
        m_MonoHeapSize = Profiler.GetMonoHeapSizeLong();
        m_MonoUsedSize = Profiler.GetMonoUsedSizeLong();
        m_UsedHeapSize = Profiler.usedHeapSizeLong;
        m_TempAllocatorSize = Profiler.GetTempAllocatorSize();
        m_NetworkInterfaceInfo = GetNetworkInterfaceInfo();

    }
    string GetNetworkInterfaceInfo()
    {
        string info = "Network Interfaces:\n";

        NetworkInterface[] interfaces = NetworkInterface.GetAllNetworkInterfaces();
        foreach (NetworkInterface netInterface in interfaces)
        {
            info += "名前: " + netInterface.Name + "\n";
            info += "説明: " + netInterface.Description + "\n";
            //ネットワークインターフェースの種類
            //Ethernet：イーサネット ネットワーク インターフェイス
            //Wireless80211：ワイヤレス ネットワーク インターフェイス
            //Loopback：ループバック ネットワーク インターフェイス
            //Tunnel：トンネル ネットワーク インターフェイス
            info += "タイプ: " + netInterface.NetworkInterfaceType.ToString() + "\n";
            double speedMbps = (double)netInterface.Speed / 1000000.0; // Mbpsでの速度計算
            string speedInfo;
            if (speedMbps >= 1000)
            {
                double speedGbps = speedMbps / 1000.0; // Gbpsでの速度計算
                speedInfo = string.Format("{0:F2} Gbps", speedGbps);
            }
            else
            {
                speedInfo = string.Format("{0:F2} Mbps", speedMbps);
            }
            /*
             * ネットワークインターフェースがどのような状態にあるかを示す
             * 例えば、 Up（動作中）Down（停止中）Testing（テスト中）Unknown（不明）などの状態がある
             */
            info += "速度: " + speedInfo + "\n"; info += "状態: " + netInterface.OperationalStatus.ToString() + "\n";
        }

        return info;
    }
    void OnGUI()
    {
        // CPU使用率を表示
        GUI.Label(new Rect(10, 10, 400, 40), "CPU 使用率: " + CpuDll.GetCpuUsage().ToString("F2") + "%", m_LeftGuiStyle);

        // GPUとCPUの情報を表示
        GUI.Label(new Rect(10, 60, 400, 40), "GPU: " + m_GpuInfo, m_LeftGuiStyle);
        GUI.Label(new Rect(10, 110, 400, 40), "CPU: " + m_CpuInfo, m_LeftGuiStyle);

        GUI.Label(new Rect(10, 160, 400, 40), "総予約メモリ: " + (m_TotalReservedMemory / (1024 * 1024)).ToString() + " MB", m_LeftGuiStyle);
        GUI.Label(new Rect(10, 210, 400, 40), "総割り当てメモリ: " + (m_TotalAllocatedMemory / (1024 * 1024)).ToString() + " MB", m_LeftGuiStyle);
        GUI.Label(new Rect(10, 260, 400, 40), "未使用の総予約メモリ: " + (m_TotalUnusedReservedMemory / (1024 * 1024)).ToString() + " MB", m_LeftGuiStyle);
        GUI.Label(new Rect(10, 310, 400, 40), "Monoヒープサイズ: " + (m_MonoHeapSize / (1024 * 1024)).ToString() + " MB", m_LeftGuiStyle);
        GUI.Label(new Rect(10, 360, 400, 40), "Mono使用サイズ: " + (m_MonoUsedSize / (1024 * 1024)).ToString() + " MB", m_LeftGuiStyle);
        GUI.Label(new Rect(10, 410, 400, 40), "使用されているヒープサイズ: " + (m_UsedHeapSize / (1024 * 1024)).ToString() + " MB", m_LeftGuiStyle);
        GUI.Label(new Rect(10, 460, 400, 40), "一時割り当てサイズ: " + (m_TempAllocatorSize / (1024 * 1024)).ToString() + " MB", m_LeftGuiStyle);

        GUI.Label(new Rect(1300,440, 600, 200), m_NetworkInterfaceInfo, m_RightGuiStyle);
        // FPSを表示
        float msec = m_DeltaTime * 1000.0f;
        float fps = 1.0f / m_DeltaTime;
        string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
        GUI.Label(new Rect(10, 510, 400, 40), text, m_LeftGuiStyle);
    }
}