using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Vivox;
using AbubuResouse.Log;
using VivoxUnity;

namespace LobbyRelay.vivox
{

    /// <summary>
    /// ロビーに入った後、音声チャネルを設定するための処理
    /// </summary>
    public class VivoxSetup
    {
        private bool m_hasInitialized = false;               // 初期化が完了しているかを示すフラグ
        private bool m_isMidInitialize = false;              // 初期化中であることを示すフラグ
        private ILoginSession m_loginSession = null;         // ログインセッションのインターフェース
        private IChannelSession m_channelSession = null;     // チャネルセッションのインターフェース
        private List<VivoxUserHandler> m_userHandlers;       // ユーザーハンドラのリスト

        public event Action<string> OnUserJoined;
        public event Action<string> OnUserLeft;

        /// <summary>
        /// Vivoxサービスを初期化
        /// </summary>
        /// <param name="onComplete">ログインの成否にかかわらず呼び出されるコールバック</param>
        public void Initialize(Action<bool> onComplete)
        {
            if (m_isMidInitialize) return;

            m_isMidInitialize = true;
            DebugUtility.Log("Vivoxサービスの初期化を開始");

            if (VivoxService.Instance == null)
            {
                HandleInitializationError("VivoxService.Instanceがnull", onComplete);
                return;
            }

            VivoxService.Instance.Initialize();
            InitializeLoginSession(onComplete);
        }

        /// <summary>
        /// ログインセッションを初期化
        /// </summary>
        /// <param name="onComplete">ログイン完了コールバック</param>
        private void InitializeLoginSession(Action<bool> onComplete)
        {
            if (AuthenticationService.Instance == null)
            {
                HandleInitializationError("AuthenticationService.Instanceがnull", onComplete);
                return;
            }

            Account account = new Account(AuthenticationService.Instance.PlayerId);
            m_loginSession = VivoxService.Instance.Client.GetLoginSession(account);

            if (m_loginSession == null)
            {
                HandleInitializationError("m_loginSessionがnull", onComplete);
                return;
            }

            m_loginSession.PropertyChanged += OnLoginSessionPropertyChanged;
            string token = m_loginSession.GetLoginToken();

            m_loginSession.BeginLogin(token, SubscriptionMode.Accept, null, null, null, result =>
            {
                try
                {
                    m_loginSession.EndLogin(result);
                    CheckLoginState(onComplete);
                }
                catch (Exception ex)
                {
                    HandleLoginError("Vivoxのログインに失敗: " + ex.Message, onComplete);
                }
                finally
                {
                    m_isMidInitialize = false;
                }
            });
        }

        /// <summary>
        /// ログイン状態を確認
        /// </summary>
        /// <param name="onComplete">ログイン状態確認後コールバック</param>
        private async void CheckLoginState(Action<bool> onComplete)
        {
            while (m_loginSession.State != LoginState.LoggedIn)
            {
                await Task.Delay(100);
            }

            m_hasInitialized = true;
            onComplete?.Invoke(true);
            DebugUtility.Log("Vivoxのログインが完了");
        }

        /// <summary>
        /// 初期化エラー時の処理
        /// </summary>
        /// <param name="message">エラーメッセージ</param>
        /// <param name="onComplete">エラー処理後コールバック</param>
        private void HandleInitializationError(string message, Action<bool> onComplete)
        {
            DebugUtility.LogError(message);
            m_isMidInitialize = false;
            onComplete?.Invoke(false);
        }

        /// <summary>
        /// ログインエラー時の処理
        /// </summary>
        /// <param name="message">エラーメッセージ</param>
        /// <param name="onComplete">エラー処理後コールバック</param>
        private void HandleLoginError(string message, Action<bool> onComplete)
        {
            DebugUtility.LogWarning(message);
            onComplete?.Invoke(false);
        }

