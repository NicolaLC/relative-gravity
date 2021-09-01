using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GrapplingHookController : MonoBehaviour
{
    public GameObject RopeHingeAnchor;
    public DistanceJoint2D RopeJoint;
    public Transform Crosshair;
    public SpriteRenderer CrosshairSprite;

    private bool RopeAttached;
    private Vector2 PlayerPosition;
    private Rigidbody2D RopeHingeAnchorRb;
    private SpriteRenderer RopeHingeAnchorSprite;

    public LineRenderer RopeRenderer;
    public LayerMask RopeLayerMask;
    private float RopeMaxCastDistance = 20f;
    private List<Vector2> RopePositions = new List<Vector2>();

    private bool DistanceSet;
    
    private Camera M_Camera;
    private MainCameraController M_CameraController;
    private PlayerController M_Player;
    private bool M_Grappling = false;

    private void Awake()
    {
        M_Camera = GetComponentInChildren<Camera>();
        M_CameraController = GetComponentInChildren<MainCameraController>();
        M_Player = GetComponent<PlayerController>();
        if (!M_CameraController)
        {
            throw new Exception("No camera controller set");
        }

        RopeJoint.enabled = false;
        PlayerPosition = transform.position;
        RopeHingeAnchorRb = RopeHingeAnchor.GetComponent<Rigidbody2D>();
        RopeHingeAnchorSprite = RopeHingeAnchor.GetComponent<SpriteRenderer>();
        CrosshairSprite.enabled = false;
    }

    private void Start()
    {
        GameManager.OnGameFailed.AddListener(() =>
        {
            ResetRope();
            Destroy(this);
        });
    }

    private void Update()
    {
        if (Input.GetButtonDown("MouseFire1"))
        {
            M_CameraController.ZoomBig();
            GameManager.SlowTime();
            M_Player.Stop();
            M_Grappling = true;
        }

        if (Input.GetButtonUp("MouseFire1"))
        {
            M_CameraController.ZoomDefault();
            GameManager.RestoreTime();
            M_Grappling = false;
            CrosshairSprite.enabled = false;
            if (RopeAttached)
            {
                M_Player.MoveOnRopePosition(RopePositions.Last());
            }
        }

        if (M_Grappling)
        {
            UpdateRope();
        }
        else
        {
            ResetRope();
        }
    }

    private void UpdateRope()
    {
        var worldMousePosition =
            M_Camera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y,
                -M_Camera.transform.position.z));
        var FacingDirection = worldMousePosition - transform.position;
        var AimAngle = Mathf.Atan2(FacingDirection.y, FacingDirection.x);
        if (AimAngle < 0f)
        {
            AimAngle = Mathf.PI * 2 + AimAngle;
        }

        var AimDirection = Quaternion.Euler(0, 0, AimAngle * Mathf.Rad2Deg) * Vector2.right;
        PlayerPosition = transform.position;
        SetCrosshairPosition(AimAngle);
        HandleInput(AimDirection);
        UpdateRopePositions();
    }

    private void SetCrosshairPosition(float AimAngle)
    {
        if (!CrosshairSprite.enabled)
        {
            CrosshairSprite.enabled = true;
        }

        var x = transform.position.x + 1f * Mathf.Cos(AimAngle);
        var y = transform.position.y + 1f * Mathf.Sin(AimAngle);

        var crossHairPosition = new Vector3(x, y, 0);
        Crosshair.transform.position = crossHairPosition;
    }

    // 1
    private void HandleInput(Vector2 AimDirection)
    {
        ResetRope();
        var hit = Physics2D.Raycast(PlayerPosition, AimDirection, RopeMaxCastDistance, RopeLayerMask);

        if (hit.collider == null) return;
        RopeRenderer.enabled = true;
        RopeAttached = true;
        if (RopePositions.Contains(hit.point)) return;
        // 4
        // Jump slightly to distance the player a little from the ground after grappling to something.
        RopePositions.Add(hit.point);
        RopeJoint.distance = Vector2.Distance(PlayerPosition, hit.point);
        RopeJoint.enabled = true;
        RopeHingeAnchorSprite.enabled = true;
    }

    private void ResetRope()
    {
        RopeJoint.enabled = false;
        RopeAttached = false;
        RopeRenderer.positionCount = 2;
        var position = transform.position;
        RopeRenderer.SetPosition(0, position);
        RopeRenderer.SetPosition(1, position);
        RopePositions.Clear();
        RopeHingeAnchorSprite.enabled = false;
    }

    private void UpdateRopePositions()
    {
        // 1
        if (!RopeAttached)
        {
            return;
        }

        // 2
        RopeRenderer.positionCount = RopePositions.Count + 1;

        // 3
        for (var i = RopeRenderer.positionCount - 1; i >= 0; i--)
        {
            if (i != RopeRenderer.positionCount - 1) // if not the Last point of line renderer
            {
                RopeRenderer.SetPosition(i, RopePositions[i]);

                // 4
                if (i == RopePositions.Count - 1 || RopePositions.Count == 1)
                {
                    var ropePosition = RopePositions[RopePositions.Count - 1];
                    if (RopePositions.Count == 1)
                    {
                        RopeHingeAnchorRb.transform.position = ropePosition;
                        if (DistanceSet) continue;
                        RopeJoint.distance = Vector2.Distance(transform.position, ropePosition);
                        DistanceSet = true;
                    }
                    else
                    {
                        RopeHingeAnchorRb.transform.position = ropePosition;
                        if (DistanceSet) continue;
                        RopeJoint.distance = Vector2.Distance(transform.position, ropePosition);
                        DistanceSet = true;
                    }
                }
                // 5
                else if (i - 1 == RopePositions.IndexOf(RopePositions.Last()))
                {
                    var ropePosition = RopePositions.Last();
                    RopeHingeAnchorRb.transform.position = ropePosition;
                    if (DistanceSet) continue;
                    RopeJoint.distance = Vector2.Distance(transform.position, ropePosition);
                    DistanceSet = true;
                }
            }
            else
            {
                RopeRenderer.SetPosition(i, transform.position);
            }
        }
    }
}