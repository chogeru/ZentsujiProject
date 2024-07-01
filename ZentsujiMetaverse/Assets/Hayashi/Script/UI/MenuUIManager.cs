using UnityEngine;
using UnityEngine.UI;
using R3;
using System.Collections.Generic;
using VInspector;
using Mirror.Examples.CouchCoop;
using System.Linq;
using UnityEngine.InputSystem;
using Cysharp.Threading.Tasks;
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
        foreach (var pair in m_ButtonPanelMap)
        {
            GameObject panel = pair.Value;
            CanvasGroup canvasGroup = m_PanelCanvasGroups[panel];

            // CanvasGroupのalphaが0より大きい場合、パネルは開いていると見なす
            if (canvasGroup.alpha > 0)
            {
                return true;
            }
        }

        return false;
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
        CloseAllPanels().Forget();
        SetupButtonCallbacks();
        foreach (var pair in m_ButtonPanelMap)
        {
            m_PanelOpenStatus[pair.Value] = false;
        }
    }

    private void Update()
    {
        m_OptionsButton.gameObject.SetActive(!IsUIOpen());

        if ((Input.GetKeyDown(KeyCode.Escape) || Gamepad.current?.startButton.isPressed == true) && !isKeyOrButtonPressed)
        {
            isKeyOrButtonPressed = true;
            if (isOpenUI)
            {
                CloseAllPanels().Forget();
            }
            else
            {
                ShowOnlyThisPanel(m_OptionsPanel).Forget();
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

    private async UniTask CloseAllPanels()
    {
        foreach (var pair in m_ButtonPanelMap)
        {
            GameObject panel = pair.Value;
            CanvasGroup canvasGroup = m_PanelCanvasGroups[panel];
            await FadeCanvasGroup(canvasGroup, 0, 0.15f);
            canvasGroup.blocksRaycasts = false;
            m_PanelOpenStatus[panel] = false;
        }
    }

    private void SetupButtonCallbacks()
    {
        foreach (var pair in m_ButtonPanelMap)
        {
            pair.Key.onClick.AddListener(() => ShowOnlyThisPanel(pair.Value).Forget());
        }
    }

    private async UniTask ShowOnlyThisPanel(GameObject activePanel)
    {
        foreach (var pair in m_ButtonPanelMap)
        {
            GameObject panel = pair.Value;
            CanvasGroup canvasGroup = m_PanelCanvasGroups[panel];
            bool isActive = panel == activePanel;

            if (isActive)
            {
                await FadeCanvasGroup(canvasGroup, 1, 0.2f);
                canvasGroup.blocksRaycasts = true;
                m_PanelOpenStatus[panel] = true;
            }
            else
            {
                await FadeCanvasGroup(canvasGroup, 0, 0f);
                canvasGroup.blocksRaycasts = false;
                m_PanelOpenStatus[panel] = false;
            }
        }
    }

    private async UniTask FadeCanvasGroup(CanvasGroup canvasGroup,float targetAlpha, float duration)
    {
        float startAlpha = canvasGroup.alpha;
        float time = 0;

        while (time < duration)
        {
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, time/duration);
            time += Time.deltaTime;
            await UniTask.Yield(PlayerLoopTiming.Update);
        }
        canvasGroup.alpha = targetAlpha;
    }
}