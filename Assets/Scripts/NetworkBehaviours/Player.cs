using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : NetworkBehaviour
{
  public override void OnNetworkSpawn()
  {
    base.OnNetworkSpawn();
    if (IsOwner)
    {
      // Initialize player-specific logic here
      Debug.Log($"Player {OwnerClientId} has spawned.");
    }
    if (IsHost)
    {
      transform.position = GameManager.Instance.ActivatePlayer(GameManager.StartingPosition.Farmer, transform);
    }
    else
    {
      transform.position = GameManager.Instance.ActivatePlayer(GameManager.StartingPosition.Children, transform);
    }
  }

  public override void OnNetworkDespawn()
  {
    base.OnNetworkDespawn();
  }

  [SerializeField] private float speed;
  private InputAction inputMove;

  private void Awake()
  {
    inputMove = InputSystem.actions["Move"];
  }

  private void Update()
  {
    if (!IsOwner) return;

    Vector3 cameraForward = Camera.main.transform.forward;
    cameraForward.y = 0;
    cameraForward.Normalize();
    Vector2 moveInput = inputMove.ReadValue<Vector2>();
    Vector3 move = (cameraForward * moveInput.y + Camera.main.transform.right * moveInput.x) * speed * Time.deltaTime;
    transform.Translate(move, Space.World);
  }

  private void OnCandiesChanged(int oldValue, int newValue)
  {
    Debug.Log($"Player {OwnerClientId} candies changed from {oldValue} to {newValue}.");
  }
}
