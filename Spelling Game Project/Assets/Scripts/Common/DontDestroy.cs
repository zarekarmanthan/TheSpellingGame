using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestory : MonoBehaviour
{
    private void Awake()
    {

    }

    private void Start()
    {
        GameObject[] objs = GameObject.FindGameObjectsWithTag("TeamManager");

        if (objs.Length > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}
