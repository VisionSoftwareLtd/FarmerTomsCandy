using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MainMenuManager : MonoBehaviour
{
    IEnumerator Start()
    {
        yield return null;
        SoundManager.Instance.PlayMusic("MenuIntro", true, 0.3f);
    }

    public void OnClickStart()
    {
        SceneManager.LoadScene("Introduction");
    }
}
