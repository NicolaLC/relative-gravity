using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerUIController : MonoBehaviour
{
    [SerializeField, Header("View elements")]
    private Text StackText;
    [SerializeField]
    private Text LevelName;
    [SerializeField]
    private SkillSlotController[] Slots;

    private bool bCanUseAbilities = true;

    private void Awake()
    {
        LevelName.text = $"LEVEL: {SceneManager.GetActiveScene().name}";
    }

    private void Start()
    {
        bCanUseAbilities = GameManager.PlayerCanUseAbilities();
        ResourceManager.OnResourceUpdate.AddListener(AfterResourceUpdate);
        AfterResourceUpdate();
    }

    private void AfterResourceUpdate()
    {
        StackText.text = $"{ResourceManager.GetCurrentStack()}";
        foreach (var controller in Slots)
        {
            controller.Toggle(bCanUseAbilities && ResourceManager.GetCurrentStack() != 0);
        }
    }
}