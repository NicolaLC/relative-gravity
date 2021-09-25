using System;
using UnityEngine;

public class PopupController : MonoBehaviour
{
    private void Start()
    {
        GameManager.PauseGame();
    }

    private void OnDestroy()
    {
        GameManager.ResumeGame();
    }

    public void Hide()
    {
        Destroy(gameObject);
    }
}
