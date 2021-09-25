using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
internal enum ButtonInputKey
{
    CONFIRM = 0,
    CANCEL = 1
}


public class ButtonController : MonoBehaviour
{
    private readonly Dictionary<ButtonInputKey, string> ButtonInputNameDict = new Dictionary<ButtonInputKey, string>
    {
        {ButtonInputKey.CONFIRM, "Confirm"},
        {ButtonInputKey.CANCEL, "Cancel"}
    };
    [SerializeField]
    private ButtonInputKey KeyToWait = ButtonInputKey.CONFIRM;
    private string AxisBinding = "Confirm";

    private void Awake()
    {
        if (ButtonInputNameDict.TryGetValue(KeyToWait, out AxisBinding)) return;
    }

    private void Update()
    {
        print(Input.GetAxisRaw(AxisBinding));
        if (Input.GetAxisRaw(AxisBinding) != 0)
        {
            GetComponent<Button>().onClick.Invoke();
        }
    }
}
