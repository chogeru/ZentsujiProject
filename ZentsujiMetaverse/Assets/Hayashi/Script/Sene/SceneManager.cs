using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SQLite4Unity3d;
using Cysharp.Threading.Tasks;
using System;
using AbubuResouse.Log;
using UnityEngine.UI;
using TMPro;
using Mirror;

namespace AbubuResouse.Singleton
{
    /// <summary>
    /// シーン管理を行うクラス
    /// </summary>
    public class SceneManager : NetworkBehaviour
    {
        public static SceneManager Instance;

        private string m_NextScene;
        private bool isSceneLoading = false;

        [SerializeField, Header("ロード時のキャンバス")]
        private GameObject m_LoadingCanvas;
        [SerializeField, Header("ロード進捗バー")]
        private Image m_LoadingBar;
        [SerializeField, Header("ロード時に差し替える画像")]
        private Image m_LoadingSprite;
        [SerializeField, Header("TipsのText")]
        private TMP_Text m_LoadingDescription;
        [SerializeField, Header("Mapの名前Text")]
        private TMP_Text m_MapNameText;

        [Header("オフラインシーンのリスト")]
        public List<SceneLoadData> offlineScenes;
        [Header("オンラインシーンのリスト")]
        public List<SceneLoadData> onlineScenes;

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

            if (m_LoadingCanvas != null)
            {
                m_LoadingCanvas.SetActive(false);
            }
        }

        /// <summary>
        /// 次のシーンの読み込み用
        /// </summary>
        /// <param name="currentSceneName">現在のシーン名</param>
        public void TriggerSceneLoad(string currentSceneName)
        {
            LoadNextSceneAsync(currentSceneName);
        }

        /// <summary>
        /// 次のシーンを非同期で読み込む
        /// </summary>
        /// <param name="currentSceneName">現在のシーン名</param>
        private async void LoadNextSceneAsync(string currentSceneName)
        {
            if (isSceneLoading || NetworkManager.singleton.isNetworkActive)
            {
                return;
            }

            isSceneLoading = true;

            // 次のシーン名を非同期で取得
            m_NextScene = GetNextSceneName(currentSceneName);

            if (!string.IsNullOrEmpty(m_NextScene))
            {
                UpdateLoadingScreen(m_NextScene);
                if (m_LoadingSprite != null)
                {
                    m_LoadingCanvas.SetActive(true);
                }

                // オンラインシーンの場合、NetworkManagerのonlineSceneに設定し、ホストかクライアントで開始
                if (IsOnlineScene(m_NextScene))
                {
                    NetworkManager.singleton.onlineScene = m_NextScene;

                    // ホストまたはクライアントのどちらかを開始
                    if (NetworkServer.active || NetworkClient.active)
                    {
                        // すでにホストまたはクライアントとしてアクティブな場合は、シーンを直接ロード
                        NetworkManager.singleton.ServerChangeScene(m_NextScene);
                    }
                    else
                    {
                        if (NetworkServer.active)
                        {
                            // すでにホストとしてアクティブならシーンを直接ロード
                            NetworkManager.singleton.ServerChangeScene(m_NextScene);
                        }
                        else if (NetworkClient.active)
                        {
                            // すでにクライアントとしてアクティブならそのまま続行
                            NetworkManager.singleton.ServerChangeScene(m_NextScene);
                        }
                        else
                        {
                            // 他のホストがオンラインに存在するかをチェック
                            if (NetworkManager.singleton.networkAddress != "" && NetworkClient.isConnected)
                            {
                                // クライアントとして参加
                                NetworkManager.singleton.StartClient();
                            }
                            else
                            {
                                // クライアントとして参加できない場合はホストとして開始
                                NetworkManager.singleton.StartHost();
                            }
                        }
                    }
                }
                else
                {
                    // オフラインシーンの場合、通常のシーンロード処理
                    await SceneTransitionAsync(m_NextScene);
                }

                if (m_LoadingCanvas != null)
                {
                    m_LoadingCanvas.SetActive(false);
                }
            }
            else
            {
                Debug.LogError("次のシーン名が設定されていない");
            }
            isSceneLoading = false;
        }

        /// <summary>
        /// 次のシーン名をリストから取得する
        /// </summary>
        /// <param name="currentSceneName">現在のシーン名</param>
        /// <returns>次のシーン名</returns>
        private string GetNextSceneName(string currentSceneName)
        {
            SceneLoadData sceneData = null;
            if (IsOnlineScene(currentSceneName))
            {
                sceneData = onlineScenes.Find(data => data.sceneName == currentSceneName);
            }
            else
            {
                sceneData = offlineScenes.Find(data => data.sceneName == currentSceneName);
            }

            return sceneData?.sceneName;
        }

        /// <summary>
        /// オンラインシーンかどうかを判定する
        /// </summary>
        /// <param name="sceneName">シーン名</param>
        /// <returns>オンラインシーンの場合はtrue、それ以外はfalse</returns>
        private bool IsOnlineScene(string sceneName)
        {
            return onlineScenes.Exists(data => data.sceneName == sceneName);
        }

        /// <summary>
        /// ロード画面を更新する
        /// </summary>
        /// <param name="sceneName">シーン名</param>
        private void UpdateLoadingScreen(string sceneName)
        {
            var sceneData = offlineScenes.Find(data => data.sceneName == sceneName)
                            ?? onlineScenes.Find(data => data.sceneName == sceneName);

            if (sceneData != null)
            {
                if (m_LoadingSprite != null && m_LoadingDescription != null && m_MapNameText != null)
                {
                    m_LoadingSprite.sprite = sceneData.loadingSprite;
                    m_LoadingDescription.text = sceneData.loadingDescription;
                    m_MapNameText.text = sceneData.mapName;
                }
            }
        }

        /// <summary>
        /// シーン読み込み用関数
        /// </summary>
        /// <param name="sceneName">シーン名</param>
        /// <returns></returns>
        public async UniTask SceneTransitionAsync(string sceneName)
        {
            if (m_LoadingBar != null)
            {
                m_LoadingBar.fillAmount = 0;
            }
            var asyncOperation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName);
            while (!asyncOperation.isDone)
            {
                if (m_LoadingBar != null)
                {
                    m_LoadingBar.fillAmount = asyncOperation.progress;
                }
                await UniTask.Yield();
            }

            if (m_LoadingBar != null)
            {
                m_LoadingBar.fillAmount = 1f;
            }
        }

        public void ExitGame()
        {
            Application.Quit();
        }

        [Serializable]
        public class SceneLoadData
        {
            public string sceneName;
            public Sprite loadingSprite;
            [TextArea(3, 10)]
            public string loadingDescription;
            public string mapName;
        }
    }
}
