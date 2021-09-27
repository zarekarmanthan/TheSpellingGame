using Facebook.Unity;
using PlayFab;
using PlayFab.ClientModels;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using LoginResult = PlayFab.ClientModels.LoginResult;
using System;
using System.Collections;

public class PlayFabAuthService : MonoBehaviour
{
    // holds the latest message to be displayed on the screen
    private string _message;
    [SerializeField]
    private string playerName;
    public TextMeshProUGUI loginText;

    private const string LogInKey = "FbLoginKey"; 
    private const string RememberMeID = "PlayFabIDKey"; 

    #region Default Functions

    private void Awake()
    {
       // PlayerPrefs.DeleteAll();

        if (!FB.IsInitialized)
        {
            FB.Init(() =>
            {
                if (FB.IsInitialized)
                    FB.ActivateApp();
                else
                    Debug.LogError("Could Not Start The App");

            },
            isGameShown =>
            {
                if (!isGameShown)
                    Time.timeScale = 0;
                else
                    Time.timeScale = 1;
            });
        }
        else
            FB.ActivateApp();
    }


    private void Start()
    {
        PlayerPrefs.GetString(playerName);
      //  Debug.LogWarning(playerName);
        StartCoroutine(FB_AutoLogin());
    }

    #endregion


    #region Public Methods

    public void FacebookLogin()
    {
       loginText.text =  "Initializing Facebook..."; // logs the given message and displays it on the screen using OnGUI method

        // This call is required before any other calls to the Facebook API. We pass in the callback to be invoked once initialization is finished
        // FB.Init(OnFacebookInitialized);
        OnFacebookInitialized();
    }

    public void SetMessage(string message, bool error = false)
    {
        _message = message;
        if (error)
            Debug.LogError(_message);
        else
            Debug.Log(_message);
    }

    public void OnGUI()
    {
        var style = new GUIStyle { fontSize = 40, normal = new GUIStyleState { textColor = Color.white }, alignment = TextAnchor.MiddleCenter, wordWrap = true };
        var area = new Rect(0, 0, Screen.width, Screen.height);
        GUI.Label(area, _message, style);
    }

    public void SetUserName(string name)
    {
        playerName = name;
        PlayerPrefs.SetString("USERNAME", playerName);
        PlayerPrefs.Save();
    }

    public bool RememberMe
    {
        get
        {
            return PlayerPrefs.GetInt(LogInKey, 0) == 0 ? false : true;
        }
        set
        {
            PlayerPrefs.SetInt(LogInKey, value ? 1 : 0);
        }
    }

    #endregion


    #region Private Methods


    private string RememberMePlayFabID
    {
        get
        {
            return PlayerPrefs.GetString(RememberMeID,"");
        }
        set
        {
            var token = AccessToken.CurrentAccessToken;
            PlayerPrefs.SetString(RememberMeID,token.TokenString);
        }
    }

    private void OnFacebookInitialized()
    {
        loginText.text =("Logging into Facebook...");

        // Once Facebook SDK is initialized, if we are logged in, we log out to demonstrate the entire authentication cycle.
       /* if (FB.IsLoggedIn)
            FB.LogOut();*/

        // We invoke basic login procedure and pass in the callback to process the result
        var perms = new List<string>() { "public_profile", "user_friends" };
        FB.LogInWithReadPermissions(perms, OnFacebookLoggedIn);
    }

