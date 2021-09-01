using System;
using System.Collections;
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
    }

    private void Start()
    {
        GameManager.OnGameFailed.AddListener(OnGameFailed);
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
        StopCoroutine(ChangeFOV());
        StartCoroutine(ChangeFOV());
    }

    private IEnumerator ChangeFOV()
    {
        var Direction = M_Camera.fieldOfView > M_FOV ? -1 : 1;
        while (Math.Abs(M_Camera.fieldOfView - M_FOV) > 0.1f)
        {
            M_Camera.fieldOfView += 1f * Direction;
            yield return null;
        }

        M_Camera.fieldOfView = M_FOV;
    }
}