using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicScripts : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        if (FindObjectsOfType<MusicScripts>().Length > 1)
        {
            Destroy(gameObject);
        }
    }
}
