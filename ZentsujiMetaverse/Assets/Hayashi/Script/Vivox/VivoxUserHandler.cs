using UnityEngine;
using Unity.Services.Vivox;
using VivoxUnity;

namespace LobbyRelaySample.vivox
{
    /// <summary>
    /// ロビー内の1ユーザーのVivox状態の変化を監視します。
    /// Relayを通じてではなく、既にすべてのクライアントの状態変化を伝えるVivoxサービスを介して監視します。
    /// </summary>
    public class VivoxUserHandler : MonoBehaviour
    {
        [SerializeField]
        private UI.LobbyUserVolumeUI m_lobbyUserVolumeUI;  // ユーザー音量UIの参照。Inspectorで設定する

        private IChannelSession m_channelSession;  // 現在のチャネルセッション
        private string m_id;  // ユーザーID
        private string m_vivoxId;  // Vivoxで使用されるID

        private const int k_volumeMin = -50, k_volumeMax = 20;  // Vivoxのドキュメントに基づく音量の有効範囲

        /// <summary>
        /// 標準化された音量のデフォルト値を取得します。
        /// </summary>
        public static float NormalizedVolumeDefault
        {
            get { return (0f - k_volumeMin) / (k_volumeMax - k_volumeMin); }  // 音量を0から1の範囲に正規化したデフォルト値を計算
        }

        /// <summary>
        /// スクリプトの初期化時に呼び出されます。
        /// </summary>
        public void Start()
        {
            if (m_lobbyUserVolumeUI != null)
            {
                m_lobbyUserVolumeUI.DisableVoice(true);  // 音声UIを無効に設定
            }
        }

