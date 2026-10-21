using System;
using Unity.Netcode;
using UnityEngine;

public class NewMonoBehaviourScript : NetworkBehaviour
{
    [SerializeField]
    private MultiplayerUI m_multiplayerUI;

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
