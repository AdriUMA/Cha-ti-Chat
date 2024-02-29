using System;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[DisallowMultipleComponent]
public class Dress : MonoBehaviour
{
    [SerializeField] public string identifier = "DRESS_ID";
    [SerializeField] private Sprite _idle;
    [SerializeField] private Sprite _walkLeftFoot;
    [SerializeField] private Sprite _walkRightFoot;
    [SerializeField, HideInInspector] private SpriteRenderer _spriteRenderer;

    private void OnValidate()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();

        // Set the idle sprite by default
        _spriteRenderer.sprite = _idle;

        var assigner = GetComponentInParent<DressAssigner>();

        if (assigner == null) return;

        assigner.OnValidate();

        // Check if the identifier already exist
        Dress[] dresses = transform.parent.GetComponentsInChildren<Dress>();
        foreach (Dress dress in dresses)
        {
            if(dress != this && dress.identifier == this.identifier)
            {
                Debug.LogError(identifier + " identifier already exist");
            }
        }
    }

    public void SetLayer(int layer) => _spriteRenderer.sortingOrder = layer;
    public void FlipX(bool enabled) => _spriteRenderer.flipX = enabled;

    private void OnEnable()
    {
        PlayerController.OnPoseChanged += ChangeSprite;
    }

    private void OnDisable()
    {
        PlayerController.OnPoseChanged -= ChangeSprite;
    }

    private void ChangeSprite(PlayerController.Pose pose)
    {
        switch (pose)
        {
            case PlayerController.Pose.Idle:
                _spriteRenderer.sprite = _idle;
                break;
            case PlayerController.Pose.WalkLeftFoot:
                _spriteRenderer.sprite = _walkLeftFoot;
                break;
            case PlayerController.Pose.WalkRightFoot:
                _spriteRenderer.sprite = _walkRightFoot;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(pose), pose, null);
        }
    }
}
