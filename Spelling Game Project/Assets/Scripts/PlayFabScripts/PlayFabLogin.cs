using UnityEngine;
using UnityEngine.UI;
using PlayFab;
using TMPro;
using PlayFab.ClientModels;
using System;
using System.Collections.Generic;

public class PlayFabLogin : MonoBehaviour
{
    [SerializeField]
    private string userName;

    public GameObject rowPrefab;
    public Transform rowParent;

    #region Default Unity Functions

    private void Start()
    {
        if (string.IsNullOrEmpty(PlayFabSettings.TitleId))
        {
            PlayFabSettings.TitleId = "67413";
        }
    }

    #endregion


    #region Private Methods

    private bool IsValidUserName()
    {
        bool isValid = false;

        if (userName.Length >= 3 && userName.Length <= 15)
            isValid = true;


        return isValid;
    }

    private void LoginWithCustomID()
    {
        Debug.Log($"Login to PlayFab as {userName}");

        var request = new LoginWithCustomIDRequest { CustomId = userName , CreateAccount = true };
        PlayFabClientAPI.LoginWithCustomID(request, OnLoginWithCustomIdSuccess , OnFailure);
    }

    private void UpdateDisplayName(string displayName)
    {
        Debug.Log($"Updating Playfabs's account's Display  name to : {displayName}");

        var request = new UpdateUserTitleDisplayNameRequest{ DisplayName = displayName};
        PlayFabClientAPI.UpdateUserTitleDisplayName(request,OnDisplayNameSuccess , OnFailure);
    }

    private void OnDisplayNameSuccess(UpdateUserTitleDisplayNameResult result)
    {
        Debug.Log($"display name of playfab account");
        SceneController.PlayFabLoadScene("Gameplay");
    }

    private void OnFailure(PlayFabError error)
    {
        Debug.Log($"There is an issue with your request : {error.GenerateErrorReport()}");
    }

    private void OnLoginWithCustomIdSuccess(LoginResult result)
    {
        Debug.Log($"You have into PlayFab using id : {userName}");
        UpdateDisplayName(userName);

        
    }


    private void OnLeaderBoardUpdate(UpdatePlayerStatisticsResult result)
    {
        Debug.Log("LeaderBoard Sent");
    }

    private void OnLeaderBoardGet(GetLeaderboardResult result)
    {
        foreach  (Transform item in rowParent)
        {
            Destroy(item.gameObject);
        }


        foreach (var item in result.Leaderboard)
        {
            GameObject newRow = Instantiate(rowPrefab,rowParent);

            TextMeshProUGUI[] texts = newRow.GetComponentsInChildren<TextMeshProUGUI>();
            texts[0].text = (item.Position + 1).ToString();
            texts[1].text = item.DisplayName;
            texts[2].text = item.StatValue.ToString();

            if (item.DisplayName == userName)
            {
                texts[0].color = Color.cyan;
                texts[1].color = Color.cyan;
                texts[2].color = Color.cyan;
            }

            Debug.Log(item.Position + " " + item.DisplayName + " " + item.StatValue);
        }
    }

    #endregion


    #region Public Methods

    public void SetUserName(string name)
    {
        userName = name;
        PlayerPrefs.SetString("USERNAME", userName);
    }

    public void Login()
    {
        if (!IsValidUserName())
            return;

        LoginWithCustomID();
    }

    public void SendLeaderBoard(int score)
    {
        var request = new UpdatePlayerStatisticsRequest
        {
            Statistics = new List<StatisticUpdate>
            {
                new StatisticUpdate{
                    StatisticName = "HighScore",
                    Value = score
                }
            }
        };
        PlayFabClientAPI.UpdatePlayerStatistics(request ,OnLeaderBoardUpdate ,OnFailure);
    }

    public void GetLeaderBoard()
    {
        var request = new GetLeaderboardRequest
        {
            StatisticName = "HighScore",
            StartPosition = 0,
            MaxResultsCount = 10
        };
        PlayFabClientAPI.GetLeaderboard(request , OnLeaderBoardGet ,OnFailure);
    }

    
    public void GetLeaderBoardAroundPlayer()
    {
        var request = new GetLeaderboardAroundPlayerRequest
        {
            StatisticName = "HighScore",
            MaxResultsCount = 11
        };
        PlayFabClientAPI.GetLeaderboardAroundPlayer(request,OnGetleaderBoardAroundPlayer,OnFailure);

    }

    private void OnGetleaderBoardAroundPlayer(GetLeaderboardAroundPlayerResult result)
    {
        foreach (Transform item in rowParent)
        {
            Destroy(item.gameObject);
        }


        foreach (var item in result.Leaderboard)
        {
            GameObject newRow = Instantiate(rowPrefab, rowParent);

            TextMeshProUGUI[] texts = newRow.GetComponentsInChildren<TextMeshProUGUI>();
            texts[0].text = (item.Position + 1).ToString();
            texts[1].text = item.DisplayName;
            texts[2].text = item.StatValue.ToString();


            Debug.Log(item.Position + " " + item.DisplayName + " " + item.StatValue);
        }
    }

    #endregion
}
