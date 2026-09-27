using Unity.Cinemachine;
using UnityEngine;

public class CinemachineCameraShaker : MonoBehaviour
{
  [SerializeField] private float shakeAmplitude = 10f;
  [SerializeField] private float shakeDuration = 2f;
  private float shakeEndTime;

  private CinemachineBasicMultiChannelPerlin perlin;

  private void Awake()
  {
    perlin = GetComponent<CinemachineBasicMultiChannelPerlin>();
  }

  void Update()
  {
    if (Time.time < shakeEndTime)
    {
      float ratio = (shakeEndTime - Time.time) / shakeDuration;
      perlin.AmplitudeGain = shakeAmplitude * ratio;
    }
    else if (perlin.AmplitudeGain != 0f)
    {
      perlin.AmplitudeGain = 0f;
    }
  }

  public void ShakeCamera()
  {
    shakeEndTime = Time.time + shakeDuration;
    perlin.AmplitudeGain = shakeAmplitude;
  }
}