    private void OnFacebookLoggedIn(ILoginResult result)
    {
        // If result has no errors, it means we have authenticated in Facebook successfully
        if (FB.IsLoggedIn && result == null || string.IsNullOrEmpty(result.Error))
        {
            var aToken = Facebook.Unity.AccessToken.CurrentAccessToken;

            loginText.text = ("Facebook Auth Complete! Access Token: " + AccessToken.CurrentAccessToken.TokenString + "\nLogging into PlayFab...");

            /*
             * We proceed with making a call to PlayFab API. We pass in current Facebook AccessToken and let it create
             * and account using CreateAccount flag set to true. We also pass the callback for Success and Failure results
             */
            PlayFabClientAPI.LoginWithFacebook(new LoginWithFacebookRequest { CreateAccount = true, AccessToken = aToken.TokenString },
                OnPlayfabFacebookAuthComplete, OnPlayfabFacebookAuthFailed);

            
        }
        else
        {
            // If Facebook authentication failed, we stop the cycle with the message
            SetMessage("Facebook Auth Failed: " + result.Error + "\n" + result.RawResult, true);
        }
    }

    // When processing both results, we just set the message, explaining what's going on.
    private void OnPlayfabFacebookAuthComplete(LoginResult result)
    {
        loginText.text = "PlayFab Facebook Auth Complete. Session ticket: " + result.SessionTicket;

        // SceneController.PlayFabLoadScene("Multiplayer");
        //UpdateDisplayName();

        if (RememberMe)
        {
            RememberMePlayFabID = AccessToken.CurrentAccessToken.TokenString;

            PlayFabClientAPI.LinkFacebookAccount(new LinkFacebookAccountRequest
            {
                AccessToken = RememberMePlayFabID
            }, null, null);
        }

        SceneController.PlayFabLoadScene("Gameplay"); 
    }

    private void OnPlayfabFacebookAuthFailed(PlayFabError error)
    {
        SetMessage("PlayFab Facebook Auth Failed: " + error.GenerateErrorReport(), true);
    }


    private void LoginStatusCallback(ILoginStatusResult result)
    {
        if (!string.IsNullOrEmpty(result.Error))
        {
            Debug.Log("Error: " + result.Error);
        }
        else if (result.Failed)
        {
            Debug.Log("Failure: Access Token could not be retrieved");
        }
        else
        {
            // Successfully logged user in
            // A popup notification will appear that says "Logged in as <User Name>"
            Debug.Log("Success: " + result.AccessToken.UserId);
        }

        SceneController.PlayFabLoadScene("Gameplay");
    }

  /*  private void UpdateDisplayName()
    {

        Debug.Log($"Updating Playfabs's account's Display  name to : {playerName}");

        var request = new UpdateUserTitleDisplayNameRequest { DisplayName = playerName };
        PlayFabClientAPI.UpdateUserTitleDisplayName(request, OnDisplayNameSuccess, OnFailure);
        
    }*/

    private void GetFacebookData(IGraphResult result)
    {
        playerName = result.ResultDictionary["name"].ToString();

        Debug.Log("fbName: " + playerName);
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

    #endregion


    IEnumerator FB_AutoLogin()
    {
        yield return new WaitForSeconds(1f);

        if (RememberMe && !string.IsNullOrEmpty(RememberMePlayFabID))
        {
            PlayerPrefs.GetString("Username", playerName);

            Debug.Log(RememberMePlayFabID);

            var perms = new List<string>() { "public_profile", "user_friends" };
            FB.LogInWithReadPermissions(perms, OnFacebookAutoLogIn);

           /* PlayFabClientAPI.LoginWithFacebook(new LoginWithFacebookRequest { CreateAccount = true, AccessToken = RememberMeID },
                OnPlayfabFacebookAuthComplete, OnPlayfabFacebookAuthFailed);*/
            //OnFacebookInitialized();
        }

       /* FB.Android.RetrieveLoginStatus(LoginStatusCallback);
        Debug.Log("FB retrived");*/
    }

    private void OnFacebookAutoLogIn(ILoginResult result)
    {
        if (FB.IsLoggedIn && result == null || string.IsNullOrEmpty(result.Error))
        {
            PlayFabClientAPI.LoginWithFacebook(new LoginWithFacebookRequest { CreateAccount = true, AccessToken = RememberMeID },
              OnPlayfabFacebookAuthComplete, OnPlayfabFacebookAuthFailed);
        }
        else
            Debug.LogError(result.Error);
    }
}