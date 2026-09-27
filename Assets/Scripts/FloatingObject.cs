using UnityEngine;

public class FloatingObject : MonoBehaviour
{
    [SerializeField] private float amplitude = 0.5f;
    [SerializeField] private float frequency = 1f;

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        Vector3 tempPos = startPos;
        tempPos.y += Mathf.Sin(Time.fixedTime * Mathf.PI * frequency) * amplitude;
        transform.position = tempPos;
    }
}
