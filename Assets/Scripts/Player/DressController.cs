using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class DressController : MonoBehaviour
{
    [SerializeField] private Dictionary<DressAssigner.Identifier, DressAssigner> _assigners;
    [SerializeField, HideInInspector] public PlayerController playerController;

    private void OnValidate()
    {
        playerController = GetComponent<PlayerController>();
    }

    private void Start()
    {
        StartCoroutine(WaitForNetworkInstantiate());
    }

    IEnumerator WaitForNetworkInstantiate()
    {
        if (NetworkManager.Singleton == null) yield return new WaitUntil(() => NetworkManager.Singleton != null);
        if (GetComponentInParent<DressController>() == null) yield return new WaitUntil(() => GetComponentInParent<DressController>() != null);
        if (GetComponentInParent<PlayerController>() == null) yield return new WaitUntil(() => GetComponentInParent<PlayerController>() != null);

        yield return new WaitForSecondsRealtime(0.5f);

        // Initialize the dictionary
        _assigners = new Dictionary<DressAssigner.Identifier, DressAssigner>();

        // Get all sprite layer assigners in the hierarchy
        var assigners = GetComponentsInChildren<DressAssigner>();

        // Add all assigners to the dictionary & deactivate all dress
        foreach (DressAssigner assigner in assigners)
        {
            _assigners.Add(assigner.identifier, assigner);
            assigner.DeactivateAllDress();
        }

        SetDress(DressAssigner.Identifier.BaseBodyHead, "Angustias");
        SetDress(DressAssigner.Identifier.BaseBodyChest, "Angustias");
        SetDress(DressAssigner.Identifier.BaseBodyCloseArm, "Angustias");
        SetDress(DressAssigner.Identifier.BaseBodyFarArm, "Angustias");
        SetDress(DressAssigner.Identifier.BaseBodyLegs, "Angustias");

        SetDress(DressAssigner.Identifier.FarArmAccessory, "Bracelet");
        SetDress(DressAssigner.Identifier.CloseArmAccessory, "Bracelet");

        //SetDress(DressAssigner.Identifier.TShirt, "Umungus");
        //SetDress(DressAssigner.Identifier.FarArmSleeve, "Umungus");
        //SetDress(DressAssigner.Identifier.CloseArmSleeve, "Umungus");

        //TODO
        SetDress(DressAssigner.Identifier.OnePieceClothing, "UglyPink");
        //SetDress(DressAssigner.Identifier.Pants, "Blue");
        //SetDress(DressAssigner.Identifier.Skirt, "Pink");

        SetDress(DressAssigner.Identifier.Socks, "ShortWhite");
        SetDress(DressAssigner.Identifier.Shoes, "Andalias");

        SetDress(DressAssigner.Identifier.Necklace, "Star");
        SetDress(DressAssigner.Identifier.Hair, "UglyHair");
        //SetDress(DressAssigner.Identifier.Mask, "Doramion");
        SetDress(DressAssigner.Identifier.Earrings, "Heart");
        SetDress(DressAssigner.Identifier.HairAccesory, "Ribbons");
    }

    /// <summary>
    /// Set dress in the assigners
    /// </summary>
    /// <param name="assignerId"></param>
    /// <param name="dressId"></param>
    public void SetDress(DressAssigner.Identifier assignerId, string dressId)
    {
        // If the assigner exists, update the activated controller
        if (_assigners.TryGetValue(assignerId, out DressAssigner assigner)) assigner.ChangeDress(dressId);
        
        #if UNITY_EDITOR
        else Debug.LogError($"Assigner {assignerId} not found");
        #endif
    }

    /// <summary>
    /// Deactivate dress in the assigners
    /// </summary>
    /// <param name="assignerId"></param>
    public void DeactivateDress(DressAssigner.Identifier assignerId, bool onlyCurrent = true)
    {
        // If the assigner exists, update the dress
        if (_assigners.TryGetValue(assignerId, out DressAssigner assigner))
        {
            if (onlyCurrent) assigner.DeactivateDress();
            else assigner.DeactivateAllDress();
        }
    }

    /// <summary>
    /// Flip dress in the assigners
    /// </summary>
    /// <param name="enabled"></param>
    public void FlipX(bool enabled)
    {
        foreach (var assigner in _assigners.Values) assigner.FlipX(enabled);
    }
}
