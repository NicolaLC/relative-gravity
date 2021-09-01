using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private Camera M_Camera;
    private MainCameraController M_CameraController;
    private bool M_Grappling = false;

    private void Awake()
    {
        M_Camera = GetComponentInChildren<Camera>();
        M_CameraController = GetComponentInChildren<MainCameraController>();

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

    private void Update()
    {
        if (Input.GetButtonDown("Fire3"))
        {
            print("Grappling hook action start");
            M_CameraController.ZoomBig();
            GameManager.SlowTime();
            // GameManager.EnableCursor();
            M_Grappling = true;
        }

        if (Input.GetButton("Fire3"))
        {
            print("Grappling hook action continue");
        }

        if (Input.GetButtonUp("Fire3"))
        {
            print("Grappling hook action end");
            M_CameraController.ZoomDefault();
            GameManager.RestoreTime();
            // GameManager.DisableCursor();
            M_Grappling = false;
            CrosshairSprite.enabled = false;
        }

        if (M_Grappling)
        {
            UpdateRope();
        }
    }

    private void UpdateRope()
    {
        // 3

        var worldMousePosition =
            M_Camera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -M_Camera.transform.position.z));
        var FacingDirection = worldMousePosition - transform.position;
        print(worldMousePosition);
        var AimAngle = Mathf.Atan2(FacingDirection.y, FacingDirection.x);
        
        if (AimAngle < 0f)
        {
            AimAngle = Mathf.PI * 2 + AimAngle;
        }

        // 4
        var aimDirection = Quaternion.Euler(0, 0, AimAngle * Mathf.Rad2Deg) * Vector2.right;

        // 5
        PlayerPosition = transform.position;
        
        SetCrosshairPosition(AimAngle);
    }
    
    private void SetCrosshairPosition(float AimAngle)
    {
        if (!CrosshairSprite.enabled)
        {
            CrosshairSprite.enabled = true;
        }

        var x = transform.position.x + 3f * Mathf.Cos(AimAngle);
        var y = transform.position.y + 3f * Mathf.Sin(AimAngle);

        var crossHairPosition = new Vector3(x, y, 0);
        Crosshair.transform.position = crossHairPosition;
    }
}