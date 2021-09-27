using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Linq;
using PlayfabFriendInfo = PlayFab.ClientModels.FriendInfo;
using PhotonFriendInfo = Photon.Realtime.FriendInfo;

public class PhotonFriendController : MonoBehaviourPunCallbacks
{

    public static Action<List<PhotonFriendInfo>> OnDisplayFriends = delegate { };

    private void Awake()
    {
        PlayFabFriendController.OnFriendListUpdated += HanndleFriendsUpdated;
    }

    private void OnDestroy()
    {
        PlayFabFriendController.OnFriendListUpdated -= HanndleFriendsUpdated;
    }

    private void HanndleFriendsUpdated(List<PlayfabFriendInfo> friends)
    {
        if (friends.Count != 0)
        {
            string[] friendsDisplayNames = friends.Select(f => f.TitleDisplayName).ToArray();
            PhotonNetwork.FindFriends(friendsDisplayNames);
        }
        else
        {
            List<PhotonFriendInfo> friendList = new List<PhotonFriendInfo>();
            OnDisplayFriends?.Invoke(friendList);
        }
    }

    public override void OnFriendListUpdate(List<PhotonFriendInfo> friendList)
    {
        OnDisplayFriends?.Invoke(friendList);
    }

}
