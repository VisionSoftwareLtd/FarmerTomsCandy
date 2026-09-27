using UnityEngine;

public class OneShotParticles : MonoBehaviour
{
  void Start()
  {
    ParticleSystem ps = GetComponent<ParticleSystem>();
    if (ps != null)
    {
      ps.Play();
      Destroy(gameObject, ps.main.duration + ps.main.startLifetime.constantMax);
    }
  }
}