        /// <summary>
        /// ユーザーIDを設定します。
        /// </summary>
        /// <param name="id">設定するユーザーID</param>
        public void SetaId(string id)
        {
            m_id = id;  // ユーザーIDを設定

            m_vivoxId = null;  // Vivox IDをリセット

            // 既にチャネルセッションが存在する場合、参加者リストをチェックしてIDを設定
            if (m_channelSession != null)
            {
                foreach (var participant in m_channelSession.Participants)
                {
                    if (m_id == participant.Account.DisplayName)
                    {
                        m_vivoxId = participant.Key;  // Vivox IDを設定
                        if (m_lobbyUserVolumeUI != null)
                        {
                            m_lobbyUserVolumeUI.IsLocalPlayer = participant.IsSelf;  // ローカルプレイヤーであるかを設定
                            m_lobbyUserVolumeUI.EnableVoice(true);  // 音声UIを有効に設定
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// チャネルに参加した際に呼び出されます。これはロビーに参加すると開始されます。
        /// </summary>
        /// <param name="channelSession">参加したチャネルセッション</param>
        public void OnChannelJoined(IChannelSession channelSession)
        {
            m_channelSession = channelSession;  // チャネルセッションを設定
            m_channelSession.Participants.AfterKeyAdded += OnParticipantAdded;  // 参加者が追加された際のイベントハンドラを登録
            m_channelSession.Participants.BeforeKeyRemoved += BeforeParticipantRemoved;  // 参加者が削除される前のイベントハンドラを登録
            m_channelSession.Participants.AfterValueUpdated += OnParticipantValueUpdated;  // 参加者の値が更新された際のイベントハンドラを登録
        }

        /// <summary>
        /// チャネルを離脱した際に呼び出されます。
        /// </summary>
        public void OnChannelLeft()
        {
            if (m_channelSession != null)  // チャネルセッションが存在する場合
            {
                m_channelSession.Participants.AfterKeyAdded -= OnParticipantAdded;  // イベントハンドラを解除
                m_channelSession.Participants.BeforeKeyRemoved -= BeforeParticipantRemoved;  // イベントハンドラを解除
                m_channelSession.Participants.AfterValueUpdated -= OnParticipantValueUpdated;  // イベントハンドラを解除
                m_channelSession = null;  // チャネルセッションをリセット
            }
        }

        /// <summary>
        /// 新しい参加者がチャネルに追加されるたびに呼び出されます。これはVivoxのカスタム辞書のイベントを使用します。
        /// </summary>
        /// <param name="sender">イベントの送信元</param>
        /// <param name="keyEventArg">キーイベントの引数</param>
        private void OnParticipantAdded(object sender, KeyEventArg<string> keyEventArg)
        {
            var source = (VivoxUnity.IReadOnlyDictionary<string, IParticipant>)sender;  // 送信元の辞書を取得
            var participant = source[keyEventArg.Key];  // 追加された参加者を取得
            var username = participant.Account.DisplayName;  // 参加者の表示名を取得

            bool isThisUser = username == m_id;  // 参加者がこのユーザーかどうかを確認
            if (isThisUser)
            {
                m_vivoxId = keyEventArg.Key;  // Vivox IDを設定
                m_lobbyUserVolumeUI.IsLocalPlayer = participant.IsSelf;  // ローカルプレイヤーかどうかを設定

                if (!participant.IsMutedForAll)
                    m_lobbyUserVolumeUI.EnableVoice(false);  // 全体ミュートされていない場合、音声を有効にする
                else
                    m_lobbyUserVolumeUI.DisableVoice(false);  // 全体ミュートされている場合、音声を無効にする
            }
            else
            {
                if (!participant.LocalMute)
                    m_lobbyUserVolumeUI.EnableVoice(false);  // ローカルミュートされていない場合、音声を有効にする
                else
                    m_lobbyUserVolumeUI.DisableVoice(false);  // ローカルミュートされている場合、音声を無効にする
            }
        }

        /// <summary>
        /// 参加者が削除される前に呼び出されます。
        /// </summary>
        /// <param name="sender">イベントの送信元</param>
        /// <param name="keyEventArg">キーイベントの引数</param>
        private void BeforeParticipantRemoved(object sender, KeyEventArg<string> keyEventArg)
        {
            var source = (VivoxUnity.IReadOnlyDictionary<string, IParticipant>)sender;  // 送信元の辞書を取得
            var participant = source[keyEventArg.Key];  // 削除される参加者を取得
            var username = participant.Account.DisplayName;  // 参加者の表示名を取得

            bool isThisUser = username == m_id;  // 参加者がこのユーザーかどうかを確認
            if (isThisUser)
            {
                m_lobbyUserVolumeUI.DisableVoice(true);  // このユーザーの場合、音声UIを無効にする
            }
        }

        /// <summary>
        /// 参加者の値が更新された際に呼び出されます。
        /// </summary>
        /// <param name="sender">イベントの送信元</param>
        /// <param name="valueEventArg">値イベントの引数</param>
        private void OnParticipantValueUpdated(object sender, ValueEventArg<string, IParticipant> valueEventArg)
        {
            var source = (VivoxUnity.IReadOnlyDictionary<string, IParticipant>)sender;  // 送信元の辞書を取得
            var participant = source[valueEventArg.Key];  // 更新された参加者を取得
            var username = participant.Account.DisplayName;  // 参加者の表示名を取得
            string property = valueEventArg.PropertyName;  // 更新されたプロパティ名を取得

            if (username == m_id)  // 参加者がこのユーザーかどうかを確認
            {
                if (property == "UnavailableCaptureDevice")  // キャプチャデバイスが利用不可になった場合
                {
                    if (participant.UnavailableCaptureDevice)
                    {
                        m_lobbyUserVolumeUI.DisableVoice(false);  // 音声UIを無効にする
                        participant.SetIsMuteForAll(true, null);  // 全体ミュートに設定
                    }
                    else
                    {
                        m_lobbyUserVolumeUI.EnableVoice(false);  // 音声UIを有効にする
                        participant.SetIsMuteForAll(false, null);  // 全体ミュートを解除
                    }
                }
                else if (property == "IsMutedForAll")  // 全体ミュート状態が変更された場合
                {
                    if (participant.IsMutedForAll)
                        m_lobbyUserVolumeUI.DisableVoice(false);  // 全体ミュートの場合、音声UIを無効にする
                    else
                        m_lobbyUserVolumeUI.EnableVoice(false);  // 全体ミュートが解除された場合、音声UIを有効にする
                }
            }
        }

        /// <summary>
        /// 音量スライダーが操作された際に呼び出されます。
        /// </summary>
        /// <param name="volumeNormalized">正規化された音量値</param>
        public void OnVolumeSlide(float volumeNormalized)
        {
            if (m_channelSession == null || m_vivoxId == null)  // チャネルセッションまたはVivox IDが未設定の場合、処理を中断
                return;

            int vol = (int)Mathf.Clamp(k_volumeMin + (k_volumeMax - k_volumeMin) * volumeNormalized, k_volumeMin, k_volumeMax);  // 音量を適切な範囲内に制限
            bool isSelf = m_channelSession.Participants[m_vivoxId].IsSelf;  // ローカルプレイヤーかどうかを確認
            if (isSelf)
            {
                VivoxService.Instance.Client.AudioInputDevices.VolumeAdjustment = vol;  // ローカルプレイヤーの場合、入力デバイスの音量を調整
            }
            else
            {
                m_channelSession.Participants[m_vivoxId].LocalVolumeAdjustment = vol;  // 他のプレイヤーの場合、ローカル音量を調整
            }
        }

        /// <summary>
        /// ミュートトグルが操作された際に呼び出されます。
        /// </summary>
        /// <param name="isMuted">ミュート状態</param>
        public void OnMuteToggle(bool isMuted)
        {
            if (m_channelSession == null || m_vivoxId == null)  // チャネルセッションまたはVivox IDが未設定の場合、処理を中断
                return;

            bool isSelf = m_channelSession.Participants[m_vivoxId].IsSelf;  // ローカルプレイヤーかどうかを確認
            if (isSelf)
            {
                VivoxService.Instance.Client.AudioInputDevices.Muted = isMuted;  // ローカルプレイヤーの場合、入力デバイスをミュート
            }
            else
            {
                m_channelSession.Participants[m_vivoxId].LocalMute = isMuted;  // 他のプレイヤーの場合、ローカルミュートを設定
            }
        }
    }
}
