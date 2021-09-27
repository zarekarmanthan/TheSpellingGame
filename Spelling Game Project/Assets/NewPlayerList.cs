using Photon.Realtime;
using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using ExitGames.Client.Photon;

public class NewPlayerList : MonoBehaviourPunCallbacks
{
    public TextMeshProUGUI playerInfo;
    public TextMeshProUGUI playerScoreText;
    public SpellingCheck spelling;
    public int result = 0;

    public Player NewPlayer { get; private set; }

    private void Awake()
    {
        spelling = FindObjectOfType<SpellingCheck>();
        result = spelling.score;
    }

    public void SetPlayerInfo(Player player)
    {
        NewPlayer = player;
        SetPlayerText(player);
       
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        base.OnPlayerPropertiesUpdate(targetPlayer, changedProps);

        if (targetPlayer != null && targetPlayer == NewPlayer)
        {
            Debug.Log(targetPlayer.NickName);
            if (changedProps.ContainsKey("Score"))
            {
                SetPlayerText(targetPlayer);
                Debug.Log("properties updated");
            }
        }
    }

    public void SetPlayerText(Player player)
    {
        result = spelling.score;
        if (player.CustomProperties.ContainsKey("Score"))
            result = (int)player.CustomProperties["Score"];

        playerScoreText.text = result.ToString();
        playerInfo.text = player.NickName;
    }

   /* public void UpdateScore()
    {
        playerInfo.text = $"{NewPlayer.NickName}";
        playerScoreText.text = $" {NewPlayer.GetScore()}";
    }*/
}
