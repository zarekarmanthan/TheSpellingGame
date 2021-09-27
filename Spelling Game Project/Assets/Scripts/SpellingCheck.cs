using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using SimpleJSON;
using TMPro;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;
using ExitGames.Client.Photon;

public class SpellingCheck : MonoBehaviour
{

    #region Variable Declaration

    #region Public Classes

    [System.Serializable]
    public class Word
    {
        public string originalWord;
        public string definitionInfo;
        public string exampleSentence;
        public AudioClip wordAudio;
    }

    [System.Serializable]
    public class Levels
    {
        public string levelName;
        public Word[] words;
    }


    [System.Serializable]
    public class WordArray
    {
        public string newWord;
    }


    #endregion

    public Levels[] levels;

    [HideInInspector]
    public WordArray[] wordArrays;

    public TextMeshProUGUI definitionBox;
    public TextMeshProUGUI sentence;
    public TextMeshProUGUI inputWord;

    public GameObject uiPanel;
    public GameObject scorePanel;
    public Button nextButton;
    AudioSource wordAudioSource;

    private string enteredWord;

    public int wordIndex;
    public int levelIndex;
   // [HideInInspector]
    public int score;
    int value;

    public TextMeshProUGUI timerText;
    private float floatTimer;
    private bool timerOn;

    #region API Variables

    private readonly string spellingAPI_URL = "https://api.dictionaryapi.dev/api/v2/entries/en_US/";

    string wordURL;
    string wordDefinitionURL;
    string wordSentenceURL;
    string wordAudioURL;

    UnityWebRequest wordInfoRequest;
    UnityWebRequest wordAudioRequest;
    JSONNode wordInfo;


    public TextAsset jsonData;

    #endregion

    public NewPlayerList newPlayer;
    [SerializeField]
    PhotonView view;
    [SerializeField]
    Score updateScore;

    private ExitGames.Client.Photon.Hashtable myPlayerProperties = new ExitGames.Client.Photon.Hashtable();
    public int gameOver;

    #endregion


    #region Default functions
    private void Awake()
    {
       /* score = (int)PhotonNetwork.LocalPlayer.CustomProperties["Score"];
        //score += 20;
        ExitGames.Client.Photon.Hashtable hash = new ExitGames.Client.Photon.Hashtable
        {
            { "Score", score }
        };
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);*/
    }

    private void Start()
    {
        levelIndex = 0;
        wordIndex = 0;
        score = 0;
        gameOver = 0;

        floatTimer = 0;
        timerText.text = floatTimer.ToString("F2");

        wordAudioSource = GetComponent<AudioSource>();


        Next_Level_Button();

        Next_Word_Button();


        timerOn = true;
    }

    private void Update()
    {
       /* if (view.IsMine )
        {
            view.RPC("UpdateScore",RpcTarget.All,score);
            //Debug.LogWarning(score);
        }*/

        if (gameOver == PhotonNetwork.CurrentRoom.PlayerCount)
        {
            scorePanel.SetActive(true);
        }


        if (uiPanel.transform.GetChild(0).gameObject.activeInHierarchy)
        {
            wordIndex = 0;

            uiPanel.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text
                = "Your Score : " + score;
            uiPanel.transform.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text
                = "Time Taken : " + timerText.text;

            if (levelIndex >= levels.Length)
            {
                levelIndex = 0;
            }
        }

        if (timerOn)
        {
            floatTimer += Time.deltaTime;
            int minutes = Mathf.FloorToInt(floatTimer / 60f);
            int seconds = Mathf.FloorToInt(floatTimer - minutes * 60);
            timerText.text = "Time : " + string.Format("{0:00}:{1:00}", minutes, seconds);
        }



    }

    #endregion


    #region UI Button Functions 

