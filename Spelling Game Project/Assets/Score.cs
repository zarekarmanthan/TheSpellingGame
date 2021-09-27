using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Score : MonoBehaviour
{
    public SpellingCheck spelling;
    public PhotonView view;
    //NewPlayerList newPlayer;


    private void Update()
    {
        if (view.IsMine)
        {
            view.RPC("UpdateScore", RpcTarget.All, spelling.score);
        }
    }

    [PunRPC]
    public void UpdateScore(int playerScore)
    {
        playerScore = spelling.score;
        spelling.newPlayer.playerScoreText.text = playerScore.ToString();
        //Debug.LogWarning(score);
    }

    [PunRPC]
    public void GameOver()
    {
        if ( spelling.uiPanel.transform.GetChild(0).gameObject.activeInHierarchy)
        {
            spelling.gameOver++;
        }
    }
}
