using System;
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

  private NetworkVariable<int> numCandies = new NetworkVariable<int>(20);
  [SerializeField] private UIDocument sessionDocument;
  [SerializeField] private Transform[] startingPositions;
  [SerializeField] private CinemachineCamera cinemachineCamera;
  private int localNumCandies;
  private Player localPlayer;

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
    localNumCandies = numCandies.Value;
    UI.Instance.UpdateCandyCount(localNumCandies);
    cinemachineCamera.gameObject.SetActive(false);
    Camera.main.transform.position = startingPositions[(int)StartingPosition.Camera].position;
  }

  private void OnNumCandiesChanged(int oldValue, int newValue)
  {
    localNumCandies = newValue;
    UI.Instance.UpdateCandyCount(newValue);
  }

  [Rpc(SendTo.Server)]
  public void SubtractCandyServerRpc()
  {
    if (numCandies.Value > 0)
    {
      numCandies.Value--;
    }
  }

  public void HideSessionDocument()
  {
    sessionDocument.gameObject.SetActive(false);
    UnityEngine.Cursor.lockState = CursorLockMode.Locked;
    UnityEngine.Cursor.visible = false;
  }

  internal Vector3 ActivatePlayer(StartingPosition startingPosition, Player targetPlayer)
  {
    localPlayer = targetPlayer;
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
    cinemachineCamera.Follow = targetPlayer.transform;
    cinemachineCamera.gameObject.SetActive(true);
    return startingPositionVector;
  }

  internal void OnChildGotCandy(Player parent)
  {
    if (numCandies.Value > 0)
    {
      SubtractCandyServerRpc();
    }
  }

  internal void SetPlayerName(string playerName)
  {
    if (localPlayer != null)
    {
      if (playerName.IndexOf('#') != -1)
      {
        playerName = playerName.Substring(0, playerName.IndexOf('#'));
      }
      localPlayer.SetPlayerName(playerName);
      localPlayer.EnableMovement();
    }
  }

  internal void WinGame()
  {
    StopMovementRpc();
  }

  [Rpc(SendTo.Everyone)]
  private void StopMovementRpc()
  {
    UI.Instance.ShowWinScreen(numCandies.Value);
    localPlayer.EndGame();
  }
}
