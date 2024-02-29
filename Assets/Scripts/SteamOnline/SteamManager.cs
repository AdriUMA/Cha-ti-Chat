using Netcode.Transports.Facepunch;
using Steamworks;
using Steamworks.Data;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class SteamManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField _lobbyIDInputField;
    [SerializeField] private TextMeshProUGUI _lobbyID;

    [SerializeField] private GameObject _mainMenu;
    [SerializeField] private GameObject _inLobbyMenu;

    private void Start()
    {
        CheckUI();
    }

    private void OnEnable()
    {
        SteamMatchmaking.OnLobbyCreated += OnLobbyCreated;
        SteamMatchmaking.OnLobbyEntered += OnLobbyEntered;
        SteamFriends.OnGameLobbyJoinRequested += OnGameLobbyJoinRequested;
    }

    private void OnDisable()
    {
        SteamMatchmaking.OnLobbyCreated -= OnLobbyCreated;
        SteamMatchmaking.OnLobbyEntered -= OnLobbyEntered;
        SteamFriends.OnGameLobbyJoinRequested -= OnGameLobbyJoinRequested;
    }

    private async void OnGameLobbyJoinRequested(Lobby lobby, SteamId id)
    {
        await lobby.Join();
    }

    private void OnLobbyEntered(Lobby lobby)
    {
        SteamLobbySaver.instance.currentLobby = lobby;
        Debug.Log("Lobby joined");

        CheckUI();

        if(NetworkManager.Singleton.IsHost) return;

        NetworkManager.Singleton.gameObject.GetComponent<FacepunchTransport>().targetSteamId = lobby.Owner.Id;
        NetworkManager.Singleton.StartClient();
    }

    private void OnLobbyCreated(Result result, Lobby lobby)
    {
        if(result == Result.OK)
        {
            lobby.SetPublic();
            lobby.SetJoinable(true); 
            
            _lobbyID.text = lobby.Id.ToString();
            SteamLobbySaver.instance.currentLobby = lobby;

            NetworkManager.Singleton.StartHost();
        }
    }

    public async void CreateLobby()
    {
        await SteamMatchmaking.CreateLobbyAsync(4);
    }

    public async void JoinLobby()
    {
        if(ulong.TryParse(_lobbyIDInputField.text, out ulong id))
        {
            await SteamMatchmaking.JoinLobbyAsync(id);
        }else
        {
            Debug.LogWarning("Formato del ID incorrecto");
        }
    }

    public void LeaveLobby()
    {
        SteamLobbySaver.instance.currentLobby?.Leave();
        SteamLobbySaver.instance.currentLobby = null;

        NetworkManager.Singleton.Shutdown();

        CheckUI();
    }

    private void CheckUI()
    {
        bool AmIInALobby = SteamLobbySaver.instance.currentLobby != null;
        
        _mainMenu.SetActive(!AmIInALobby);
        _inLobbyMenu.SetActive(AmIInALobby);
    }

    public void StartGameServer()
    {
        if (NetworkManager.Singleton.IsHost)
        {
            NetworkManager.Singleton.SceneManager.LoadScene("Gameplay", UnityEngine.SceneManagement.LoadSceneMode.Single);
        }
    }
}
