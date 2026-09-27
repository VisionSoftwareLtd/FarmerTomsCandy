using UnityEngine;

public class PersistentData : MonoBehaviour
{
    public static PersistentData instance { get; private set; }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        Reset();
    }

    public float mouseSensitivity { get; set; } = 1f;
    public int bottlesCollected { get; set; } = 0;

    public void Reset()
    {
        bottlesCollected = 0;
    }
}
