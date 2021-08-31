using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateWithPlayer : MonoBehaviour {
	private void Start() {
		PlayerController Player = FindObjectOfType<PlayerController>();
		if (!Player) {
			Debug.LogError("No player found. cannot proceed");
			Destroy(gameObject);
		}
		Player.OnPlayerRotate.AddListener(AfterPlayerRotate);
	}

	private void OnDestroy() {
		PlayerController Player = FindObjectOfType<PlayerController>();
		if(!Player) {
			return;
		}
		Player.OnPlayerRotate.RemoveListener(AfterPlayerRotate);
	}

	private void AfterPlayerRotate(Quaternion NextRotation) {
		transform.rotation = NextRotation;
	}
}