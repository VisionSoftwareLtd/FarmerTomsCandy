using System.Runtime.InteropServices;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraShaker : MonoBehaviour
{
    private Camera myCamera;
    private float shake = 0;
    [SerializeField] float shakeAmount = 0.7f;
    private Vector3 originalPos;

    private void Awake()
    {
        myCamera = GetComponent<Camera>();
        originalPos = myCamera.transform.localPosition;
    }

    void Update()
    {
        if (shake > 0)
        {
            Vector3 position = Random.insideUnitSphere * shakeAmount * shake + originalPos;
            position.z = myCamera.transform.localPosition.z;
            myCamera.transform.localPosition = position;
            shake -= Time.deltaTime;
            if (shake < 0)
            {
                shake = 0;
                myCamera.transform.localPosition = originalPos;
            }
        }
    }

    public void ShakeCamera(float duration)
    {
        shake = duration;
    }
}