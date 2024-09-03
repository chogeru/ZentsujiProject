using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;
using Unity.Services.Vivox;
using VivoxUnity;
using AbubuResouse.Log;

public class VivoxManager : MonoBehaviour
{
    [SerializeField,Header("ロビーのID")]
    private string lobbyId = "defaultLobbyId";
    [SerializeField,Header("ユーザーハンドラのリスト")]
    private List<LobbyRelay.vivox.VivoxUserHandler> m_vivoxUserHandlers;

    private LobbyRelay.vivox.VivoxSetup m_VivoxSetup = new LobbyRelay.vivox.VivoxSetup();
    [SerializeField,Header("接続中のユーザーリスト")]
    private List<string> connectedUsers = new List<string>();


    private async void Start()
    {
        await InitializeUnityServices();
    }

    /// <summary>
    /// Unity Servicesを初期化し、サインインする
    /// </summary>
    /// <returns></returns>
    private async UniTask InitializeUnityServices()
    {
        await UnityServices.InitializeAsync();
        AuthenticationService.Instance.SignedIn += OnSignedIn;
        await AuthenticationService.Instance.SignInAnonymouslyAsync().AsUniTask();
    }

    /// <summary>
    /// サインインが完了した際に呼び出されるコールバック
    /// </summary>
    private void OnSignedIn()
    {
        Debug.Log("サインイン: " + AuthenticationService.Instance.PlayerId);
        m_VivoxSetup.SetUserHandlers(m_vivoxUserHandlers);
        m_VivoxSetup.Initialize(OnVivoxInitialized);
    }

    /// <summary>
    /// Vivoxの初期化完了時に呼び出される
    /// </summary>
    /// <param name="success">初期化成功フラグ</param>
    private void OnVivoxInitialized(bool success)
    {
        if (!success)
        {
            DebugUtility.LogWarning("Vivox初期化に失敗!");
            return;
        }

        DebugUtility.Log("Vivox初期化が成功! ロビーID: " + lobbyId);
        m_VivoxSetup.JoinLobbyChannel(lobbyId, OnLobbyChannelJoined);
    }

    /// <summary>
    /// ロビーチャネルへの接続が完了したときに呼び出される
    /// </summary>
    /// <param name="channelJoined">チャネル参加成功フラグ</param>
    private void OnLobbyChannelJoined(bool channelJoined)
    {
        if (!channelJoined)
        {
            DebugUtility.LogWarning("ロビーチャネルへの接続に失敗");
            return;
        }

        Debug.Log("ロビーチャネルへの接続が成功");
        m_VivoxSetup.OnUserJoined += HandleUserJoined;
        m_VivoxSetup.OnUserLeft += HandleUserLeft;
    }

    /// <summary>
    /// 新しいユーザーがチャネルに参加したときに呼び出される
    /// </summary>
    /// <param name="username">参加したユーザー名</param>
    private void HandleUserJoined(string username)
    {
        connectedUsers.Add(username);
        DebugUtility.Log("ユーザーが参加しました: " + username);
        DisplayConnectedUsers();
    }

    /// <summary>
    /// ユーザーがチャネルから退出したときに呼び出される
    /// </summary>
    /// <param name="username">退出したユーザー名</param>
    private void HandleUserLeft(string username)
    {
        connectedUsers.Remove(username);
        DebugUtility.Log("ユーザーが離れました: " + username);
        DisplayConnectedUsers();
    }

    /// <summary>
    /// 現在接続中の全ユーザーをログに出力
    /// </summary>
    private void DisplayConnectedUsers()
    {
        DebugUtility.Log("接続中のユーザー:");
        foreach (var user in connectedUsers)
        {
            DebugUtility.Log(user);
        }
    }

    /// <summary>
    /// オブジェクトの破棄時にイベントの登録を解除
    /// </summary>
    private void OnDestroy()
    {
        m_VivoxSetup.OnUserJoined -= HandleUserJoined;
        m_VivoxSetup.OnUserLeft -= HandleUserLeft;
    }
}
