using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[DisallowMultipleComponent]
public class Dress : MonoBehaviour
{
    [SerializeField, HideInInspector] private DressAssigner _assigner;
    private PlayerController PlayerController => _assigner.dressCrontroller.playerController;

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

        _assigner = GetComponentInParent<DressAssigner>();

        if (_assigner == null) return;

        _assigner.Validate();

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
        StartCoroutine(SubscribeToPlayerEvent(true));
    }

    private void OnDisable()
    {
        SubscribeToPlayerEvent(false);
    }

    private void ChangeSprite(PlayerController.Pose pose, bool flipX)
    {
        _spriteRenderer.flipX = flipX;

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

    IEnumerator SubscribeToPlayerEvent(bool enable)
    {
        float pollingWaitTime = 0.1f;
        float timePassed = 0f;
        float timeOut = 5f;

        while (_assigner == null)
        {
            _assigner = GetComponentInParent<DressAssigner>();

            yield return new WaitForSecondsRealtime(pollingWaitTime);
            timePassed += pollingWaitTime;

            if (timePassed >= timeOut)
            {
                throw new NullReferenceException("DressAssigner not found");
            }
        }
        while (_assigner.dressCrontroller == null || _assigner.dressCrontroller.playerController == null)
        {
            yield return new WaitForSecondsRealtime(pollingWaitTime);
            timePassed += pollingWaitTime;

            if (timePassed >= timeOut)
            {
                throw new NullReferenceException("PlayerController not found");
            }
        }

        if (enable) PlayerController.OnPoseChanged += ChangeSprite;
        else PlayerController.OnPoseChanged -= ChangeSprite;
    }
}