    public void Check()
    {
        if (enteredWord == null)
        {
            return;
        }

        if (enteredWord.ToUpper() == levels[levelIndex - 1].words[wordIndex - 1].originalWord.ToUpper())
        {
            Debug.Log("Right Answer");
             score += 20;

             SetCustomScore();

           /* if (view.IsMine && PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("Score"))
            {
                Debug.Log("contains key");
                score = (int)PhotonNetwork.LocalPlayer.CustomProperties["Score"];
                score += 20;
               
                PhotonNetwork.LocalPlayer.SetCustomProperties(myPlayerProperties);
            }*/


            uiPanel.transform.GetChild(1).gameObject.SetActive(true);
            uiPanel.transform.GetChild(1).GetChild(1).GetComponent<TextMeshProUGUI>().text
                = levels[levelIndex - 1].words[wordIndex - 1].originalWord.ToUpper();

           /* if (updateScore.view.IsMine && PhotonNetwork.LocalPlayer.IsLocal)
                updateScore.view.RPC("UpdateScore", RpcTarget.AllBuffered, score);*/

            PhotonNetwork.LocalPlayer.AddScore(score);
        }
        else
        {
            Debug.Log("Wrong Answer");

            uiPanel.transform.GetChild(2).gameObject.SetActive(true);
            uiPanel.transform.GetChild(2).GetChild(1).GetComponent<TextMeshProUGUI>().text
                = "Right Answer : " + levels[levelIndex - 1].words[wordIndex - 1].originalWord.ToUpper();
        }
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Sound()
    {
        wordAudioSource.volume = 0.8f;
        wordAudioSource.PlayOneShot(levels[levelIndex - 1].words[wordIndex - 1].wordAudio);
    }

    #endregion



    #region Keyboard Functions

    public void KeyBoard(string alphabet)
    {
        // for (int i = 0; i < word.Length; i++)
        {
            enteredWord = enteredWord + alphabet;
            inputWord.text = enteredWord.ToUpper();
        }
    }

    public void Erase()
    {
        if (inputWord.text.Length > 0)
        {
            inputWord.text = inputWord.text.Substring(0, inputWord.text.Length - 1);
            enteredWord = inputWord.text;
        }
    }

    #endregion


    #region Ienumerators

    IEnumerator GetWordFromAPI()
    {
        wordURL = spellingAPI_URL + levels[levelIndex - 1].words[wordIndex - 1].originalWord;
        Debug.Log(wordURL);

        wordInfoRequest = UnityWebRequest.Get(wordURL);

        yield return wordInfoRequest.SendWebRequest();

        while (wordInfoRequest.isHttpError || wordInfoRequest.isNetworkError)
        {
            Debug.LogError(wordInfoRequest.error);

            // while (wordInfoRequest.isHttpError || wordInfoRequest.isNetworkError)
            {
                int random = Random.Range(0, value);
                levels[levelIndex - 1].words[wordIndex - 1].originalWord = wordArrays[random].newWord;
                wordURL = spellingAPI_URL + levels[levelIndex - 1].words[wordIndex - 1].originalWord;
            }

            wordInfoRequest = UnityWebRequest.Get(wordURL);

            yield return wordInfoRequest.SendWebRequest();
        }

        wordInfo = JSON.Parse(wordInfoRequest.downloadHandler.text);

        wordDefinitionURL = wordInfo[0]["meanings"][0]["definitions"][0]["definition"];
        wordSentenceURL = wordInfo[0]["meanings"][0]["definitions"][0]["example"];
        wordAudioURL = wordInfo[0]["phonetics"][0]["audio"];

        wordAudioRequest = UnityWebRequestMultimedia.GetAudioClip(wordAudioURL, AudioType.MPEG);

        yield return wordAudioRequest.SendWebRequest();

        if (wordAudioRequest.isHttpError || wordAudioRequest.isNetworkError)
        {
            levels[levelIndex - 1].words[wordIndex - 1].wordAudio = null;
            yield break;
        }


        for (int i = 0; i < levels[levelIndex - 1].words.Length; i++)
        {
            levels[levelIndex - 1].words[wordIndex - 1].definitionInfo = wordDefinitionURL;
            definitionBox.text = "<b>Definition</b> :- " + levels[levelIndex - 1].words[wordIndex - 1].definitionInfo;

            levels[levelIndex - 1].words[wordIndex - 1].exampleSentence = wordSentenceURL;

            WordSentence();

            levels[levelIndex - 1].words[wordIndex - 1].wordAudio = DownloadHandlerAudioClip.GetContent(wordAudioRequest);

            wordAudioSource.volume = 0.2f;
            wordAudioSource.PlayOneShot(levels[levelIndex - 1].words[wordIndex - 1].wordAudio);

        }
    }


   

    #endregion


    #region In-Game Functions

    public void Next_Word_Button()
    {
        wordIndex++;

        definitionBox.text = null;
        sentence.text = null;
        inputWord.text = null;
        enteredWord = null;

        if (wordIndex <= levels[levelIndex - 1].words.Length)
            StartCoroutine(GetWordFromAPI());

        if (wordIndex >= levels[levelIndex - 1].words.Length + 1)
        {
            uiPanel.transform.GetChild(0).gameObject.SetActive(true);
        }

        if (updateScore.view.IsMine && PhotonNetwork.LocalPlayer.IsLocal)
        {
            updateScore.view.RPC("GameOver", RpcTarget.All);
        }

    }

    public void WordsSorting()
    {
        for (int i = 0; i < levels[levelIndex - 1].words.Length; i++)
        {
            for (int j = i + 1; j < levels[levelIndex - 1].words.Length; j++)
            {
                if (levels[levelIndex - 1].words[i].originalWord.Length > levels[levelIndex - 1].words[j].originalWord.Length)
                {
                    string temp = levels[levelIndex - 1].words[i].originalWord;
                    levels[levelIndex - 1].words[i].originalWord = levels[levelIndex - 1].words[j].originalWord;
                    levels[levelIndex - 1].words[j].originalWord = temp;
                }
            }
        }
       
    }

    public void Next_Level_Button()
    {
        levelIndex++;

        definitionBox.text = inputWord.text = sentence.text = null;
        enteredWord = null;

        nextButton.interactable = true;

        GeneratingLevels();

        WordsSorting();

    }

    public void WordSentence()
    {
        if (levels[levelIndex - 1].words[wordIndex - 1].exampleSentence != null)
        {
            string s = levels[levelIndex - 1].words[wordIndex - 1].exampleSentence;
            string newstr = s.Replace(levels[levelIndex - 1].words[wordIndex - 1].originalWord, ".......");
            sentence.text = "<b>Example </b>:- " + newstr;
        }
        else
        {
            sentence.text = "<b>Example :- NOT AVAILABLE </b>";
        }
      
    }

    void GeneratingLevels()
    {

        string sFilePath = Path.Combine(Application.streamingAssetsPath, "data/Words.json");
        string jsonString;
        if (Application.platform == RuntimePlatform.Android)
        {
            UnityWebRequest www = UnityWebRequest.Get(sFilePath);
            www.SendWebRequest();
            while (!www.isDone) ;
            jsonString = www.downloadHandler.text;
        }
        else jsonString = File.ReadAllText(sFilePath);

        JSONNode json = JSON.Parse(jsonString);
        #region Generating Levels

       // Debug.Log(_path);

        int j = 0;
        for (int i = 0; i < wordArrays.Length; i++)
        {
            string temp = json["words"][i];

            if (temp.Length >= 4 && temp.Length <= 6 && levelIndex == 1)
            {
                wordArrays[j].newWord = json["words"][i];
                j++;
            }

            if (temp.Length >= 7 && temp.Length <= 9 && levelIndex == 2)
            {
                wordArrays[j].newWord = json["words"][i];
                j++;
            }

            if (temp.Length >= 10 && temp.Length <= 12 && levelIndex == 3)
            {
                wordArrays[j].newWord = json["words"][i];
                j++;
            }

            if (i == wordArrays.Length - 1)
            {
                value = j;
                for (int k = 0; k < levels[levelIndex - 1].words.Length; k++)
                {
                    int random = Random.Range(0, j);
                    levels[levelIndex - 1].words[k].originalWord = wordArrays[random].newWord;
                }
            }
        }
        Debug.Log(value);

        #endregion

    }

    public void QuitGame()
    {
        Application.Quit();
    }

    #endregion


    #region PlayerCustomProperties
    
    private void SetCustomScore()
    {
        myPlayerProperties["Score"] = score;
        //PhotonNetwork.SetPlayerCustomProperties(myPlayerProperties);
        PhotonNetwork.LocalPlayer.CustomProperties = myPlayerProperties;
    }

    #endregion


    /* [PunRPC]
     public void UpdateScore(int playerScore)
     {
         playerScore = score;
         newPlayer.playerScoreText.text = playerScore.ToString();
         //Debug.LogWarning(score);
     }

     [PunRPC]
     public void GameOver()
     {
         if (uiPanel.transform.GetChild(0).gameObject.activeInHierarchy)
         {
             gameOver++;
         }
     }*/

}
