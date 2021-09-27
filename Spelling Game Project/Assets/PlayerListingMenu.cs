using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerListingMenu : MonoBehaviourPunCallbacks
{
    public RectTransform content;
    public NewPlayerList playerList;
    private List<NewPlayerList> lists = new List<NewPlayerList>();



    private void Awake()
    {
            GetCurrentRoomPlayer();
            //UpdateScoreBoard();
    }


    void GetCurrentRoomPlayer()
    {
        foreach (KeyValuePair<int, Player> playerInfo in PhotonNetwork.CurrentRoom.Players)
        {
            Player_X(playerInfo.Value);
        }
    }


    public void Player_X(Player newPlayer)
    {
        //  for (int i = 0; i < PhotonNetwork.CurrentRoom.PlayerCount; i++)
        {

            NewPlayerList listing = Instantiate(playerList, content); ;
            if (listing != null)
            {
                listing.SetPlayerInfo(newPlayer);
                lists.Add(listing);
            }
        }
        Debug.LogError($"Player {newPlayer.ActorNumber} joined the room {PhotonNetwork.ServerAddress}");
    }


    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
         Player_X(newPlayer);

       /* CreateNewEntry(newPlayer);
        UpdateScoreBoard();*/
    }

   

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        int index = lists.FindIndex(x => x.NewPlayer == otherPlayer);
        if (index != -1)
        {
            Destroy(lists[index].gameObject);
            lists.RemoveAt(index);
        }
        Debug.LogError($"Player {otherPlayer.ActorNumber} left the room {PhotonNetwork.ServerAddress}");


       // UpdateScoreBoard();
    }

    #region Trial
   /* public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if (targetPlayer.IsLocal)
            if (changedProps.ContainsKey("Score"))
                UpdateScoreBoard();

    }

    private void UpdateScoreBoard()
    {
        foreach (var targetPlayer in PhotonNetwork.CurrentRoom.Players.Values)
        {
            var targetEntry = lists.Find(x => x.NewPlayer == targetPlayer);

            if (targetEntry == null)
            {
                targetEntry = CreateNewEntry(targetPlayer);
            }

            targetEntry.UpdateScore();
        }

        SortEntries();
    }

    private void SortEntries()
    {
        //sort entries in list
        lists.Sort((a, b) => b.playerScoreText.text.CompareTo(a.playerScoreText.text));

        //sort child order
        for (var i = 0; i < lists.Count; i++)
        {
            lists[i].transform.SetSiblingIndex(i);
        }
    }

    private NewPlayerList CreateNewEntry(Player newPlayer)
    {
        var newEntry = Instantiate(playerList, content);
        newEntry.SetPlayerText(newPlayer);
        lists.Add(newEntry);
        return newEntry;
    }
*/

    #endregion

}
