using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Facebook.Unity;
using TMPro;

public class FacebookLogin : MonoBehaviour
{
    public TextMeshProUGUI friendsText;


    #region Initialization

    private void Awake()
    {
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

    #endregion


    #region Login/Logout Functions


    public void FB_Login()
    {
       /* var permissions = new List<string>() {"public_profile","user_friends" };
        FB.LogInWithReadPermissions(permissions);*/

        var perms = new List<string>() { "public_profile", "user_friends" };
        FB.LogInWithReadPermissions(perms, AuthCallback);
    }

   

    private void AuthCallback(ILoginResult result)
    {
        if (FB.IsLoggedIn)
        {
            // AccessToken class will have session details
            var aToken = Facebook.Unity.AccessToken.CurrentAccessToken;
            // Print current access token's User ID
          //  Debug.Log(aToken.UserId);
            Debug.Log("Facebook Logged IN");
            // Print current access token's granted permissions
            foreach (string perm in aToken.Permissions)
            {
               // Debug.Log(perm);
            }
        }
        else
        {
            Debug.Log("User cancelled login");
        }
    }

    public void FB_Logout()
    {
        FB.LogOut();
    }

    #endregion


    public void FB_Share()
    {
        FB.ShareLink(new System.Uri("http://resocoder.com"),"Check This App","",
            new System.Uri("http://resocoder.com/wp-content/uploads/2017/logoRound512.png"));
    }

    public void FB_GameRequest()
    {
        FB.AppRequest("Hey! Come Play This Awesome Game ", title : "Spelling Game ");
    }

    public void GetFriendsList()
    {
        string query = "/me/friends";
        FB.API(query, HttpMethod.GET, result =>
        {
           var jsonDicitonary = (Dictionary<string, object>)Facebook.MiniJSON.Json.Deserialize(result.RawResult);
           var friendsList = (List<object>)jsonDicitonary["data"];

           foreach (var dictionary in friendsList)
               friendsText.text += ((Dictionary<string, object>)dictionary)["name"];
        }
        );
    }

}
