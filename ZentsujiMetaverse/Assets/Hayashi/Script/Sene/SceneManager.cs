using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SQLite4Unity3d;
using Cysharp.Threading.Tasks;
using System;
using AbubuResouse.Log;
using UnityEngine.UI;
using TMPro;

namespace AbubuResouse.Singleton
{
    /// <summary>
    /// シーン管理を行うクラス
    /// </summary>

    public class SceneManager : SingletonMonoBehaviour<SceneManager>
    {
        private SQLiteConnection m_Connection;
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
        [SerializeField, Header("各シーンのデータ")]
        private List<SceneLoadData> m_SceneLoadDataList;

        protected override void Awake()
        {
            base.Awake();
            var databasePath = System.IO.Path.Combine(Application.streamingAssetsPath, "scene_data.db").Replace("\\", "/");
            m_Connection = new SQLiteConnection(databasePath, SQLiteOpenFlags.ReadOnly);
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
            if (isSceneLoading)
            {
                DebugUtility.LogWarning("シーンがすでにロード中");
                return;
            }
            isSceneLoading = true;
            // 次のシーン名を非同期で取得
            m_NextScene = await GetNextSceneNameFromDBAsync(currentSceneName);

            if (!string.IsNullOrEmpty(m_NextScene))
            {
                UpdateLoadingScreen(m_NextScene);
                if (m_LoadingSprite != null)
                {
                    m_LoadingCanvas.SetActive(true);
                }
                await SceneTransitionAsync(m_NextScene);
                if (m_LoadingCanvas != null)
                {
                    m_LoadingCanvas.SetActive(false);
                }
            }
            else
            {
                DebugUtility.LogError("次のシーン名が設定されていない");
            }
            isSceneLoading = false;
        }

        /// <summary>
        /// データベースから次のシーン名を非同期で取得する
        /// </summary>
        /// <param name="currentSceneName">現在のシーン名</param>
        /// <returns>次のシーン名</returns>
        private async UniTask<string> GetNextSceneNameFromDBAsync(string currentSceneName)
        {
            try
            {
                return await UniTask.RunOnThreadPool(() =>
                {
                    var query = m_Connection.Table<SceneTransition>()
                                           .Where(x => x.CurrentScene == currentSceneName)
                                           .FirstOrDefault();
                    return query?.NextScene;
                });
            }
            catch (System.Exception ex)
            {
                DebugUtility.LogError(ex.Message);
            }

            return null;
        }

        /// <summary>
        /// ロード画面を更新する
        /// </summary>
        /// <param name="sceneName">シーン名</param>
        private void UpdateLoadingScreen(string sceneName)
        {
            var sceneData = m_SceneLoadDataList.Find(data => data.sceneName == sceneName);
            if (sceneData != null)
            {
                if (m_LoadingSprite != null && m_LoadingDescription != null && m_MapNameText != null)
                    m_LoadingSprite.sprite = sceneData.loadingSprite;
                m_LoadingDescription.text = sceneData.loadingDescription;
                m_MapNameText.text = sceneData.mapName;
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
            // シーンの非同期読み込みを開始
            // シーンの読み込みが完了するまで待機
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

        /// <summary>
        /// ゲームを終了する。
        /// </summary>
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

        class SceneTransition
        {
            [PrimaryKey, AutoIncrement]
            public int Id { get; set; }
            public string CurrentScene { get; set; }
            public string NextScene { get; set; }
        }
    }
}
