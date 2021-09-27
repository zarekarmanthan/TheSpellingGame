using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.UI;

public class AdsIntegration : MonoBehaviour,IUnityAdsListener
{

    private string android_ID = "4241411";
    private string interstitial_ID = "Interstitial_Android";
    private string rewarded_ID = "Rewarded_Android";
    public bool testMode;
    public Button interstitial_Button;
    public Button rewarded_Button;

    private void Start()
    {
        Advertisement.Initialize(android_ID,testMode);
        Advertisement.AddListener(this);

        interstitial_Button.interactable = false;
        rewarded_Button.interactable = false;
    }

    public void ShowInterstitial()
    {
        if (Advertisement.IsReady(interstitial_ID))
            Advertisement.Show(interstitial_ID);
    }

    public void ShowRewardedVideo()
    {
        Advertisement.Show(rewarded_ID);
    }

    public void OnUnityAdsReady(string placementId)
    {
        if (placementId == interstitial_ID)
            interstitial_Button.interactable = true;

        if (placementId == rewarded_ID)
            rewarded_Button.interactable = true;
    }
    public void OnUnityAdsDidFinish(string placementId, ShowResult showResult)
    {
        if (placementId == rewarded_ID)
        {
            if (showResult == ShowResult.Finished)
                Debug.Log("Reward Collected");

            if (showResult == ShowResult.Skipped)
                Debug.Log("Watch the full video to recieve the reward " );

            if (showResult == ShowResult.Failed)
                Debug.Log("Failed to Load " );
                
        }
    }

    public void OnUnityAdsDidError(string message)
    {
       // throw new System.NotImplementedException();
    }

    public void OnUnityAdsDidStart(string placementId)
    {
        //throw new System.NotImplementedException();
    }

}
