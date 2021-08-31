using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public enum ECollectableType
{
    Gold,
    Silver,
    Bronze
}
public class CollectableController : MonoBehaviour
{
    [SerializeField, Header("Configuration")]
    private ECollectableType CollectableType = ECollectableType.Gold;

    [SerializeField] private GameObject SpawnOnDeath;
    
    [SerializeField, Header("Audio Settings")]
    private AudioClip CollectSound;
    
    private AudioSource M_AudioSource;

    private void Awake()
    {
        M_AudioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        ScoreManager.CollectableGained(CollectableType);
        M_AudioSource.PlayOneShot(CollectSound);
        Destroy(GetComponent<SpriteRenderer>());
        Destroy(GetComponent<BoxCollider2D>());

        Instantiate(SpawnOnDeath, transform.position, Quaternion.identity);
        Invoke(nameof(DestroyMe), 1f);
    }
    
    private void DestroyMe() {Destroy(gameObject);}
}
