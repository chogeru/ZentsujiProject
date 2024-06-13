using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Vivox;
using UnityEngine.Analytics;
using VivoxUnity;

namespace LobbyRelaySample.vivox
{
    /// <summary>
    /// ロビーに入った後、音声チャネルを設定するための処理を行います。
    /// </summary>
    public class VivoxSetup
    {
        private bool m_hasInitialized = false;  // 初期化が完了しているかを示すフラグ
        private bool m_isMidInitialize = false;  // 初期化中であることを示すフラグ
        private ILoginSession m_loginSession = null;  // ログインセッションのインターフェース
        private IChannelSession m_channelSession = null;  // チャネルセッションのインターフェース
        private List<VivoxUserHandler> m_userHandlers;  // ユーザーハンドラのリスト

        public event Action<string> OnUserJoined;
        public event Action<string> OnUserLeft;
        /// <summary>
        /// Vivoxサービスを初期化し、音声チャネルに参加する前に呼び出します。
        /// </summary>
        /// <param name="onComplete">ログインの成否にかかわらず呼び出されるコールバック</param>
        public void Initialize(Action<bool> onComplete = null)
        {
            if (m_isMidInitialize)
                return;  // 初期化中の場合は何もしない
            m_isMidInitialize = true;  // 初期化中フラグをセット

             DebugUtility.Log("Vivoxサービスの初期化を開始します");

            if (VivoxService.Instance == null)
            {
                DebugUtility.LogError("VivoxService.Instanceがnullです");
                m_isMidInitialize = false;
                onComplete?.Invoke(false);
                return;
            }

            // Vivoxクライアントの初期化
            VivoxService.Instance.Initialize();

            // クライアントの設定（ポート範囲の設定はサポートを確認）
            var client = VivoxService.Instance.Client;
            client.AudioOutputDevices.VolumeAdjustment = 1;  // 音声出力のボリュームを調整
            client.AudioInputDevices.Muted = false;  // マイクのミュート設定を解除


            DebugUtility.Log("Vivoxサービスの初期化が完了しました");

            if (AuthenticationService.Instance == null)
            {
                DebugUtility.LogError("AuthenticationService.Instanceがnullです");
                m_isMidInitialize = false;
                onComplete?.Invoke(false); 
                return;
            }

            DebugUtility.Log("プレイヤーIDを取得します");

            Account account = new Account(AuthenticationService.Instance.PlayerId);  // プレイヤーIDからアカウントを作成

            DebugUtility.Log("アカウント作成完了: " + AuthenticationService.Instance.PlayerId);

            m_loginSession = VivoxService.Instance.Client.GetLoginSession(account);  // ログインセッションを取得

            if (m_loginSession == null)
            {
                DebugUtility.LogError("m_loginSessionがnullです");
                m_isMidInitialize = false;
                onComplete?.Invoke(false);
                return;
            }

            DebugUtility.Log("ログイントークンを取得します");

            string token = m_loginSession.GetLoginToken();  // ログイントークンを取得

            DebugUtility.Log("ログイントークン取得完了: " + token);
            m_loginSession.PropertyChanged += OnLoginSessionPropertyChanged;

            m_loginSession.BeginLogin(token, SubscriptionMode.Accept, null, null, null, result =>
            {
                try
                {
                    DebugUtility.Log("Vivoxのログイン処理を開始します");
                    m_loginSession.EndLogin(result);  // ログインを完了
                    UnityEngine.Debug.Log(m_loginSession.State);
                    CheckLoginState(onComplete);

                    if (m_loginSession.State == LoginState.LoggedIn)
                    {
                        DebugUtility.Log("Vivoxのログインが完了しました");
                        m_hasInitialized = true;
                        onComplete?.Invoke(true);
                    }
                    else
                    {
                        DebugUtility.LogWarning("Vivoxのログインが完了していません");
                        onComplete?.Invoke(false);
                    }


                }
                catch (Exception ex)
                {
                    DebugUtility.LogWarning("Vivoxのログインに失敗しました: " + ex.Message);
                    onComplete?.Invoke(false); // 初期化失敗

                }
                finally
                {
                    m_isMidInitialize = false;  // 初期化中フラグをリセット
                }
            });
        }
        private async void CheckLoginState(Action<bool> onComplete)
        {
            while (m_loginSession.State != LoginState.LoggedIn)
            {
                DebugUtility.Log("Waiting for login to complete...");
                await Task.Delay(100); // 100ms待機
            }

            DebugUtility.Log("Vivoxのログインが完了しました");
            m_hasInitialized = true;
            m_isMidInitialize = false;
            onComplete?.Invoke(true);
        }
        private void OnLoginSessionPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "State" && m_loginSession.State == LoginState.LoggedIn)
            {
                DebugUtility.Log("ログイン状態がLoggedInに変わりました");
                m_hasInitialized = true;
            }
        }
        /// <summary>
        /// ロビーに入った後、そのロビーの音声チャネルに参加します。必ずInitializeを先に完了してください。
        /// </summary>
        /// <param name="onComplete">チャネルの参加に成功したかどうかにかかわらず呼び出されるコールバック</param>
        public void JoinLobbyChannel(string lobbyId, Action<bool> onComplete)
        {
            UnityEngine.Debug.Log(m_loginSession.State);

            if (!m_hasInitialized || m_loginSession.State != LoginState.LoggedIn)
            {
                DebugUtility.LogWarning("Vivoxのログインが完了していないため、音声チャネルに参加できません。");
                onComplete?.Invoke(false);
                return;
            }

            ChannelType channelType = ChannelType.NonPositional;  // チャネルタイプを非位置情報型に設定
            Channel channel = new Channel(lobbyId + "_voice", channelType, null);  // ロビーチャネルを作成
            DebugUtility.Log(lobbyId);
            DebugUtility.Log("チャンネル"+channel+"に接続しました");
            m_channelSession = m_loginSession.GetChannelSession(channel);  // チャネルセッションを取得
            if (m_channelSession == null)
            {
                DebugUtility.LogError("m_channelSessionがnullです");
                onComplete?.Invoke(false);
                return;
            }
            string token = m_channelSession.GetConnectToken();  // 接続トークンを取得

            if (string.IsNullOrEmpty(token))
            {
                DebugUtility.LogError("接続トークンが無効です");
                onComplete?.Invoke(false);
                return;
            }
            m_channelSession.BeginConnect(true, false, true, token, result =>
            {
                try
                {
                    DebugUtility.Log("Vivoxチャネルの接続を開始します");

                    if (m_channelSession.ChannelState == ConnectionState.Disconnecting ||
                        m_channelSession.ChannelState == ConnectionState.Disconnected)
                    {
                        DebugUtility.LogWarning("Vivoxチャネルが既に切断中です。チャネル接続シーケンスを終了します。");
                        HandleEarlyDisconnect();  // 早期切断を処理
                        return;
                    }
                    DebugUtility.Log("Vivoxチャネルの接続を完了します");
                    m_channelSession.EndConnect(result);  // 接続を完了
                    DebugUtility.Log("Vivoxチャネルの接続が成功しました");
                    onComplete?.Invoke(true);  // 成功コールバックを呼び出す
                    m_channelSession.Participants.AfterKeyAdded += OnParticipantAdded;
                    m_channelSession.Participants.BeforeKeyRemoved += OnParticipantRemoved;
                    if (m_userHandlers != null && m_userHandlers.Count > 0)
                    {
                        DebugUtility.Log("ユーザーハンドラにチャネル参加を通知します");
                        foreach (VivoxUserHandler userHandler in m_userHandlers)
                        {
                            userHandler.OnChannelJoined(m_channelSession);  // 各ユーザーハンドラにチャネル参加を通知
                        }
                    }
                    else
                    {
                        DebugUtility.LogWarning("ユーザーハンドラが設定されていないか、空です");
                    }
                }
                catch (Exception ex)
                {
                    DebugUtility.LogWarning("Vivoxの接続に失敗しました: " + ex.Message);
                    onComplete?.Invoke(false);  // 失敗コールバックを呼び出す
                    m_channelSession?.Disconnect();  // チャネルを切断
                }
            });
        }

        /// <summary>
        /// ロビーを離れる際に呼び出します。
        /// </summary>
        public void LeaveLobbyChannel()
        {
            if (m_channelSession != null)
            {
                m_channelSession.Participants.AfterKeyAdded -= OnParticipantAdded;
                m_channelSession.Participants.BeforeKeyRemoved -= OnParticipantRemoved;
                if (m_channelSession.ChannelState == ConnectionState.Connecting)
                {
                    DebugUtility.LogWarning("Vivoxチャネルが接続中に切断しようとしています。接続が完了するまで待機します。");
                    HandleEarlyDisconnect();  // 早期切断を処理
                    return;
                }

                ChannelId id = m_channelSession.Channel;
                m_channelSession?.Disconnect(
                    (result) =>
                    {
                        m_loginSession.DeleteChannelSession(id);  // チャネルセッションを削除
                        m_channelSession = null;  // チャネルセッションをリセット
                    });
            }

            foreach (VivoxUserHandler userHandler in m_userHandlers)
                userHandler.OnChannelLeft();  // 各ユーザーハンドラにチャネル離脱を通知
        }

        private void HandleEarlyDisconnect()
        {
            DisconnectOnceConnected();  // 接続完了後に切断を処理
        }

        async void DisconnectOnceConnected()
        {
            while (m_channelSession?.ChannelState == ConnectionState.Connecting)
            {
                await Task.Delay(200);  // 接続中は200ms待機
                return;
            }

            LeaveLobbyChannel();  // ロビーチャネルを離脱
        }

        /// <summary>
        /// 終了時に呼び出します。これにより、開いているロビーチャネルを離れるだけでなく、Vivoxから完全に切断されます。
        /// </summary>
        public void Uninitialize()
        {
            if (!m_hasInitialized)
                return;
            m_loginSession.Logout();  // ログアウトを実行
        }
        public void SetUserHandlers(List<VivoxUserHandler> userHandlers)
        {
            m_userHandlers = userHandlers;
        }
        private void OnParticipantAdded(object sender, KeyEventArg<string> e)
        {
            string participantName = e.Key;
            DebugUtility.Log("参加者が参加しました: " + participantName);
            OnUserJoined?.Invoke(participantName);
        }

        private void OnParticipantRemoved(object sender, KeyEventArg<string> e)
        {
            string participantName = e.Key;
            DebugUtility.Log("参加者が離れました: " + participantName);
            OnUserLeft?.Invoke(participantName);
        }
    }
}