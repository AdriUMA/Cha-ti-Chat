using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteLayerAssigner : MonoBehaviour
{
    [SerializeField] private int _sortingOrder;

    private List<SpriteRenderer> _spriteRenderers;

    void OnValidate()
    {
        // Get all sprite renderers in the hierarchy and set their sorting order
        _spriteRenderers = new List<SpriteRenderer>();
        GetComponentRecursively(transform, _spriteRenderers);
        foreach (var sprite in _spriteRenderers) sprite.sortingOrder = _sortingOrder;
    }

    private void GetComponentRecursively<T>(Transform parent, List<T> output)
    {
        // If the parent has the component, add it to the list
        if (TryGetComponent(out T component)) output.Add(component);

        // Loop through all children and call this function recursively
        for (int i = 0; i < transform.childCount; i++) GetComponentRecursively(transform.GetChild(i), output);
    }
}
