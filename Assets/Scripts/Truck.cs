using UnityEngine;

public class Truck : MonoBehaviour
{
  public void OnTriggerEnter(Collider other)
  {
    if (other.TryGetComponent(out Player player) && player.IsFarmer)
    {
      GameManager.Instance.WinGame();
    }
  }
}