        /// <summary>
        /// ログインセッションのプロパティが変更されたときに呼び出されるイベントハンドラ
        /// </summary>
        private void OnLoginSessionPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "State" && m_loginSession.State == LoginState.LoggedIn)
            {
                DebugUtility.Log("ログイン状態がLoggedInに変わった");
                m_hasInitialized = true;
            }
        }

        /// <summary>
        /// ロビーチャネルに参加
        /// </summary>
        /// <param name="lobbyId">ロビーID</param>
        /// <param name="onComplete">チャネル参加完了コールバック</param>
        public void JoinLobbyChannel(string lobbyId, Action<bool> onComplete)
        {
            if (!m_hasInitialized || m_loginSession.State != LoginState.LoggedIn)
            {
                DebugUtility.LogWarning("Vivoxのログインが完了していないので、音声チャネルに参加できない");
                onComplete?.Invoke(false);
                return;
            }

            Channel channel = new Channel(lobbyId + "_voice", ChannelType.NonPositional, null);
            m_channelSession = m_loginSession.GetChannelSession(channel);

            if (m_channelSession == null)
            {
                DebugUtility.LogError("m_channelSessionがnull");
                onComplete?.Invoke(false);
                return;
            }

            string token = m_channelSession.GetConnectToken();
            if (string.IsNullOrEmpty(token))
            {
                DebugUtility.LogError("接続トークンが無効");
                onComplete?.Invoke(false);
                return;
            }

            m_channelSession.BeginConnect(true, false, true, token, result =>
            {
                try
                {
                    m_channelSession.EndConnect(result);
                    onComplete?.Invoke(true);
                    DebugUtility.Log("Vivoxチャネルの接続が成功しました!");
                    SubscribeToChannelEvents();
                }
                catch (Exception ex)
                {
                    HandleChannelConnectionError("Vivoxの接続に失敗: " + ex.Message, onComplete);
                }
            });
        }

        /// <summary>
        /// チャネルイベントの購読を開始
        /// </summary>
        private void SubscribeToChannelEvents()
        {
            m_channelSession.Participants.AfterKeyAdded += OnParticipantAdded;
            m_channelSession.Participants.BeforeKeyRemoved += OnParticipantRemoved;
            NotifyUserHandlersOnChannelJoined();
        }

        /// <summary>
        /// ユーザーハンドラにチャネル参加を通知
        /// </summary>
        private void NotifyUserHandlersOnChannelJoined()
        {
            if (m_userHandlers == null || m_userHandlers.Count == 0)
            {
                DebugUtility.LogWarning("ユーザーハンドラが設定されていないか、空");
                return;
            }

            foreach (VivoxUserHandler userHandler in m_userHandlers)
            {
                userHandler.OnChannelJoined(m_channelSession);
            }
        }

        /// <summary>
        /// チャネル接続エラー時の処理
        /// </summary>
        /// <param name="message">エラーメッセージ</param>
        /// <param name="onComplete">エラー処理後コールバック</param>
        private void HandleChannelConnectionError(string message, Action<bool> onComplete)
        {
            DebugUtility.LogWarning(message);
            onComplete?.Invoke(false);
            m_channelSession?.Disconnect();
        }

        /// <summary>
        /// ロビーチャネルを離脱
        /// </summary>
        public void LeaveLobbyChannel()
        {
            if (m_channelSession != null)
            {
                UnsubscribeFromChannelEvents();

                if (m_channelSession.ChannelState == ConnectionState.Connecting)
                {
                    DebugUtility.LogWarning("Vivoxチャネルが接続中に切断しようとしている。接続が完了するまで待機します");
                    HandleEarlyDisconnect();
                    return;
                }

                ChannelId id = m_channelSession.Channel;
                m_channelSession?.Disconnect(result => m_loginSession.DeleteChannelSession(id));
                m_channelSession = null;
            }

            NotifyUserHandlersOnChannelLeft();
        }

        /// <summary>
        /// チャネルイベントの購読を解除
        /// </summary>
        private void UnsubscribeFromChannelEvents()
        {
            m_channelSession.Participants.AfterKeyAdded -= OnParticipantAdded;
            m_channelSession.Participants.BeforeKeyRemoved -= OnParticipantRemoved;
        }

        /// <summary>
        /// ユーザーハンドラにチャネル離脱を通知
        /// </summary>
        private void NotifyUserHandlersOnChannelLeft()
        {
            foreach (VivoxUserHandler userHandler in m_userHandlers)
            {
                userHandler.OnChannelLeft();
            }
        }

        /// <summary>
        /// 接続が完了するまで待機し、その後にチャネルを切断
        /// </summary>
        private void HandleEarlyDisconnect()
        {
            DisconnectOnceConnected();
        }

        /// <summary>
        /// 接続が完了するまで待機し、その後に切断
        /// </summary>
        private async void DisconnectOnceConnected()
        {
            while (m_channelSession?.ChannelState == ConnectionState.Connecting)
            {
                await Task.Delay(200);
            }

            LeaveLobbyChannel();
        }

        /// <summary>
        /// Vivoxの初期化を解除
        /// </summary>
        public void Uninitialize()
        {
            if (!m_hasInitialized) return;

            m_loginSession?.Logout();
        }

        /// <summary>
        /// ユーザーハンドラを設定
        /// </summary>
        /// <param name="userHandlers">ユーザーハンドラのリスト</param>
        public void SetUserHandlers(List<VivoxUserHandler> userHandlers)
        {
            m_userHandlers = userHandlers;
        }

        /// <summary>
        /// 参加者が追加されたときに呼び出されるイベントハンドラ
        /// </summary>
        /// <param name="e">参加したユーザーのイベント引数</param>
        private void OnParticipantAdded(object sender, KeyEventArg<string> e)
        {
            OnUserJoined?.Invoke(e.Key);
            DebugUtility.Log("参加しました: " + e.Key);
        }

        /// <summary>
        /// 参加者が削除されたときに呼び出されるイベントハンドラ
        /// </summary>
        /// <param name="e">削除されたユーザーのイベント引数</param>
        private void OnParticipantRemoved(object sender, KeyEventArg<string> e)
        {
            OnUserLeft?.Invoke(e.Key);
            DebugUtility.Log("参加者が離れました: " + e.Key);
        }
    }
}