using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinLoseManager : MonoBehaviour
{
    private IEnumerator Start()
    {
        yield return null;
        if (SceneManager.GetActiveScene().name == "Win") SoundManager.instance.PlayMusic("Win");
        else if (SceneManager.GetActiveScene().name == "Lose") SoundManager.instance.PlayMusic("Lose");
    }

    public void OnClickMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
