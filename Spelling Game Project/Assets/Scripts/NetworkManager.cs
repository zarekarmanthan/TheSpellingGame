using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Photon.Realtime;
using TMPro;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    private const int maxPlayers = 2;

    public TMP_InputField userNameInput;

    // Automaticaly syncs the scene for all other clients
    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    /// <summary>
    /// Sets the Game Version for all the players 
    ///  PhotonNetwork.NickName sets the name the of the player 
    ///  which he enters in the imputfield at the start
    ///  
    /// PhotonNetwork.ConnectUsingSettings() conects the player to the server 
    /// </summary>
    public void Connect()
    {
        PhotonNetwork.GameVersion = "0.0.1";
        PhotonNetwork.NickName = userNameInput.text;

        PhotonNetwork.ConnectUsingSettings();
    }

    

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
        Debug.LogError(PhotonNetwork.ServerAddress);
    }

    public override void OnJoinedLobby()
    {
        SceneManager.LoadScene("Lobby");
    }

   
}
