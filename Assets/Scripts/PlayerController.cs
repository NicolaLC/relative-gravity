using System;
using System.Collections;
using System.Data;
using UnityEngine;
using UnityEngine.Events;

public class PlayerController : MonoBehaviour
{
	[SerializeField, Header("Movement Settings")]
	private float Speed = 10f;

	[SerializeField]
	private float MaxHorizontalSpeed = 15f;

	[SerializeField]
	private float JumpForce = 5f;

	[SerializeField]
	private LayerMask FloorLayerMask;

	[SerializeField, Space(10), Header("Rotation Settings")]
	private float RotationSpeed = .1f;

	[SerializeField, Header("Audio Settings")]
	private AudioClip JumpClip;

	private Vector2 M_NextMovement;
	private Rigidbody2D M_RigidBody;
	private bool M_IsGrounded = true;
	private bool M_IsRotating = false;
	private Vector3 M_LocalScale = Vector3.one;
	private Quaternion M_TargetRotation;
	private float M_JumpCooldown = 0;
	private float M_Gravity = -9.81f;
	private Vector2 M_PhysicsDirection = Vector2.one;
	private AudioSource M_AudioSource;

	private SpriteRenderer M_SpriteRenderer;
	private bool M_IsSpriteFlipped = false;

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

	private void Update()
	{
		CheckGround();
		M_JumpCooldown--;

		var HorizontalAxis = Input.GetAxisRaw("Horizontal");
		var VerticalAxis = Input.GetAxisRaw("Vertical");
		var Jump = M_JumpCooldown <= 0 && M_IsGrounded && VerticalAxis > 0f;

		// update movement
		M_NextMovement.x = GetHorizontalMovement(HorizontalAxis, Jump);
		M_NextMovement.y = GetVerticalMovement(VerticalAxis);

		if (HorizontalAxis != 0)
		{
			M_IsSpriteFlipped = HorizontalAxis < 0;
			M_SpriteRenderer.flipX = M_IsSpriteFlipped;
		}

		if (!Jump) { return; }

		M_AudioSource.PlayOneShot(JumpClip);
		StartRotate(HorizontalAxis);
		M_JumpCooldown = 100;
		M_IsGrounded = false;
	}

	private float GetVerticalMovement(float VerticalAxis)
	{
		return VerticalAxis > 0 && M_IsGrounded ? VerticalAxis * JumpForce : M_Gravity;
	}

	private float GetHorizontalMovement(float HorizontalAxis, bool Jump)
	{
		return !Jump ? HorizontalAxis * Speed : 0;
	}

	private void StartRotate(float Direction)
	{
		if (M_IsRotating) return;
		StartCoroutine(Rotate(Direction, RotationSpeed));
	}

	private IEnumerator Rotate(float Direction, float Duration)
	{
		M_IsRotating = true;
		yield return new WaitForSeconds(0.25f);

		var StartRotation = transform.rotation;
		var Angles = new Vector3(0, 0, 90f * Direction);
		M_TargetRotation = Quaternion.Euler(Angles) * transform.rotation;

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
		M_IsGrounded = Physics2D.Raycast(
			transform.position, -transform.up,
			GetOffsetDown(),
			FloorLayerMask
		);
	}

	private void FixedUpdate()
	{
		if (M_RigidBody.velocity.x > MaxHorizontalSpeed)
		{
			M_NextMovement.x = 0;
		}

		M_NextMovement.x *= M_PhysicsDirection.x;
		M_NextMovement.y *= M_PhysicsDirection.y;

		Vector2 NextMovement = M_NextMovement;

		// @todo clean this
		if (NeedToSwitchControls())
		{
			NextMovement.x = M_NextMovement.y;
			NextMovement.y = M_NextMovement.x;
		}

		M_RigidBody.AddForce(NextMovement * Time.fixedDeltaTime, ForceMode2D.Impulse);
	}

	private bool NeedToSwitchControls()
	{
		return M_TargetRotation.eulerAngles.z == 90f || M_TargetRotation.eulerAngles.z == 270f;
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.green;
		var position = transform.position;
		Gizmos.DrawLine(position, position + transform.TransformDirection(Vector3.down) * GetOffsetDown());
	}

	private float GetOffsetDown()
	{
		return (M_LocalScale.y / 2f + 0.05f);
	}
}