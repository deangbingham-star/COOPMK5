using System;
using Unity.Netcode;
using UnityEngine;

public abstract class PickableBase : NetworkBehaviour, IInteractable
{

    //public NetworkObject NetworkObject => throw new System.NotImplementedException();
    protected NetworkVariable<bool> m_isAvailable = new(
       true,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server);
    [SerializeField] private SelectionOutline m_outline;
    [SerializeField] private ObjectType m_ObjectType;

    public bool CanBePickedUp => m_isAvailable.Value;
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        m_isAvailable.OnValueChanged += OnAvailabilityChanged;
        ApplyAvailabilityState(m_isAvailable.Value);
    }

    public override void OnNetworkDespawn()
    {
        m_isAvailable.OnValueChanged -= OnAvailabilityChanged;
        base.OnNetworkDespawn();

    }

    private void OnAvailabilityChanged(bool previousValue, bool newValue)
    {
        ApplyAvailabilityState(newValue);
    }

    protected abstract void ApplyAvailabilityState(bool newvalue);

    public void PickUp()
    {
        if (IsServer == false)
        {
            return;
        }
        m_isAvailable.Value = false;
        OnPickedUp();
    }

    protected abstract void OnPickedUp();


    public void ToggleSelection(bool isSelected)
    {
        if (m_outline != null)
        {
            m_outline.ToggleOutline(isSelected);
        }
    }

}




// CanBePickedUp