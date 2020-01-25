using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(TileManager))]
[RequireComponent(typeof(InputManager))]
public class OldGameManager : SingletonMonoBehaviour<OldGameManager> {
    public bool debugging = false;
    public float chanceToGet4;

    [HideInInspector] public int score = 0;   // sum of all of the merged tiles
    [HideInInspector] public int best = 2;    // the highest tile

    TileManager tileManager;
    InputManager inputManager;

    private void Awake() {
        base.Awake();
        tileManager = GetComponent<TileManager>();
        inputManager = GetComponent<InputManager>();

        if (chanceToGet4 > 100.0f || chanceToGet4 < 0) {
            Debug.LogWarning("chanceToGet4 should be float from 0.0f to 100.0f");
            chanceToGet4 = 20.0f;
        }
    }

    // Use this for initialization
    void Start () {
        Debug.Log("game manager acrivated");
	}
	
	// Update is called once per frame
	void Update () {
	}
}
