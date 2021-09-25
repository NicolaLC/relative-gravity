using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class PlayerController : MonoBehaviour
{
    [SerializeField, Header("Children Settings")]
    private GameObject Attractor;

    [SerializeField]
    private GameObject BlueTrail;

    [SerializeField]
    private GameObject PurpleTrail;
    
    [SerializeField, Header("Ability Settings")]
    public bool CanUseAbility = true;
    
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
    private float M_Gravity = -8f;
    private Vector2 M_PhysicsDirection = Vector2.one;
    private AudioSource M_AudioSource;

    private SpriteRenderer M_SpriteRenderer;
    private bool M_IsSpriteFlipped = false;

    private float M_VerticalAcceleration = 0f;
    private bool M_GameEnded = false;

    private float M_WaitBeforeTakingInput = 0.25f;

    private Vector3 M_TargetPosition = Vector3.zero;
    private bool M_IsMovingToTargetPosition = false;

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
        if (GameManager.IsGamePaused())
        {
            return;
        }

        // wait a little delay
        if (M_WaitBeforeTakingInput > 0)
        {
            M_WaitBeforeTakingInput -= Time.deltaTime;
            return;
        }

        if (M_GameEnded || M_IsMovingToTargetPosition)
        {
            M_RigidBody.velocity = Vector2.zero;
            M_NextMovement = Vector3.zero;
            return;
        }

        CheckGround();

        // var HorizontalAxis = Input.GetAxisRaw("Horizontal");

        // update movement
        M_NextMovement.x = 0; // GetHorizontalMovement(HorizontalAxis);
        M_NextMovement.y = GetVerticalMovement();

        var HorizontalAxis = Input.GetAxisRaw("Horizontal");

        if (HorizontalAxis != 0)
        {
            M_IsSpriteFlipped = HorizontalAxis < 0;
            StartRotate(HorizontalAxis);
            M_SpriteRenderer.flipX = M_IsSpriteFlipped;
        }
    }

    private float GetVerticalMovement()
    {
        return GetGravityForce();
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
        M_TargetPosition = transform.position;
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
        if (GameManager.IsGamePaused())
        {
            return;
        }

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

    public void MoveOnRopePosition(Vector2 RopeLastPosition)
    {
        M_TargetPosition = RopeLastPosition;
        StartCoroutine(MoveToTargetPosition());
    }

    private IEnumerator MoveToTargetPosition()
    {
        M_IsMovingToTargetPosition = true;
        while (Vector3.Distance(transform.position, M_TargetPosition) > .5f)
        {
            transform.position = Vector3.Lerp(transform.position, M_TargetPosition, .05f);
            yield return null;
        }

        M_IsMovingToTargetPosition = false;
    }

    public void Stop()
    {
        M_NextMovement = Vector3.zero;
    }

    public void Attract(float Duration)
    {
        Attractor.SetActive(true);
        StartCoroutine(StopAttract(Duration));
    }

    private IEnumerator StopAttract(float Duration)
    {
        yield return new WaitForSeconds(Duration);
        Attractor.SetActive(false);
    }

    public void SlowGravity(float Duration)
    {
        M_Gravity = M_Gravity / 2.0f;
        M_VerticalAcceleration = 0f;
        BlueTrail.SetActive(false);
        PurpleTrail.SetActive(true);
        StartCoroutine(RestoreGravity(Duration));
    }

    private IEnumerator RestoreGravity(float Duration)
    {
        yield return new WaitForSeconds(Duration);
        BlueTrail.SetActive(true);
        PurpleTrail.SetActive(false);
        M_Gravity = M_Gravity * 2.0f;
    }
}