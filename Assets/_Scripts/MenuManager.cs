using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public GameObject data;
    //The order of these scenes can be found within "Build Settings"

    //Menu is 0
    //Game is 1
    //User manual is 2
    //Credits is 3
    public void LoadGame()
    {
        SceneManager.LoadScene(1);
    }

    public void LoadEndGame()
    {
        DontDestroyOnLoad(data);
        SceneManager.LoadScene(3);
    }

    public void LoadUserManual()
    {
        SceneManager.LoadScene(2);
    }

    public void LoadMenu()
    {
        SceneManager.LoadScene(0);
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
