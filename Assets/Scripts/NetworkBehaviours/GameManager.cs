using System.Collections;
using Unity.Cinemachine;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
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

  private IEnumerator Start()
  {
    localNumCandies = numCandies.Value;
    UI.Instance.UpdateCandyCount(localNumCandies);
    cinemachineCamera.gameObject.SetActive(false);
    Camera.main.transform.position = startingPositions[(int)StartingPosition.Camera].position;
    yield return null;
    SoundManager.Instance.StopMusic();
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
      SoundManager.Instance.PlaySoundRandomPitch("Eat");
      numCandies.Value--;
    }
  }

  [Rpc(SendTo.Everyone)]
  private void PlayerJoinedRpc()
  {
    SoundManager.Instance.PlaySound("Spawn", 0.8f);
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
        startingPositionVector += new Vector3(UnityEngine.Random.Range(-1f, 1f), 0, UnityEngine.Random.Range(-0.2f, 0.2f));
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
      PlayerJoinedRpc();
      SoundManager.Instance.PlayMusic("Game", true, 0.3f);
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
    SoundManager.Instance.PlayMusic("GameEnd", true, 0.3f);
    UnityEngine.Cursor.lockState = CursorLockMode.None;
    UnityEngine.Cursor.visible = true;
  }

  public void OnClickMainMenu()
  {
    SceneManager.LoadScene("MainMenu");
  }
}
