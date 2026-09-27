using UnityEngine;

public class Child : MonoBehaviour
{
  private Player parent;
  private Animator animator;

  private void Awake()
  {
    animator = GetComponentInChildren<Animator>();
    parent = GetComponentInParent<Player>();
  }

  private void Start()
  {
    parent.Speed = 6f;
  }

  public void UpdateAnimator(bool isMoving)
  {
    animator.SetBool("IsMoving", isMoving);
  }

  private void OnTriggerEnter(Collider other)
  {
    if (other.TryGetComponent(out Player player) && player.IsFarmer && parent.CanMove)
    {
      animator.SetTrigger("GetCandy");
      parent.DisableMovement();
      parent.UpdateChildEatCandyRpc();
      GameManager.Instance.OnChildGotCandy(parent);
    }
  }

  internal void EatCandy()
  {
    animator.SetTrigger("GetCandy");
  }
}
