using System;
using System.Collections;
using System.Data;
using UnityEngine;
using UnityEngine.Events;

public class PlayerController : MonoBehaviour
{
    [SerializeField, Header("Movement Settings")]
    private float Speed = 10f;
    [SerializeField] private float JumpForce = 5f;
    [SerializeField] private LayerMask FloorLayerMask;
    [SerializeField] private Transform GroundCheckSpot;

    [SerializeField, Space(10), Header("Rotation Settings")]
    private float RotationSpeed = .1f;
    
    [SerializeField, Header("Audio Settings")]
    private AudioClip JumpClip;
    [SerializeField] private AudioClip DeathClip;

    public UnityEvent<Quaternion> OnPlayerRotate = new UnityEvent<Quaternion>();

    private Vector3 M_NextMovement;
    private Rigidbody2D M_RigidBody;
    private bool M_IsGrounded = true;
    private bool M_IsRotating = false;
    private Vector3 M_LocalScale = Vector3.one;
    private Quaternion M_TargetRotation;
    private float M_Gravity = -10f;
    private Vector2 M_PhysicsDirection = Vector2.one;
    private AudioSource M_AudioSource;

    private SpriteRenderer M_SpriteRenderer;
    private bool M_IsSpriteFlipped = false;

    private float M_VerticalAcceleration = 0f;
    private bool M_GameEnded = false;

    private void Awake()
    {
        M_RigidBody = GetComponent<Rigidbody2D>();
        if (!M_RigidBody)
        {
            throw new Exception("Player needs RigidBody");
        }

        M_TargetRotation = transform.rotation;
        M_LocalScale = transform.localScale;

        M_AudioSource = GetComponent<AudioSource>();
        M_SpriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        GameManager.OnGameFailed.AddListener(OnGameEnded);
    }

    private void OnGameEnded()
    {
        M_GameEnded = true;
        M_AudioSource.PlayOneShot(DeathClip);
    }

    private void Update()
    {
        if (M_GameEnded)
        {
            M_RigidBody.velocity = Vector2.zero;
            M_NextMovement = Vector3.zero;
            return;
        }
        
        CheckGround();

        var HorizontalAxis = Input.GetAxisRaw("Horizontal");

        // update movement
        M_NextMovement.x = 0; // GetHorizontalMovement(HorizontalAxis);
        M_NextMovement.y = GetVerticalMovement(Input.GetAxisRaw("Jump"));

        if (!M_AudioSource.isPlaying && Input.GetAxisRaw("Jump") > 0 && M_IsGrounded)
        {
            M_AudioSource.PlayOneShot(JumpClip);
        }

        if (HorizontalAxis != 0)
        {
            M_IsSpriteFlipped = HorizontalAxis < 0;
            M_SpriteRenderer.flipX = M_IsSpriteFlipped;
        }

        if (Input.GetAxisRaw("Fire1") != 0)
        {
            StartRotate(-1);
        }
        else if (Input.GetAxisRaw("Fire2") != 0)
        {
            StartRotate(1);
        }
    }

    private float GetVerticalMovement(float VerticalAxis)
    {
        return (VerticalAxis > 0) switch
        {
            true when M_IsGrounded => JumpForce,
            _ => GetGravityForce()
        };
    }

    private float GetGravityForce()
    {
        var mass = M_RigidBody.mass;
        return M_Gravity * (mass * mass) * M_VerticalAcceleration;
    }

    private float GetHorizontalMovement(float HorizontalAxis)
    {
        return HorizontalAxis * Speed;
    }

    private void StartRotate(float Direction)
    {
        if (M_IsRotating) return;
        M_IsGrounded = false;
        StartCoroutine(Rotate(Direction, RotationSpeed));
    }

    private IEnumerator Rotate(float Direction, float Duration)
    {
        M_IsRotating = true;

        var StartRotation = transform.rotation;
        var Angles = new Vector3(0, 0, 90f * Direction);
        M_TargetRotation = Quaternion.Euler(Angles) * transform.rotation;

        OnPlayerRotate.Invoke(M_TargetRotation); // notify others about rotation

        // @todo clean this
        UpdatePhysicDirection();
        print(M_TargetRotation.eulerAngles);

        for (float t = 0; t < Duration; t += Time.deltaTime)
        {
            transform.rotation = Quaternion.Lerp(StartRotation, M_TargetRotation, t / Duration);
            yield return null;
        }

        transform.rotation = M_TargetRotation;
        M_IsRotating = false;
    }

    private void UpdatePhysicDirection()
    {
        M_PhysicsDirection = new Vector2(
            M_TargetRotation.eulerAngles.z == 270f || M_TargetRotation.eulerAngles.z == 180f ? -1 : 1,
            M_TargetRotation.eulerAngles.z == 180f || M_TargetRotation.eulerAngles.z == 90f ? -1 : 1
        );
    }

    private void CheckGround()
    {
        M_IsGrounded = Physics2D.OverlapCircle(GroundCheckSpot.position, .1f, FloorLayerMask);
        if (M_IsGrounded)
        {
            M_VerticalAcceleration = 1;
        }
        else
        {
            M_VerticalAcceleration += Time.deltaTime / 10;
        }
    }

    private void FixedUpdate()
    {
        M_NextMovement.x *= M_PhysicsDirection.x;
        M_NextMovement.y *= M_PhysicsDirection.y;

        Vector3 NextMovement = M_NextMovement;

        // @todo clean this
        if (NeedToSwitchControls())
        {
            NextMovement.x = M_NextMovement.y;
            NextMovement.y = M_NextMovement.x;
            M_RigidBody.velocity = new Vector2(M_RigidBody.velocity.x, NextMovement.y * 100f * Time.fixedDeltaTime);
        }
        else
        {
            M_RigidBody.velocity = new Vector2(NextMovement.x * 100f * Time.fixedDeltaTime, M_RigidBody.velocity.y);
        }

        M_RigidBody.AddForce(NextMovement * 100f * Time.fixedDeltaTime, ForceMode2D.Force);
    }

    private bool NeedToSwitchControls()
    {
        return M_TargetRotation.eulerAngles.z == 90f || M_TargetRotation.eulerAngles.z == 270f;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(GroundCheckSpot.position, .1f);
    }

    private float GetOffsetDown()
    {
        return (M_LocalScale.y / 2f + 0.05f);
    }
}