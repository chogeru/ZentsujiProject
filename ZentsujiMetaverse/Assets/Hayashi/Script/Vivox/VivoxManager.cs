using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;
using Unity.Services.Vivox;
using VivoxUnity;
using AbubuResouse.Log;

public class VivoxManager : MonoBehaviour
{
    LobbyRelaySample.vivox.VivoxSetup m_VivoxSetup = new LobbyRelaySample.vivox.VivoxSetup();
    [SerializeField]
    List<LobbyRelaySample.vivox.VivoxUserHandler> m_vivoxUserHandlers;
    [SerializeField]
    private string lobbyId = "defaultLobbyId";
    private List<string> connectedUsers = new List<string>();

    private async void Start()
    {
        await UnityServices.InitializeAsync(); // Unity Servicesを初期化
        AuthenticationService.Instance.SignedIn += OnSignedIn; // サインイン後のコールバックを登録
        await AuthenticationService.Instance.SignInAnonymouslyAsync(); // 匿名でサインイン
    }

    private void OnSignedIn()
    {
        Debug.Log("Signed in as: " + AuthenticationService.Instance.PlayerId);
        m_VivoxSetup.SetUserHandlers(m_vivoxUserHandlers);

        m_VivoxSetup.Initialize(success =>
        {
            if (success)
            {
                DebugUtility.Log("Vivox初期化が成功しました。ロビーID: " + lobbyId);

                m_VivoxSetup.JoinLobbyChannel(lobbyId, channelJoined =>
                {
                    if (channelJoined)
                    {
                        Debug.Log("ロビーチャネルへの接続が成功しました。");
                        m_VivoxSetup.OnUserJoined += HandleUserJoined;
                        m_VivoxSetup.OnUserLeft += HandleUserLeft;
                    }
                    else
                    {
                        DebugUtility.LogWarning("ロビーチャネルへの接続に失敗しました。");
                    }
                });
            }
            else
            {
                DebugUtility.LogWarning("Vivox初期化に失敗しました。");
            }
        });
 

    }
    private void HandleUserJoined(string username)
    {
        connectedUsers.Add(username);
        DebugUtility.Log("ユーザーが参加しました: " + username);
        DisplayConnectedUsers();
    }

    private void HandleUserLeft(string username)
    {
        connectedUsers.Remove(username);
        DebugUtility.Log("ユーザーが離れました: " + username);
        DisplayConnectedUsers();
    }

    private void DisplayConnectedUsers()
    {
        DebugUtility.Log("接続中のユーザー:");
        foreach (var user in connectedUsers)
        {
            DebugUtility.Log(user);
        }
    }

    private void OnDestroy()
    {
        m_VivoxSetup.OnUserJoined -= HandleUserJoined;
        m_VivoxSetup.OnUserLeft -= HandleUserLeft;
    }

}
