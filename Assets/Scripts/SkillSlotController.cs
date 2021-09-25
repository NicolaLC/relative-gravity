using System;
using System.Collections;
using System.Collections.Generic;
using Core;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
internal enum SkillInputKey
{
    Q = 0,
    W = 1,
    E = 2,
    R = 4,
    NONE = 999
}

public class SkillSlotController : MonoBehaviour
{
    [SerializeField]
    private Image Background;

    [SerializeField]
    private Image Icon;

    [SerializeField]
    private Text Key;

    [SerializeField]
    private SkillInputKey KeyBinding;

    [SerializeField]
    private GameObject AbilityToCastPrefab;
    private BaseAbility AbilityToCast;
    private string AxisBinding = "Fire1";

    private readonly Dictionary<SkillInputKey, string> SkillInputNameDict = new Dictionary<SkillInputKey, string>
    {
        {SkillInputKey.Q, "Fire1"},
        {SkillInputKey.W, "Fire2"},
        {SkillInputKey.E, "Fire3"},
        {SkillInputKey.R, "Fire4"}
    };

    private bool bIsActive = true;

    private void Start()
    {
        AbilityToCast = Instantiate(AbilityToCastPrefab, Vector3.zero, Quaternion.identity).GetComponent<BaseAbility>();
        AbilityToCast.OnCooldownStart.AddListener((float Duration) =>
        {
            StartCoroutine(CooldownStart(Duration));
        });
        if (SkillInputNameDict.TryGetValue(KeyBinding, out AxisBinding)) return;
        Debug.LogWarning("SkillSlotController >> cannot get axis");
        Destroy(this);
    }

    private IEnumerator CooldownStart(float Duration)
    {
        Disable();
        yield return new WaitForSeconds(Duration - 0.1f);
        if (bIsActive)
        {
            Enable();
        }
    }

    private void Update()
    {
        if (KeyBinding == SkillInputKey.NONE)
        {
            return;
        }
        if (GameManager.IsGamePaused())
        {
            return;
        }
        if (!bIsActive)
        {
            return;
        }

        if (Input.GetAxisRaw(AxisBinding) != 0)
        {
            AbilityToCast.Execute();
        }
    }

    public void Toggle(bool Active)
    {
        if (Active == bIsActive) return; // nothing to do
        bIsActive = Active;

        if (Active)
        {
            Enable();
            return;
        }

        Disable();
    }

    private void Enable()
    {
        Background.color = Color.white;
        Icon.color = Color.white;
        Key.color = Color.white;
    }

    private void Disable()
    {
        Background.color = Color.gray;
        Icon.color = Color.gray;
        Key.color = Color.gray;
    }
}