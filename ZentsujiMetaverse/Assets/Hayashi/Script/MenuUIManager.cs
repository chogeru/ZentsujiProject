using UnityEngine;
using UnityEngine.UI;
using R3;
using System.Collections.Generic;
using VInspector;
using Mirror.Examples.CouchCoop;
using System.Linq;
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

    [Tab("オプション画面のアニメーション")]
    [SerializeField, Header("アニメ-ター")]
    private Animator m_Animator;
    [EndTab]

    private Dictionary<Button, GameObject> m_ButtonPanelMap;
    private Dictionary<GameObject, bool> m_PanelOpenStatus;

    public static MenuUIManager instance;

    public bool isOpenUI = false;

    public bool IsUIOpen()
    {
        return m_PanelOpenStatus.Values.Any(status => status);
    }

    private void Awake()
    {
        m_PanelOpenStatus = new Dictionary<GameObject, bool>();
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
        foreach (var pair in m_ButtonPanelMap)
        {
            m_PanelOpenStatus[pair.Value] = false;
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ShowOnlyThisPanel(m_OptionsPanel);
        }
        if (Input.GetKeyDown(KeyCode.Escape) && isOpenUI == true)
        {
            CloseAllPanels();
        }
        CorsorControl();
        UpdateAnimatorState();
        isOpenUI = IsUIOpen();
    }
    private void UpdateAnimatorState()
    {
        foreach (var pair in m_PanelOpenStatus.Keys.ToList())
        {
            Animator animator = pair.GetComponent<Animator>();
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            if (stateInfo.IsName("Close") && stateInfo.normalizedTime >= 1.0f)
            {
                m_PanelOpenStatus[pair] = false;  // アニメーションが完全に終了したら状態を更新
            }
            else if (stateInfo.IsName("Open") && stateInfo.normalizedTime >= 1.0f)
            {
                m_PanelOpenStatus[pair] = true;  // アニメーションが完全に終了したら状態を更新
            }
        }
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
        foreach (var pair in m_ButtonPanelMap)
        {
            GameObject panel = pair.Value;
            Animator animator = panel.GetComponent<Animator>();
            animator.Play("Close");
            m_PanelOpenStatus[panel] = false;
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

        foreach (var pair in m_ButtonPanelMap)
        {
            GameObject panel = pair.Value;
            Animator animator = panel.GetComponent<Animator>();  // 各パネルにアサインされたAnimatorを取得
            bool isActive = panel == activePanel;

            // アニメーターでアニメーションを再生
            if (isActive)
            {
                animator.Play("Open");
                m_PanelOpenStatus[panel] = true;
            }
            else if (panel != m_OptionsPanel && panel.activeInHierarchy) // パネルがすでに表示されていれば
            {
                animator.Play("Close");
                m_PanelOpenStatus[panel] = false;

            }
        }
    }



    private void CorsorControl()
    {
        Cursor.visible = isOpenUI;
    }
}