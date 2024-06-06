using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
public class SkipPerformanceManager : MonoBehaviour
{
    public static SkipPerformanceManager Instance;
    [ReadOnly,Header("スキップするかどうかのトリガー")]
    public bool isSlip=false;
    private void Awake()
    {
       if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
       else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        //シーン切り替え時のイベント追加
        SceneManager.activeSceneChanged += ResetTrigger;
    }
    public void Update()
    {
        if(Input.anyKeyDown||Gamepad.current?.buttonSouth.isPressed==true)
        {
            isSlip = true;
        }
    }

    private void ResetTrigger(Scene scene ,Scene nextScene)
    {
        isSlip=false;
    }
}
