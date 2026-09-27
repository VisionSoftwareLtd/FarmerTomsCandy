using Unity.Cinemachine;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

public class GameManager : NetworkBehaviour
{
  public static GameManager Instance { get; private set; }
  private void Awake()
  {
    Instance = this;
  }

  public enum StartingPosition
  {
    Camera,
    Farmer,
    Children
  }

  private NetworkVariable<int> numCandies = new NetworkVariable<int>(0);
  [SerializeField] private UIDocument sessionDocument;
  [SerializeField] private Transform[] startingPositions;
  [SerializeField] private CinemachineCamera cinemachineCamera;

  public override void OnNetworkSpawn()
  {
    base.OnNetworkSpawn();
    numCandies.OnValueChanged += OnNumCandiesChanged;
  }

  public override void OnNetworkDespawn()
  {
    base.OnNetworkDespawn();
    numCandies.OnValueChanged -= OnNumCandiesChanged;
  }

  private void Start()
  {
    cinemachineCamera.gameObject.SetActive(false);
    Camera.main.transform.position = startingPositions[(int)StartingPosition.Camera].position;
  }

  private void OnNumCandiesChanged(int oldValue, int newValue)
  {
    Debug.Log($"Number of candies changed from {oldValue} to {newValue}.");
  }

  [Rpc(SendTo.Server)]
  public void AddCandyServerRpc()
  {
    numCandies.Value++;
  }

  public void HideSessionDocument()
  {
    sessionDocument.gameObject.SetActive(false);
  }

  internal Vector3 ActivatePlayer(StartingPosition startingPosition, Transform trackingTarget)
  {
    Vector3 startingPositionVector = Vector3.zero;
    switch (startingPosition)
    {
      case StartingPosition.Farmer:
        startingPositionVector = startingPositions[(int)StartingPosition.Farmer].position;
        break;
      case StartingPosition.Children:
        startingPositionVector = startingPositions[(int)StartingPosition.Children].position;
        break;
    }
    cinemachineCamera.Follow = trackingTarget;
    cinemachineCamera.gameObject.SetActive(true);
    return startingPositionVector;
  }
}
