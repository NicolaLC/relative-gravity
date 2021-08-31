using System;
using UnityEngine;

public class MainCameraController : MonoBehaviour
{
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
    }

    private void Start()
    {
        GameManager.OnGameFailed.AddListener(OnGameFailed);
    }

    private void OnGameFailed()
    {
        M_Animator.Play("DeathAnimation");
    }
}
