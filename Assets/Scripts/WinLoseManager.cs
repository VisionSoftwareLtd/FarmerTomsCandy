using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinLoseManager : MonoBehaviour
{
    private IEnumerator Start()
    {
        yield return null;
        if (SceneManager.GetActiveScene().name == "Win") SoundManager.Instance.PlayMusic("Win");
        else if (SceneManager.GetActiveScene().name == "Lose") SoundManager.Instance.PlayMusic("Lose");
    }

    public void OnClickMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
