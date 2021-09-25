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
    
    [SerializeField]
    private int GainAmount = 10;

    [SerializeField] 
    private GameObject SpawnOnDeath;
    
    [SerializeField] 
    private float RespawnAfter = 0f;
    
    [SerializeField, Header("Audio Settings")]
    private AudioClip CollectSound;
    
    private AudioSource M_AudioSource;
    private Vector3 M_StartPos;

    private bool bReachingPlayer;
    private GameObject M_Player;

    private void Awake()
    {
        M_AudioSource = GetComponent<AudioSource>();
        M_StartPos = transform.position;
    }

    private void Start()
    {
        M_Player = GameObject.FindWithTag("Player");
    }

    private void Update()
    {
        if (!bReachingPlayer) return;
        transform.position = Vector3.Lerp(transform.position, M_Player.transform.position, .01f);
    }

    private void OnTriggerEnter2D(Collider2D Other)
    {
        if (Other.CompareTag("Attractor"))
        {
            bReachingPlayer = true;
            return;
        }
        if (!Other.CompareTag("Player")) return;
        
        ResourceManager.CollectableGained(GainAmount);
        M_AudioSource.PlayOneShot(CollectSound);


        Instantiate(SpawnOnDeath, transform.position, Quaternion.identity);
        if (RespawnAfter > 0.0f)
        {
            // @todo refine this
            GetComponent<SpriteRenderer>().enabled = false;
            GetComponent<BoxCollider2D>().enabled = false;
            Invoke(nameof(Respawn), RespawnAfter);
            return;
        }
        
        Destroy(GetComponent<SpriteRenderer>());
        Destroy(GetComponent<BoxCollider2D>());
        
        Invoke(nameof(DestroyMe), 1f);
    }

    private void DestroyMe()
    {
        Destroy(gameObject);
    }
    
    private void Respawn()
    {
        GetComponent<SpriteRenderer>().enabled = true;
        GetComponent<BoxCollider2D>().enabled = true;
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        GetComponent<Rigidbody2D>().angularVelocity = 0f;
        bReachingPlayer = false;
        transform.position = M_StartPos;
    }
}
