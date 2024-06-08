using UnityEngine;
using UnityEngine.UI;
using R3;
using System.Collections.Generic;
using VInspector;
using Mirror.Examples.CouchCoop;
using System.Linq;
using UnityEngine.InputSystem;
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

    private Dictionary<Button, GameObject> m_ButtonPanelMap;
    private Dictionary<GameObject, bool> m_PanelOpenStatus;
    private Dictionary<GameObject, CanvasGroup> m_PanelCanvasGroups;

    public static MenuUIManager instance;
    [ReadOnly]
    public bool isOpenUI = false;
    private bool isKeyOrButtonPressed = false;
    public bool IsUIOpen()
    {
        return m_PanelOpenStatus.Values.Any(status => status);
    }

    private void Awake()
    {
        m_PanelOpenStatus = new Dictionary<GameObject, bool>();
        m_PanelCanvasGroups = new Dictionary<GameObject, CanvasGroup>();

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
        InitializeCanvasGroups();
        CloseAllPanels();
        SetupButtonCallbacks();
        foreach (var pair in m_ButtonPanelMap)
        {
            m_PanelOpenStatus[pair.Value] = false;
        }
    }

    private void Update()
    {
        if ((Input.GetKeyDown(KeyCode.Escape) || Gamepad.current?.startButton.isPressed == true) && !isKeyOrButtonPressed)
        {
            isKeyOrButtonPressed = true;
            if (isOpenUI)
            {
                CloseAllPanels();
            }
            else
            {
                ShowOnlyThisPanel(m_OptionsPanel);
            }
        }
        if (Input.GetKeyUp(KeyCode.Escape) || (Gamepad.current?.startButton.wasReleasedThisFrame == true))
        {
            isKeyOrButtonPressed = false;
        }
        isOpenUI = IsUIOpen();
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

    private void InitializeCanvasGroups()
    {
        foreach (var pair in m_ButtonPanelMap)
        {
            CanvasGroup canvasGroup = pair.Value.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = pair.Value.AddComponent<CanvasGroup>();
            }
            m_PanelCanvasGroups[pair.Value] = canvasGroup;
        }
    }

    private void CloseAllPanels()
    {
        foreach (var pair in m_ButtonPanelMap)
        {
            GameObject panel = pair.Value;
            CanvasGroup canvasGroup = m_PanelCanvasGroups[panel];
            canvasGroup.alpha = 0;
            canvasGroup.blocksRaycasts = false;
            m_PanelOpenStatus[panel] = false;
        }
    }

    private void SetupButtonCallbacks()
    {
        foreach (var pair in m_ButtonPanelMap)
        {
            pair.Key.onClick.AddListener(() => ShowOnlyThisPanel(pair.Value));
        }
    }

    private void ShowOnlyThisPanel(GameObject activePanel)
    {
        foreach (var pair in m_ButtonPanelMap)
        {
            GameObject panel = pair.Value;
            CanvasGroup canvasGroup = m_PanelCanvasGroups[panel];
            bool isActive = panel == activePanel;

            if (isActive)
            {
                canvasGroup.alpha = 1;
                canvasGroup.blocksRaycasts = true;
                m_PanelOpenStatus[panel] = true;
            }
            else
            {
                canvasGroup.alpha = 0;
                canvasGroup.blocksRaycasts = false;
                m_PanelOpenStatus[panel] = false;
            }
        }
    }
}