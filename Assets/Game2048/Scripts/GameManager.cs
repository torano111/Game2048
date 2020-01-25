using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(TileManager))]
[RequireComponent(typeof(InputManager))]
public class GameManager : SingletonMonoBehaviour<GameManager> {
    public bool debugging = false;
    public float chanceToGet4 = 25.0f;
    public float animeDuration = 0.1f;
    public int maxPoint = 2048;
    public Color lowestTileColor;
    public Color highestTileColor;

    [SerializeField] Slider rowSlider;
    [SerializeField] Slider columnSlider;
    [SerializeField] Button generateTileButton;
    [SerializeField] Button getTileStatsButton;
    [SerializeField] Button randomGenerateButton;

    [HideInInspector] public bool animeCompleted;
    [HideInInspector] public bool playing;
    [HideInInspector] public int score = 0;   // sum of all of the merged tiles
    [HideInInspector] public int best = 2;    // the highest tile

    TileManager tileManager;
    InputManager inputManager;
    int maxScore;

    private void Awake() {
        tileManager = GetComponent<TileManager>();
        inputManager = GetComponent<InputManager>();

        maxScore = (int.MaxValue / 2) * 2;
    }

    void Start() {
        InitGame();
    }

    void InitGame() {
        playing = true;

        if (debugging) {
            Debug.Log("Init Game");
            rowSlider.gameObject.SetActive(true);
            columnSlider.gameObject.SetActive(true);
            generateTileButton.gameObject.SetActive(true);
            getTileStatsButton.gameObject.SetActive(true);
            randomGenerateButton.gameObject.SetActive(true);
        } else {
            rowSlider.gameObject.SetActive(false);
            columnSlider.gameObject.SetActive(false);
            generateTileButton.gameObject.SetActive(false);
            getTileStatsButton.gameObject.SetActive(false);
            randomGenerateButton.gameObject.SetActive(false);
        }
    }

    public void EndGame() {
        playing = false;

        if (debugging) {
            Debug.Log("End Game");
        }
    }

    public void BeatGame() {
        if (debugging) {
            Debug.Log("Beat!");
        }
    }

    // Update is called once per frame
    void Update() {
        if (score >= maxScore || best >= maxPoint) {
            EndGame();
            BeatGame();
        }
    }

    public void GenerateNewTile() {
        var pos = new Vector2Int((int)rowSlider.value, (int)columnSlider.value);
        tileManager.MakeTileAt(pos);
    }

    public void GetTileStats() {
        var pos = new Vector2Int((int)rowSlider.value, (int)columnSlider.value);
        var stats = tileManager.GetTileStatsAt(pos);
        Debug.Log(stats);
    }

    public void GenerateRandomTile() {
        tileManager.MakeRandomTile(1);
    }
}
