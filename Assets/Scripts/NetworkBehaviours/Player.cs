using System;
using System.Linq;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using Unity.Services.Multiplayer;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : NetworkBehaviour
{
  [SerializeField] private Farmer farmerPrefab;
  [SerializeField] private Child childPrefab;
  [SerializeField] private TextMeshPro nameText;
  private CharacterController characterController;
  private Farmer farmerInstance;
  private Child childInstance;
  private bool isFarmer;
  public bool IsFarmer => isFarmer;
  public bool CanMove { get; private set; } = false;
  private readonly NetworkVariable<FixedString64Bytes> playerName =
    new(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

  public float Speed { get; set; } = 5f;
  [SerializeField] private float jumpHeight = 1.5f;
  private InputAction inputMove;
  private InputAction inputJump;
  private Vector3 verticalVelocity;

  private void Awake()
  {
    inputMove = InputSystem.actions["Move"];
    inputJump = InputSystem.actions["Jump"];
    characterController = GetComponent<CharacterController>();
  }

  public override void OnNetworkSpawn()
  {
    base.OnNetworkSpawn();

    // The host always plays the farmer, every other client plays a child.
    isFarmer = OwnerClientId == NetworkManager.ServerClientId;
    if (isFarmer)
    {
      farmerInstance = Instantiate(farmerPrefab, transform);
    }
    else
    {
      childInstance = Instantiate(childPrefab, transform);
    }

    playerName.OnValueChanged += OnPlayerNameChanged;
    nameText.text = playerName.Value.ToString();

    if (IsOwner)
    {
      Debug.Log($"Player {OwnerClientId} has spawned.");
      // Only the local owning client should move its own player and take over the shared camera.
      GameManager.StartingPosition startingPosition = isFarmer
        ? GameManager.StartingPosition.Farmer
        : GameManager.StartingPosition.Children;
      characterController.enabled = false;
      transform.position = GameManager.Instance.ActivatePlayer(startingPosition, this);
      characterController.enabled = true;
    }
  }

  public override void OnNetworkDespawn()
  {
    base.OnNetworkDespawn();
    playerName.OnValueChanged -= OnPlayerNameChanged;
  }

  private void OnPlayerNameChanged(FixedString64Bytes oldValue, FixedString64Bytes newValue)
  {
    nameText.text = newValue.ToString();
  }

  private void Update()
  {
    if (!IsOwner) return;

    if (characterController.isGrounded && verticalVelocity.y < 0)
    {
      verticalVelocity.y = -2f;
    }

    if (characterController.isGrounded && inputJump.WasPressedThisFrame() && isFarmer)
    {
      verticalVelocity.y = Mathf.Sqrt(jumpHeight * -2f * Physics.gravity.y);
    }

    verticalVelocity.y += Physics.gravity.y * Time.deltaTime;

    Vector3 cameraForward = Camera.main.transform.forward;
    cameraForward.y = 0;
    cameraForward.Normalize();
    // Rotate character to face camera direction
    transform.rotation = Quaternion.LookRotation(cameraForward);
    Vector2 moveInput = inputMove.ReadValue<Vector2>();
    if (!CanMove) moveInput = Vector2.zero;
    Vector3 move = (cameraForward * moveInput.y + Camera.main.transform.right * moveInput.x) * Speed;
    characterController.Move((move + verticalVelocity) * Time.deltaTime);

    if (isFarmer)
    {
      farmerInstance.UpdateAnimator(moveInput.x, moveInput.y);
      UpdateFarmerAnimatorRpc(moveInput.x, moveInput.y);
    }
    else
    {
      bool isMoving = moveInput.sqrMagnitude > 0.0001f;
      childInstance.UpdateAnimator(isMoving);
      UpdateChildAnimatorRpc(isMoving);
    }
  }

  [Rpc(SendTo.NotOwner)]
  private void UpdateFarmerAnimatorRpc(float moveX, float moveY)
  {
    farmerInstance.UpdateAnimator(moveX, moveY);
  }

  [Rpc(SendTo.NotOwner)]
  private void UpdateChildAnimatorRpc(bool isMoving)
  {
    childInstance.UpdateAnimator(isMoving);
  }

  [Rpc(SendTo.NotOwner)]
  public void UpdateChildEatCandyRpc()
  {
    childInstance.EatCandy();
  }

  public void DisableMovement()
  {
    CanMove = false;
    Invoke(nameof(EnableMovement), 3f);
  }

  public void EnableMovement()
  {
    CanMove = true;
  }

  internal void SetPlayerName(string playerName)
  {
    this.playerName.Value = playerName;
  }

  public void EndGame()
  {
    CanMove = false;
  }
}
