using TMPro;
using UnityEngine;

public class UI : MonoBehaviour
{
  public static UI Instance { get; private set; }

  private void Awake()
  {
    Instance = this;
  }

  [SerializeField] private TextMeshProUGUI candyCountText;
  [SerializeField] private TextMeshProUGUI winScreenText;

  private void Start()
  {
    winScreenText.gameObject.SetActive(false);
  }

  public void UpdateCandyCount(int count)
  {
    candyCountText.text = "Candies remaining: " + count.ToString();
  }

  internal void ShowWinScreen(int numCandies)
  {
    winScreenText.gameObject.SetActive(true);
    winScreenText.text = $"<palette>The farmer made it to his truck with <wave>{numCandies}</wave> candies!</palette>";
  }
}
