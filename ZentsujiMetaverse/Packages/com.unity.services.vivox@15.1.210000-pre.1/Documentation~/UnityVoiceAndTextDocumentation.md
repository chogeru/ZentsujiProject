# Vivox Unity SDK QuickStart

Unity offers the Vivox service as a way for game developers to provide voice and text communications between users through a channel style workflow without the need to invest in a self-hosted solution.

Unity's Voice and Text chat service for multiplayer communication offers a voice chat and direct message text service with a managed hosted solution.

Plug in to your game and configure your project settings to immediately add communications to your project. Connect an unlimited number of users in 2D and 3D channels. Allow users to control voice volume, perform mute actions, and manage channels. Place users in team chat. Allow users to participate in multiple channels.

# Table of contents
- [Vivox Unity SDK QuickStart](#vivox-unity-sdk-quickstart)
- [Table of contents](#table-of-contents)
- [Enable the Vivox service in a Unity project](#enable-the-vivox-service-in-a-unity-project)
	- [Unity Dashboard setup for a Unity project](#unity-dashboard-setup-for-a-unity-project)
	- [Connect your Unity project to Vivox](#connect-your-unity-project-to-vivox)
- [Docs/Quickstart](#docsquickstart)
	- [Lifecycle actions](#lifecycle-actions)
		- [Set up/Initialize](#set-upinitialize)
		- [Sign in and join an echo channel](#sign-in-and-join-an-echo-channel)
		- [Leave a Voice and Text channel](#leave-a-voice-and-text-channel)
		- [Sign out a user](#sign-out-a-user)
		- [Uninitialize](#uninitialize)
	- [In-channel actions](#in-channel-actions)
		- [Handle participant events](#handle-participant-events)
		- [Send text messages to a channel](#send-text-messages-to-a-channel)
		- [Mute other participants](#mute-other-participants)
		- [Mute yourself](#mute-yourself)

# Enable the Vivox service in a Unity project
## Unity Dashboard setup for a Unity project
1. Create an account and project on the Unity Dashboard: https://dashboard.unity3d.com.
2. On the left pane, select **Explore Services**, and then select the **Vivox** tile. You can access this page directly at: https://dashboard.unity3d.com/vivox.
3. Follow the onboarding steps. During the onboarding process, you are provided with Vivox API credentials to use in your project.
   
## Connect your Unity project to Vivox
1. Create or open your local Unity project.
>**Note:** You can directly add required packages by editing the `manifest.json` file or by using the package manager (select **Window > Package Manager > + > Add package by name**).
2. Ensure that your Unity project's `manifest.json` file (located in your Unity project's `Packages` folder, which you can find by opening the project root with a file explorer) includes an entry for `"com.unity.services.vivox"`.
3. To streamline the process of handling player identity, add `"com.unity.services.authentication"` to the `manifest.json` file.
4. Select **Window > General > Services > General Settings** and ensure that your project has the correct project ID associated with it.
5. Select **Edit > Project Settings**. Select the **Vivox** tab, and then update **Environment** to `Automatic`. This automatically pulls Vivox credentials into your project.

For more information, see the [Developer Documentation](https://docs.vivox.com/v5/general/unity/5_15_0/en-us/Default.htm).

# Docs/Quickstart
For detailed documentation and examples, see the [Developer Documentation](https://docs.vivox.com/v5/general/unity/5_15_0/en-us/Default.htm).

  - [Lifecycle actions](#lifecycle-actions)
    - [Set up/Initialize](#set-upinitialize)
    - [Sign in and join an echo channel](#sign-in-and-join-an-echo-channel)
    - [Leave a Voice and Text channel](#leave-a-voice-and-text-channel)
    - [Sign out a user](#sign-out-a-user)
    - [Uninitialize](#uninitialize)
  - [In-channel actions](#in-channel-actions)
    - [Handle participant events](#handle-participant-events)
    - [Send text messages to a channel](#send-text-messages-to-a-channel)
    - [Mute other participants](#mute-other-participants)
    - [Mute yourself](#mute-yourself)

## Lifecycle actions

### Set up/Initialize
When using Unity Game Services, the Vivox package handles credentials and tokens for you, which simplifies the getting started process.

```csharp
using System;
using UnityEngine;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Vivox;
using VivoxUnity;

public ILoginSession LoginSession;

async void Start()
{
    await UnityServices.InitializeAsync();
    await AuthenticationService.Instance.SignInAnonymouslyAsync();

    VivoxService.Instance.Initialize();
}
```

### Sign in and join an echo channel
After you initialize the Vivox SDK, the game can sign a user in to the Vivox instance that is assigned to your game by using an `Account` object. 

>**Note:** The `Account` should have one-to-one mapping to the user. This is handled by `playerID` when using Unity Game Services.

To join a channel, create an `IChannelSession` by using a game-selected identifier for the channel and channel type, and then call its `BeginConnect()` method.

>**Note:** Before joining a Vivox voice channel on desktop or mobile, developers must check the permissions for the capture device at runtime.

```csharp
public void JoinChannel(string channelName, ChannelType channelType, bool connectAudio, bool connectText, bool transmissionSwitch = true, Channel3DProperties properties = null)
{
    if (LoginSession.State == LoginState.LoggedIn)
    {
        Channel channel = new Channel(channelName, channelType, properties);

        IChannelSession channelSession = LoginSession.GetChannelSession(channel);

        channelSession.BeginConnect(connectAudio, connectText, transmissionSwitch, channelSession.GetConnectToken(), ar => 
        {
            try
            {
                channelSession.EndConnect(ar);
            }
            catch(Exception e)
            {
                Debug.LogError($"Could not connect to channel: {e.Message}");
                return;
            }
        });
    } else 
    {
        Debug.LogError("Can't join a channel when not logged in.");
    }
} 

public void Login(string displayName = null)
{
    var account = new Account(displayName);
    bool connectAudio = true;
    bool connectText = true;

    LoginSession = VivoxService.Instance.Client.GetLoginSession(account);
    LoginSession.PropertyChanged += LoginSession_PropertyChanged;

    LoginSession.BeginLogin(LoginSession.GetLoginToken(), SubscriptionMode.Accept, null, null, null, ar =>
    {
        try
        {   
            LoginSession.EndLogin(ar);
        }
        catch (Exception e)
        {
            // Unbind any login session-related events you might be subscribed to.
            // Handle error
            return;
        }

        // At this point, we have successfully requested to login. 
        // When you are able to join channels, LoginSession.State will be set to LoginState.LoggedIn.
        // Reference LoginSession_PropertyChanged() 
    });
}

// For this example, we immediately join a channel after LoginState changes to LoginState.LoggedIn.
// In an actual game, when to join a channel will vary by implementation.
private void LoginSession_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
{
    var loginSession = (ILoginSession)sender;
    if (e.PropertyName == "State")
    {
        if (loginSession.State == LoginState.LoggedIn)
        {
            bool connectAudio = true;
            bool connectText = true;

            // This puts you into an echo channel where you can hear yourself speaking.
            // If you can hear yourself, then everything is working and you are ready to integrate Vivox into your project.
            JoinChannel("TestChannel", ChannelType.Echo, connectAudio, connectText);
            // To test with multiple users, try joining a non-positional channel.
            // JoinChannel("MultipleUserTestChannel", ChannelType.NonPositional, connectAudio, connectText);
        }
    }
}
```

For more information, see the [Developer Documentation](https://docs.vivox.com/v5/general/unity/5_15_0/en-us/Default.htm#Unity/developer-guide/channels/join-channel.htm).

### Leave a Voice and Text channel
To remove a user from a channel, call the channel’s IChannelSession.Disconnect method. The disconnected channel object remains in the ILoginSession.ChannelSessions list after disconnection.

Optionally, you can use the ILoginSession.DeleteChannelSession method to remove the disconnected channel object from the list; it is safe to do so immediately after disconnecting.

* If the session is completely disconnected, the object is immediately removed from the list.
* If the session is still in the process of disconnecting, the object is removed when the disconnection process is complete.

**Note:** Disconnection is not an immediate operation and can take several seconds to complete.

For more information, see the [Developer Documentation](https://docs.vivox.com/v5/general/unity/5_15_0/en-us/Default.htm#Unity/developer-guide/channels/leave-channel.htm).

### Sign out a user
When the game no longer wants the user signed in, it calls the `ILoginSession.Logout` method. After the user is signed out, there is no network traffic to or received by the Vivox SDK. This should not be a frequent operation and is typically only called when quitting the application, or in the scenario where a user can sign in to different accounts within the app, when signing the user out of the game server.

```csharp
void LogOut()
{
    // For this example, _loginSession is assumed to be an ILoginSession.
    LoginSession.Logout();
}
```

For more information, see the [Developer Documentation](https://docs.vivox.com/v5/general/unity/5_15_0/en-us/Default.htm#Unity/developer-guide/vivox-unity-sdk-basics/sign-out-of-game.htm).

### Uninitialize
In most situations, Vivox automatically handles unintialization for you. However, if necessary, you can manually perform uninitialization.

```csharp
VivoxService.Instance.Client.Uninitialize();
```

For more information, see the [Developer Documentation](https://docs.vivox.com/v5/general/unity/5_15_0/en-us/Default.htm#Unity/developer-guide/vivox-unity-sdk-basics/uninitialize-sdk.htm).

## In-channel actions

### Handle participant events
The Vivox SDK posts information about individual participants in a channel that is visible to all other participants. This includes the following information:

* When a user joins a channel
* When a user leaves a channel
* When there is an important change in user state, such as whether the user is speaking or typing

Handling participant events is optional. If there is no visualization of user state (for example, showing who has voice enabled), then the game can ignore these events.

To provide a visualization of user state information, a game must handle the following messages:

* `IChannelSession.Participants.AfterKeyAdded`
* `IChannelSession.Participants.BeforeKeyRemoved`
* `IChannelSession.Participants.AfterValueUpdated`

```csharp
private void BindChannelSessionHandlers(bool doBind, IChannelSession channelSession)
{
    //Subscribing to the events
    if (doBind)
    {
        // Participants
        channelSession.Participants.AfterKeyAdded += OnParticipantAdded;
        channelSession.Participants.BeforeKeyRemoved += OnParticipantRemoved;
        channelSession.Participants.AfterValueUpdated += OnParticipantValueUpdated;

        //Messaging
        channelSession.MessageLog.AfterItemAdded += OnChannelMessageReceived;
    }

    //Unsubscribing to the events
    else
    {
        // Participants
        channelSession.Participants.AfterKeyAdded -= OnParticipantAdded;
        channelSession.Participants.BeforeKeyRemoved -= OnParticipantRemoved;
        channelSession.Participants.AfterValueUpdated -= OnParticipantValueUpdated;

        //Messaging
        channelSession.MessageLog.AfterItemAdded -= OnChannelMessageReceived;
    }
}
```

For more information, see the [Developer Documentation](https://docs.vivox.com/v5/general/unity/5_15_0/en-us/Default.htm#Unity/developer-guide/channels/participant-events.htm).

### Send text messages to a channel
If the game has joined a channel with text enabled, users can send and receive group text messages. To send messages, the game uses the `IChannelSession.BeginSendText` method. After this SDK call is made, other users in that channel receive an `IChannelSession.MessageLog.AfterItemAdded` event.

```csharp
// For this example, _channelSession is a connected IChannelSession and _accountId is the local user’s AccountId.
// Sending
void SendGroupMessage()
{
    var channelName = _channelSession.Channel.Name;
    var senderName = _accountId.Name;
    var message = "Hello World!";

    _channelSession.BeginSendText(message, ar =>
    {
        try
        {
            _channelSession.EndSendText(ar);
        }
        catch (Exception e)
        {
            // Handle error
            return;
        }
        Debug.Log(channelName + ": " + senderName + ": " + message);
    });
}
// Receiving
_channelSession.MessageLog.AfterItemAdded += onChannelMessageReceived;
void onChannelMessageReceived(object sender, QueueItemAddedEventArgs<IChannelTextMessage> queueItemAddedEventArgs)
{
    var channelName = queueItemAddedEventArgs.Value.ChannelSession.Channel.Name;
    var senderName = queueItemAddedEventArgs.Value.Sender.Name;
    var message = queueItemAddedEventArgs.Value.Message;

    Debug.Log(channelName + ": " + senderName + ": " + message);
}
```

For more information, see the [Developer Documentation](https://docs.vivox.com/v5/general/unity/5_15_0/en-us/Default.htm#Unity/developer-guide/messaging/group-text-messages.htm).

### Mute other participants
You can mute other users for a specific user by using a local mute. The LocalMute function prevents a user from hearing a muted user, but other users in the channel continue to hear the muted user.

For example, Cynthia, Fernando, and Wang are in a channel. Fernando does not want to hear Wang’s audio. By using a local mute, you can allow Fernando to stop hearing Wang’s audio. However, Cynthia continues to hear Wang, and Wang continues to hear Cynthia and Fernando.

```csharp
public void SetParticipantMuted(IParticipant participant, Bool setMute)
{
    if (participant.InAudio)
    {
        participant.LocalMute =setMute
    }
    else
    {
        //Tell Player To Try Again
        Debug.Log("Try Again");
    }
}
```

For more information, see the [Developer Documentation](https://docs.vivox.com/v5/general/unity/5_15_0/en-us/Default.htm#Unity/developer-guide/muting/mute-other-users.htm).

### Mute yourself
There are two methods for muting a user’s microphone: `AudioInputDevices` or `SetTransmissionMode()`. Each method is beneficial for different scenarios.

Use can also use `Transmission.None` to prevent a player from being heard in any channels. For example:

```csharp
ILoginSession.SetTransmissionMode(TransmissionMode.None);
```

For more information, see the [Developer Documentation](https://docs.vivox.com/v5/general/unity/5_15_0/en-us/Default.htm#Unity/developer-guide/muting/mute-user-microphone.htm).
