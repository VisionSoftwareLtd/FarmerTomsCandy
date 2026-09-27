using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MainMenuManager : MonoBehaviour
{
    IEnumerator Start()
    {
        yield return null;
        SoundManager.instance.PlayMusic("MainMenu");
    }

    public void OnClickStart()
    {
        SceneManager.LoadScene("Introduction");
    }
}
