using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroductionManager : MonoBehaviour
{
    IEnumerator Start()
    {
        yield return null;
        SoundManager.Instance.PlayMusic("MenuIntro", true, 0.3f);
    }

    public void OnClickStart()
    {
        SceneManager.LoadScene("Game");
    }
}
