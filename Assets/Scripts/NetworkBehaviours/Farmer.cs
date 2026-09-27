using UnityEngine;

public class Farmer : MonoBehaviour
{
  private Animator animator;

  private void Awake()
  {
    animator = GetComponentInChildren<Animator>();
  }

  public void UpdateAnimator(float moveX, float moveY)
  {
    animator.SetFloat("MoveX", moveX);
    animator.SetFloat("MoveY", moveY);
  }
}
