using System;
using UnityEngine;

public class MainCameraController : MonoBehaviour
{
    [SerializeField, Header("FOV config")] private float DefaultFOV = 60f;
    [SerializeField] private float BigFOV = 80f;
    private float M_FOV = 0;

    private Camera M_Camera;
    private Animator M_Animator;

    private void Awake()
    {
        M_Camera = GetComponent<Camera>();
        if (!M_Camera)
        {
            throw new Exception("No camera found");
        }

        M_Animator = GetComponent<Animator>();
        if (!M_Animator)
        {
            throw new Exception("No animator found");
        }

        M_FOV = DefaultFOV;
        M_Camera.fieldOfView = M_FOV;
    }

    private void Start()
    {
        GameManager.OnGameFailed.AddListener(OnGameFailed);
    }

    private void Update()
    {
        if (Math.Abs(M_Camera.fieldOfView - M_FOV) > 0.1f)
        {
            M_Camera.fieldOfView = Mathf.Lerp(M_Camera.fieldOfView, M_FOV, .1f);
        }
    }

    private void OnGameFailed()
    {
        M_Animator.Play("DeathAnimation");
    }

    public void ZoomBig()
    {
        SetFOV(BigFOV);
    }

    public void ZoomDefault()
    {
        SetFOV(DefaultFOV);
    }

    private void SetFOV(float NextFOV)
    {
        M_FOV = NextFOV;
    }
}