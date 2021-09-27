using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using SimpleJSON;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
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
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI currentLevel;
    public TextMeshProUGUI lastLevel;

    public GameObject uiPanel;
    public GameObject startPanel;
    public GameObject gameplayPanel;
    public Button nextButton;
    AudioSource wordAudioSource;

    private string enteredWord;
    private string easyLevelKey = "EasyLevel";
    private string mediumLevelKey = "MediumLevel";
    private string avatarKey = "AvatarKey";
    private string freeSettingKey = "FreeSettingKey";

    public int wordIndex;
    public int levelIndex;
    int score;
    int totalWords;
    int easyLevel;
    int mediumLevel;
    [SerializeField] 
    int avatarNumber;
    [SerializeField] 
    int rightAnswer;

    public TextMeshProUGUI timerText;
    private float floatTimer;
    private bool timerOn;

    public Image avatar;

    [Space]
    public Image[] avatarButtons;
    public Button[] difficultLevelButtons;
    public GameObject[] difficultLevelScreens;
    public Button[] wordLevelButtons;
    public Button[] freeSettingButtons;

   // public Button confirmButton;
    [Space]
    public TMP_InputField numberInput;
    public Slider levelProgression;

    //PlayFabLogin playFabLogin;

    #region API Variables

    private readonly string spellingAPI_URL = "https://api.dictionaryapi.dev/api/v2/entries/en_US/";

    string wordURL;
    string wordDefinitionURL;
    string wordSentenceURL;
    string wordAudioURL;

    UnityWebRequest wordInfoRequest;
    UnityWebRequest wordAudioRequest;
    JSONNode wordInfo;


    #endregion

    #endregion


    #region Default functions


    private void Awake()
    {
        wordIndex = 0;
        score = 0;
        rightAnswer = 0;

        numberInput.text = "5";
        scoreText.text = "Score : " + score.ToString();
        levelProgression.interactable = false;

        easyLevel = PlayerPrefs.GetInt(easyLevelKey, 0);
        mediumLevel = PlayerPrefs.GetInt(mediumLevelKey, 0);
        avatarNumber = PlayerPrefs.GetInt(avatarKey, avatarNumber);
        levelIndex = PlayerPrefs.GetInt(freeSettingKey, levelIndex);

        if (levelIndex == 0)
            levelIndex = 4;


        if (easyLevel < 25)
        {
            difficultLevelButtons[1].interactable = false;
            difficultLevelButtons[2].interactable = false;
        }
        else if (mediumLevel < 50)
            difficultLevelButtons[2].interactable = false;


        if (PlayerPrefs.HasKey("AvatarKey"))
        {
            if (GameObject.Find("Panel : Avatar"))
                GameObject.Find("Panel : Avatar").SetActive(false);
            avatar.sprite = avatarButtons[avatarNumber].GetComponent<Image>().sprite;
            avatarButtons[avatarNumber].transform.parent.GetComponent<Image>().color = Color.blue;

            startPanel.SetActive(true);
        }

        if (PlayerPrefs.HasKey("FreeSettingKey"))
        {
            freeSettingButtons[levelIndex - 1].GetComponent<Image>().color = Color.blue;
            if (numberInput.text != null)
            {
                int temp = int.Parse(numberInput.text);
                levels[levelIndex - 1].words = new Word[temp];
            }
        }

        // GameObject.Find("Infinite").GetComponent<Button>().interactable = false;

        // PlayerPrefs.DeleteAll();

    }


    // Sets the value of intergers levelIndex,wordindex and scre to 0.
    // Also sets the value of float floatTimer to 0 at the start.
    // floaTimer is a float variable used to calculate the time during the game
    // wordAudioSource is the audiosource variable of this gameObject
    private void Start()
    {

        timerText.text = floatTimer.ToString("F2");

        wordAudioSource = GetComponent<AudioSource>();
       // playFabLogin = GetComponent<PlayFabLogin>();

        GameStart();


    }


    // when the timerOn boolean is true the timer starts its counting
    // it counts in minutes and seconds 
    // the Mathf.FloortoInt() converts float into an integer
    // the timer is displayed in scene in string format in the timerText text field
    //
    // when the gameobject uiPanel's 1st child is active 
    // wordIndex is set to 0
    // the 1st and the 2nd child of uiPanel's 1st child display the 
    // score and the time taken to complete the level 
    //
    // levelIndex is also set to 0
    // and the timerOn boolean is to false to stop the timer count as the level is completed
    private void Update()
    {

        if (gameplayPanel.activeInHierarchy)
            timerOn = true;

        else if(!gameplayPanel.activeInHierarchy)
            timerOn = false;

        

        if (timerOn)
        {
            floatTimer -= Time.deltaTime;
            int minutes = Mathf.FloorToInt(floatTimer / 60f);
            int seconds = Mathf.FloorToInt(floatTimer - minutes * 60);
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);

            if (Mathf.FloorToInt (floatTimer)== 0)
            {
                timerOn = false;

                uiPanel.transform.GetChild(2).gameObject.SetActive(true);
                uiPanel.transform.GetChild(2).GetChild(1).GetComponent<TextMeshProUGUI>().text
                    = "Right Answer : " + levels[levelIndex - 1].words[wordIndex - 1].originalWord.ToUpper();
            }
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

           // playFabLogin.SendLeaderBoard(score);

            timerOn = false;
        }

        if (difficultLevelScreens[0].activeInHierarchy)
        {
            for (int i = 0; i < wordLevelButtons.Length; i++)
            {
                wordLevelButtons[i].transform.GetChild(0).GetComponent<Text>().text = " Level " + i;
            }
        }
        
    }

    #endregion


    #region UI Button Functions 

    /// <summary>
    /// 
    // this function checks the whether the entered word is right or wrong
    //
    // if the enteredWord string matches the originalWord of the Word[] 
    // the the answer is right and the 2nd child of gameobject uiPanel 
    // is set to be active which displays the originalWord
    // and the score is also increased by 20
    //
    // if the enteredWord string does not match the originalWord 
    // the wrongPanel of the gameobject uiPanel is set to be active 
    // and it displays the right answer for the user to check his mistake
    /// </summary>
    public void Check()
    {
        timerOn = false;


        if (enteredWord == null)
            return;


        if (enteredWord.ToUpper() == levels[levelIndex - 1].words[wordIndex - 1].originalWord.ToUpper())
        {
            //Debug.Log("Right Answer");

            rightAnswer++;
           // levelProgression.value++;
            score  = score + Mathf.FloorToInt(floatTimer) + 20;
            scoreText.text = "Score : " + score.ToString();

          //  currentLevel.text = levelProgression.value.ToString() ;
          //  lastLevel.text = levelProgression.maxValue.ToString();

            //UI Section
            uiPanel.transform.GetChild(1).gameObject.SetActive(true);
            uiPanel.transform.GetChild(1).GetChild(2).GetComponent<TextMeshProUGUI>().text
                = levels[levelIndex - 1].words[wordIndex - 1].originalWord.ToUpper();

            if (rightAnswer >= 25 && levelIndex == 1)
            {
                difficultLevelButtons[1].interactable = true;
                wordLevelButtons[wordIndex].interactable = true;

                rightAnswer = easyLevel;

                PlayerPrefs.SetInt(easyLevelKey, easyLevel);
                PlayerPrefs.Save();
            }
            else if (rightAnswer >=50 && levelIndex == 2)
            {
                difficultLevelButtons[2].interactable = true;
                rightAnswer = mediumLevel;

                PlayerPrefs.SetInt(mediumLevelKey, mediumLevel);
                PlayerPrefs.Save();
            }
        }
        else
        {
           // Debug.Log("Wrong Answer");

            uiPanel.transform.GetChild(2).gameObject.SetActive(true);
            uiPanel.transform.GetChild(2).GetChild(2).GetComponent<TextMeshProUGUI>().text
                = "Right Answer : " + levels[levelIndex - 1].words[wordIndex - 1].originalWord.ToUpper();
        }
    }


    /// <summary>
    /// it relaods the current scene when the restart button is pressed
    /// </summary>
    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }


    /// <summary>
    /// it plays the audio clip downlaoded from the api
    /// when the sound button is clicked
    /// </summary>
    public void Sound()
    {
        wordAudioSource.volume = 0.8f;
        wordAudioSource.PlayOneShot(levels[levelIndex - 1].words[wordIndex - 1].wordAudio);
    }

    public void Display_Challenges()
    {
        difficultLevelScreens[levelIndex - 1].SetActive(true);
    }

    #endregion
   

    #region Keyboard Functions


    /// <summary>
    /// this function is called when the keyboard buttons are clicked
    /// every button has a unique letter which is passed by the string alphabet
    /// this function is used to enter the word in the input word field
    /// the inputWord text displays the enterWord
    /// </summary>
    /// <param name="alphabet"></param>
    public void KeyBoard(string alphabet)
    {
        enteredWord = enteredWord + alphabet;
        inputWord.text = enteredWord.ToUpper();
    }


    /// <summary>
    /// this function is called when the delete button is pressed
    /// it erases the unwanted letters from the enterWord 
    /// the substring() reduces the length the word by subtracting 1 element from the end  
    /// </summary>
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


    /// <summary>
    /// the string wordUrl is the link of the api + the originalWord 
    /// obtain from the json file 
    /// wordInfoRequest sends a UnityWebRequest using this wordUrl link
    /// 
    /// in the while loop
    /// if the word is not found in the api 
    /// it keeps searching for another word untl it finds a new word which has the required information
    /// and then it return the UnitywebRequest
    /// 
    ///  
    /// for downlaoding the audio from the api another webRequest is created
    /// which does hte functioning as the above unitywebRequest
    /// 
    /// in the  for (int i = 0; i < levels[levelIndex - 1].words.Length; i++)
    /// the variables from the Word[] in the Level[] are assigned the their particular values
    /// </summary>
    /// <returns></returns>
    IEnumerator GetWordFromAPI()
    {
        wordURL = spellingAPI_URL + levels[levelIndex - 1].words[wordIndex - 1].originalWord;
       // Debug.Log(wordURL);

        wordInfoRequest = UnityWebRequest.Get(wordURL);

        yield return wordInfoRequest.SendWebRequest();

        while (wordInfoRequest.isHttpError || wordInfoRequest.isNetworkError)
        {
           // Debug.LogError(wordInfoRequest.error);

            // while (wordInfoRequest.isHttpError || wordInfoRequest.isNetworkError)
            {
                int random = Random.Range(0, totalWords);
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

          /*  wordAudioSource.volume = 0.05f;
            wordAudioSource.PlayOneShot(levels[levelIndex - 1].words[wordIndex - 1].wordAudio);*/

        }
    }
    
    #endregion


    #region In-Game Functions


    /// <summary>
    /// this function is called when the nextWord button is clicked
    /// 
    /// the wordIndex is increased by 1 everytime the button is clicked
    /// if wordIndex <= the length of Word[] then
    /// the coroutine is called which contains the api and the json file
    /// 
    /// if the wordindex > the length of the Word[] then it means the level is complete
    /// and the uiPAnel is set to be active
    /// </summary>
    public void Next_Word_Button()
    {
        wordIndex++;

        floatTimer = 120;

       // timerOn = true;

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

    }


    /// <summary>
    /// its sorts the words in the Word[] i ascending order
    /// meaning it sorts the words based on the lenght of the word
    /// the word with small length is place 1st and so on 
    /// </summary>
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


    /// <summary>
    /// this is function called when the nextLevel button is pressed
    /// 
    /// levelIndex increases by 1 everytime when the button is pressed
    /// all the text and strings are set to null at the start
    /// 
    /// timerOn is set true as the level starts when this function is called
    /// 
    /// the GeneratingLEvels() generates the levels based on the number of letters 
    /// and the WordsScting() sorts these words in the ascending order
    /// </summary>
    public void Next_Level_Button()
    {
        levelIndex++;
        
        definitionBox.text = inputWord.text = sentence.text = null;
        enteredWord = null;

        nextButton.interactable = true;

        GeneratingLevels();

       /* for (int i = 0; i < levels[levelIndex - 1].words.Length; i++)
        {
            wordIndex++;
            StartCoroutine(GetWordFromAPI());
        }

        wordIndex = 0;*/

    }


    /// <summary>
    /// this functions displays the use of the word in the expamle sentence 
    /// if the word to be entered is in the sentence 
    /// it replaces that word with "...." this string so the player can fill the blanks
    /// </summary>
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


    /// <summary>
    /// this functions generates the words for the Level[]
    /// 
    /// the string jsonStrings stores all the data from the Words.json file
    /// and then the JSPNNode json coberts that file into json format
    /// 
    /// the array wordsArrays stores all the words from the json file
    /// in the  for (int i = 0; i < wordArrays.Length; i++)
    /// if the length of the string(words) is 4 or <= 6 then 
    /// it will genearate the words for the 1st Level
    /// 
    /// similarly, for Level 2 and 3
    /// if the string length are >= 7 and <= 9 
    /// and level 3 if the string length are >= 10 and <= 12
    /// it will Generate the levels repectively 
    /// 
    ///  if (i == wordArrays.Length - 1)
    ///  the words are stored in the Word[] from wordArrys[] only
    ///  until the Word[] length is full 
    /// </summary>
    void GeneratingLevels()
    {
        //string jsonString = File.ReadAllText(Application.dataPath + "/data/Words.json");

       /* if (wordIndex == 0)
            wordIndex++;*/

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

       // if (wordIndex != 0)
        {
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

                if (levelIndex == 4)
                {
                    int random = Random.Range(1000, 100000);
                    wordArrays[j].newWord = json["words"][random];
                    j++;
                }

                if (i == wordArrays.Length - 1)
                {
                    totalWords = j;
                    for (int k = 0; k < levels[levelIndex - 1].words.Length; k++)
                    {
                        int random = Random.Range(0, j);
                        levels[levelIndex - 1].words[k].originalWord = wordArrays[random].newWord;
                    }
                }
            }
        }
        //  Debug.Log(totalWords);

        #endregion

    }


    #endregion


    #region UI Functionality


    // Quits the application when pressed on the Quit button
    public void QuitGame()
    {
        Application.Quit();
    }


    /// <summary>
    /// selects the Level to start according to ones choice 
    /// when clicking the desired level button
    /// </summary>
    /// <param name="number"></param>
    public void SelectLevel(int number)
    {
        wordIndex = number;

        Next_Level_Button();
        Next_Word_Button();
    }

    public void LevelMode(int number)
    {
        levelIndex = number;
    }

    // Selects the desired avatar for the user
    public void SelectAvatar(int number)
    {
        for (int i = 0; i < avatarButtons.Length; i++)
        {
            avatarButtons[i].transform.parent.GetComponent<Image>().color = Color.white;
        }

        avatarNumber = number;
        avatar.sprite = avatarButtons[avatarNumber].GetComponent<Image>().sprite;
        avatarButtons[avatarNumber].transform.parent.GetComponent<Image>().color = Color.green;
        PlayerPrefs.SetInt(avatarKey,avatarNumber);
        PlayerPrefs.Save();
    }


    /// <summary>
    /// the user decides the number words to be present in the level
    /// the user enters the number int input field 
    /// and the int.Parse() converts the text into integer 
    /// 
    /// levels[number].words = new Word[temp];
    /// Changes the size of the Word[] according the user imput
    /// </summary>
    /// <param name="number"></param>
    public void FreeSetting_Level(int number)
    {
        wordIndex = 0;
        levelIndex = number;

        for (int i = 0; i < freeSettingButtons.Length; i++)
        {
            freeSettingButtons[i].GetComponent<Image>().color = Color.white;
        }

        freeSettingButtons[levelIndex - 1].GetComponent<Image>().color = Color.green;

        PlayerPrefs.SetInt(freeSettingKey,levelIndex);
        PlayerPrefs.Save();

        if (numberInput.text != null)
        {
            int temp = int.Parse(numberInput.text);
           // Debug.Log(temp);

            levels[levelIndex - 1].words = new Word[temp];
        }

        if (levelIndex == 4 )
        {
            levels[levelIndex - 1].words = new Word[100];
        }
    }

    /// <summary>
    /// Starts the game 
    /// And calls the NextLevelButton() and the NextWordButton()
    /// 
    /// this function is only called if the FreeSettingLevel() is called
    /// </summary>
    public  void GameStart()
    {
        if(levelIndex != 0 )
           levelIndex--;

        Next_Level_Button();

       // for (int i = 0; i < levels[levelIndex - 1].words.Length; i++)
        {
            Next_Word_Button();
        }
    }

   

    #endregion 
}
