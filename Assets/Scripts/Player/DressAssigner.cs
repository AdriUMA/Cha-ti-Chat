using Steamworks;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

[DisallowMultipleComponent]
public class DressAssigner : MonoBehaviour
{
    public enum Identifier
    {
        HairAccesory,
        Earrings,
        Mask,
        Hair,
        BaseBodyHead,
        CloseArmAccessory,
        CloseArmSleeve,
        BaseBodyCloseArm,
        Necklace,
        OnePieceClothing,
        TShirt,
        Skirt,
        Shoes,
        Pants,
        Socks,
        BaseBodyLegs,
        BaseBodyChest,
        FarArmSleeve,
        FarArmAccessory,
        BaseBodyFarArm,
    }

    [SerializeField] public Identifier identifier;
    [SerializeField] private int _sortingOrder;
    [SerializeField, HideInInspector] private Dictionary<string, Dress> _dress;
    [SerializeField, HideInInspector] public DressController dressCrontroller;
    private Dress _currentDress = null;
    private bool _flipX = false;

    public void Start()
    {
        Validate();
    }

    public void Validate()
    {
        _dress = new Dictionary<string, Dress>();

        // Get all child dress
        var _childs = GetComponentsInChildren<Dress>();

        // Add all dress to the dictionary
        foreach (var dress in _childs) _dress.Add(dress.identifier, dress);

        // Set the sorting order to all dress
        UpdateSortingOrder(0, false);

        // Set the dress controller
        dressCrontroller = GetComponentInParent<DressController>();
    }

    /// <summary>
    /// Change the sorting order of the sprite dress
    /// </summary>
    /// <param name="offset"></param>
    /// <param name="updateOnlyActive">False will update deactivated dress (not recomended)</param>
    public void UpdateSortingOrder(int offset, bool updateOnlyActive = true)
    {
        // Get the index of this child in the player childrens
        for (int i = 0; i < transform.parent.childCount; i++)
        {
            if (transform.parent.GetChild(i).Equals(this.transform)) _sortingOrder = offset + (transform.parent.childCount - 1 - i);
        }

        if (updateOnlyActive)
        {
            _currentDress.SetLayer(_sortingOrder);
        }
        else
        {
            foreach (var dress in _dress.Values) dress.SetLayer(_sortingOrder);
        }
    }

    /// <summary>
    /// Change the activated dress
    /// </summary>
    /// <param name="newDressId"></param>
    public void ChangeDress(string newDressId)
    {
        if (_dress.TryGetValue(newDressId, out Dress dress))
        {
            // Deactivate the current active dress
            DeactivateDress();

            // Set the new active dress
            _currentDress = dress;
            _currentDress.gameObject.SetActive(true);
            _currentDress.FlipX(_flipX);
        }
        #if UNITY_EDITOR
        else
        {
            Debug.LogError("No dress found for " + newDressId + " ID. Availables: " + _dress.Keys.ToString());
        }
        #endif
    }

    /// <summary>
    /// Deactivate the current active dress
    /// </summary>
    public void DeactivateDress()
    {
        // If there is an active dress, deactivate it
        if (_currentDress != null) _currentDress.gameObject.SetActive(false);
    }

    /// <summary>
    /// Deactivate all dress
    /// </summary>
    public void DeactivateAllDress()
    {
        foreach (var dress in _dress.Values) dress.gameObject.SetActive(false);
    }

    /// <summary>
    /// Flip the X axis of the active dress
    /// </summary>
    /// <param name="enabled"></param>
    public void FlipX(bool enabled)
    { 
        _flipX = enabled;
        _currentDress?.FlipX(_flipX);
    }
}
