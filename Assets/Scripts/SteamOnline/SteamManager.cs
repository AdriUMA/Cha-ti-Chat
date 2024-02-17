using Steamworks;
using Steamworks.Data;
using TMPro;
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
    }

    private void OnLobbyCreated(Result result, Lobby lobby)
    {
        if(result == Result.OK)
        {
            lobby.SetPublic();
            lobby.SetJoinable(true); 
            
            _lobbyID.text = lobby.Id.ToString();
            SteamLobbySaver.instance.currentLobby = lobby;
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

        CheckUI();
    }

    private void CheckUI()
    {
        bool AmIInALobby = SteamLobbySaver.instance.currentLobby != null;
        
        _mainMenu.SetActive(!AmIInALobby);
        _inLobbyMenu.SetActive(AmIInALobby);
    }
}
