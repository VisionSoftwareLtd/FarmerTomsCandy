using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
  public static UI Instance { get; private set; }

  private void Awake()
  {
    Instance = this;
  }

  [SerializeField] private TextMeshProUGUI candyCountText;
  [SerializeField] private TextMeshProUGUI winScreenText;
  [SerializeField] private Button mainMenuButton;

  private void Start()
  {
    winScreenText.gameObject.SetActive(false);
    mainMenuButton.gameObject.SetActive(false);
  }

  public void UpdateCandyCount(int count)
  {
    candyCountText.text = "Candies remaining: " + count.ToString();
  }

  internal void ShowWinScreen(int numCandies)
  {
    winScreenText.gameObject.SetActive(true);
    mainMenuButton.gameObject.SetActive(true);
    winScreenText.text = $"<palette>The farmer made it to his truck with <wave>{numCandies}</wave> candies!</palette>";
  }
}
