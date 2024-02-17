using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using Steamworks;
using Steamworks.Data;
using System;

public class ChatManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField _chatInput;
    [SerializeField] private TextMeshProUGUI _messageTemplate;
    [SerializeField] private GameObject _messageContainer;
    [SerializeField] private InputAction _chatAction;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    private void OnEnable()
    {
        _chatAction.Enable();
        _chatAction.performed += ToggleChatBox;

        SteamMatchmaking.OnChatMessage += ChatMessageRecieved;
    }

    private void OnDisable()
    {
        _chatAction.Disable();
        _chatAction.performed -= ToggleChatBox;
    
        SteamMatchmaking.OnChatMessage -= ChatMessageRecieved;
    }

    private void ToggleChatBox(InputAction.CallbackContext ctx)
    {
        if(_chatInput.gameObject.activeSelf)
        {
            if(!string.IsNullOrEmpty(_chatInput.text))
            {
                SteamLobbySaver.instance.currentLobby?.SendChatString(_chatInput.text);
                _chatInput.text = "";
            }

            _chatInput.gameObject.SetActive(false);
            EventSystem.current.SetSelectedGameObject(null);
        }
        else
        {
            _chatInput.gameObject.SetActive(true);
            EventSystem.current.SetSelectedGameObject(_chatInput.gameObject);
        }
    }

    private void ChatMessageRecieved(Lobby lobby, Friend friend, string msg)
    {
        AddMessageToBox(friend.Name + ": " + msg);
    }

    private void AddMessageToBox(string msg)
    {
        GameObject message = Instantiate(_messageTemplate.gameObject, _messageContainer.transform);
        message.GetComponent<TextMeshProUGUI>().text = msg;
    }
}
