using System;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour {
    public static Action OnUpKey;
    public static Action OnDownKey;
    public static Action OnRightKey;
    public static Action OnLeftKey;

    GameManager instance;

    private void Awake() {
        instance = GameManager.Instance;
    }

    // Update is called once per frame
    void Update() {
        if (instance.playing) {
            UpdateInput();
        }
    }

    void UpdateInput() {
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)) {
            if (OnUpKey != null)
                OnUpKey();
        } else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S)) {
            if (OnDownKey != null)
                OnDownKey();
        } else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D)) {
            if (OnRightKey != null)
                OnRightKey();
        } else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A)) {
            if (OnLeftKey != null)
                OnLeftKey();
        }
    }
}
