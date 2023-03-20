using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class SceneLoader : MonoBehaviour
{
    private static SceneLoader instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public static SceneLoader Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new GameObject("SceneLoader").AddComponent<SceneLoader>();
                DontDestroyOnLoad(instance.gameObject);
            }
            return instance;
        }
    }
}

