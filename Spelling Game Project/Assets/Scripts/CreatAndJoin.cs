using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Realtime;

public class CreatAndJoin : MonoBehaviourPunCallbacks
{
    public TMP_InputField createRoom;
    public TMP_InputField joinRoom;


    public void CreateRoom()
    {
        PhotonNetwork.CreateRoom(createRoom.text);
        Debug.LogError($"Player {PhotonNetwork.LocalPlayer.NickName} created the room {PhotonNetwork.ServerAddress}");

    }

    public void JoinRoom()
    {
        PhotonNetwork.JoinRoom(joinRoom.text);
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("API_Test");

    }


}
