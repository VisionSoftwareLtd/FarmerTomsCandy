using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class SliderDrag : MonoBehaviour, IPointerUpHandler
{
  [SerializeField] private UnityEvent OnSlidingFinished;
  public void OnPointerUp(PointerEventData eventData)
  {
    OnSlidingFinished?.Invoke();
  }
}