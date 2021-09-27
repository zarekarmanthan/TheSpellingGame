using UnityEngine;
using System;
using TMPro;


public class UI_Invite : MonoBehaviour
{
    [SerializeField] private string friendName;
    [SerializeField] private string roomName;
    [SerializeField] private TextMeshProUGUI friendnameText;

    public static Action<UI_Invite> OnAcceptInvite = delegate { };
    public static Action<string> OnRoomInviteAccept = delegate { };
    public static Action<UI_Invite> OnDeclineInvite = delegate { };


    public void Initialize(string friend_Name , string room_Name)
    {
        friendName = friend_Name;
        roomName = room_Name;

        friendnameText.SetText(friendName);
    }

    public void AcceptInvite()
    {
        OnAcceptInvite?.Invoke(this);
        OnRoomInviteAccept?.Invoke(roomName);
    }

    public void DeclineInvite()
    {
        OnDeclineInvite?.Invoke(this);
    }
}
