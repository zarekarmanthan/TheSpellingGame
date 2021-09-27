using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_DisplayInvites : MonoBehaviour
{
    [SerializeField] private Transform inviteContainer;
    [SerializeField] private UI_Invite uiInvitePrefab;
    [SerializeField] private RectTransform contentRect;
    [SerializeField] private Vector2 originalSize;
    [SerializeField] private Vector2 increaseSize;

    private List<UI_Invite> inviteList;

    private void Awake()
    {
        inviteList = new List<UI_Invite>();
        contentRect = inviteContainer.GetComponent<RectTransform>();
        originalSize = contentRect.sizeDelta;
        increaseSize = new Vector2(0,uiInvitePrefab.GetComponent<RectTransform>().sizeDelta.y);
        PhotonChatController.OnRoomInvite += HandleRoomInvite;

        UI_Invite.OnAcceptInvite += HandleInviteAccept;
        UI_Invite.OnDeclineInvite += HandleInviteDecline;
    }

    private void OnDestroy()
    {
        PhotonChatController.OnRoomInvite -= HandleRoomInvite;
        UI_Invite.OnAcceptInvite -= HandleInviteAccept;
        UI_Invite.OnDeclineInvite -= HandleInviteDecline;
    }

    private void HandleInviteDecline(UI_Invite invite)
    {
        if (inviteList.Contains(invite))
        {
            inviteList.Remove(invite);
            Destroy(invite.gameObject);
        }
    }

    private void HandleInviteAccept(UI_Invite invite)
    {
        if (inviteList.Contains(invite))
        {
            inviteList.Remove(invite);
            Destroy(invite.gameObject);
        }

    }

    private void HandleRoomInvite(string friend, string room)
    {
        UI_Invite uiInvite = Instantiate(uiInvitePrefab,inviteContainer);
        uiInvite.Initialize(friend,room);
        contentRect.sizeDelta += increaseSize;
        inviteList.Add(uiInvite);
    }
}
