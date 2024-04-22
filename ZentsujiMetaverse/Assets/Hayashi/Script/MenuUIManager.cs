using UnityEngine;
using UnityEngine.UI;
using R3;
using System.Collections.Generic;
using VInspector;
using Mirror.Examples.CouchCoop;

public class MenuUIManager : MonoBehaviour
{
    [Tab("ボタン")]
    [SerializeField, Header("オプションボタン")]
    private Button m_OptionsButton;
    [SerializeField, Header("音量設定ボタン")]
    private Button m_VolumeButton;
    [SerializeField, Header("チャット画面ボタン")]
    private Button m_ChatButton;
    [SerializeField, Header("画面サイズ変更画面ボタン")]
    private Button m_ScreenSizeButton;

    [Tab("各設定画面")]
    [SerializeField, Header("オプション画面")]
    private GameObject m_OptionsPanel;
    [SerializeField, Header("音量設定画面")]
    private GameObject m_VolumePanel;
    [SerializeField, Header("チャット画面")]
    private GameObject m_ChatPanel;
    [SerializeField, Header("画面サイズ変更画面")]
    private GameObject m_ScreenSizePanel;
    [EndTab]

    private Dictionary<Button, GameObject> m_ButtonPanelMap;

    public static MenuUIManager instance;

    public bool OpenUI=false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        InitializeButtonPanelMap();
        CloseAllPanels();
        SetupButtonCallbacks();
      
    }
    private void InitializeButtonPanelMap()
    {
        m_ButtonPanelMap = new Dictionary<Button, GameObject>
        {
            { m_OptionsButton, m_OptionsPanel },
            { m_VolumeButton, m_VolumePanel },
            { m_ChatButton, m_ChatPanel },
            { m_ScreenSizeButton, m_ScreenSizePanel }
        };
    }
    private void CloseAllPanels()
    {
        foreach (var panel in m_ButtonPanelMap.Values)
        {
            panel.SetActive(false);
        }
    }
    private void SetupButtonCallbacks()
    {
        foreach (var pair in m_ButtonPanelMap)
        {
            pair.Key.OnClickAsObservable()
                .Subscribe(_ => ShowOnlyThisPanel(pair.Value))
                .AddTo(this);
        }
    }
    private void ShowOnlyThisPanel(GameObject activePanel)
    {
        Cursor.visible = true;
        // 全パネルを非表示にして、選択されたパネルのみを表示
        foreach (var panel in m_ButtonPanelMap.Values)
        {
            if (panel == m_OptionsPanel)
            {
                panel.SetActive(true);
            }
            else
            {
                // 選択されたパネルのみを表示し、他は非表示に設定
                panel.SetActive(panel == activePanel);
            }
        }
    }
}
