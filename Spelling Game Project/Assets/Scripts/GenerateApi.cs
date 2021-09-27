using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;   

public class GenerateApi : MonoBehaviourPunCallbacks
{
    public GameObject canvas;

    // Start is called before the first frame update
    void Start()
    {
       // if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.Instantiate(canvas.name, transform.position, transform.rotation);

           // FindObjectOfType<SpellingCheck>().getData(canvas);
        }
    }  
}
