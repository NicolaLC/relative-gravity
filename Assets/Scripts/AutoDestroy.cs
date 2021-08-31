using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDestroy : MonoBehaviour
{
    [SerializeField] private float LifeTime = 1f;

    private void Start()
    {
        Invoke(nameof(DestroyMe), LifeTime);
    }

    private void DestroyMe()
    {
        Destroy(gameObject);
    }
}
