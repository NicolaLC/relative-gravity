using System;
using UnityEngine;

public class WormHoleEnemyController : MonoBehaviour
{
    private Animator _Animator;
    private bool bIsFreezed = false;
    private void Awake()
    {
        _Animator = GetComponent<Animator>();
        if (!_Animator)
        {
            Debug.LogError("WormHoleEnemyController >> Cannot get animator reference");
            Destroy(this);
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (bIsFreezed) return;
        GameManager.GameFailed();
    }

    public void Freeze(float Duration)
    {
        _Animator.Play($"Freeze");
        bIsFreezed = true;
        Invoke(nameof(Unfreeze), Duration);
    }
    
    public void Unfreeze()
    {
        _Animator.Play($"Idle");
        bIsFreezed = false;
    }
}
