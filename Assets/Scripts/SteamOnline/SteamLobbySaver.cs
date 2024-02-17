using Steamworks.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteamLobbySaver : MonoBehaviour
{
    public static SteamLobbySaver instance;
    public Lobby? currentLobby;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(this);
        }
    }
}
