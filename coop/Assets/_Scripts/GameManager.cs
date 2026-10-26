using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NewMonoBehaviourScript : NetworkBehaviour
{
    [SerializeField]
    private MultiplayerUI m_multiplayerUI;
    [SerializeField]
    private GameObject m_playerPrefab;
    [SerializeField]
    private List<ResourcePallet> m_pallets;


    private void Start()
    {
        if (m_multiplayerUI != null)
        {
            m_multiplayerUI.OnStartHost += StartHost;
            m_multiplayerUI.OnStartHost += StartClient;
            m_multiplayerUI.OnStartHost += DisconnectClient;
        }
    }

    private void DisconnectClient()
    {
        m_multiplayerUI.EnableButtons();
        NetworkManager.Shutdown();
        Debug.Log("Disconnect Client");
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsServer == false)
        { return; }
        NetworkManager.OnClientConnectedCallback += SpawnPlayer;
        NetworkManager.SceneManager.OnLoadEventCompleted += HandleSceneLoadCompleted;
        foreach(ResourcePallet pallet in m_pallets)
        {
            pallet.OnPalletFilled += CheckWinCondition;
        }
    }

    private void CheckWinCondition()
    {
        int points = 0;
        foreach(ResourcePallet pallet in m_pallets)
        {
            points += pallet.StackedResoruces;

        }
        if(points >= m_pallets.Count * 3)
        {
            NetworkManager.SceneManager.LoadScene(SceneManager.GetActiveScene().name,LoadSceneMode.Single);
        }
    }

    private void HandleSceneLoadCompleted(string sceneName, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
    {
        foreach(ulong clientID in clientsCompleted)
        {
            SpawnPlayer(clientID);
        }
    }

    private void SpawnPlayer(ulong clientID)
    {
        if (NetworkManager.ConnectedClients[clientID].PlayerObject != null)
        { return; }
        GameObject player = Instantiate(m_playerPrefab);
        player.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientID, true);
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkSpawn();
        if (IsServer )
        {
            NetworkManager.OnClientConnectedCallback -= SpawnPlayer;
            NetworkManager.SceneManager.OnLoadEventCompleted -= HandleSceneLoadCompleted;
            foreach (ResourcePallet pallet in m_pallets)
            {
                pallet.OnPalletFilled -= CheckWinCondition;
            }
        }
        base.OnNetworkDespawn();
        
    }


    private void StartClient()
    {
        //m_multiplayerUI.DisableButtons();
        NetworkManager.StartClient();
        Debug.Log("Start Client");
    }

    private void StartHost()
    {
        //m_multiplayerUI.DisableButtons();
        NetworkManager.StartHost();
        Debug.Log("Start Host");
    }
}

//Stacked