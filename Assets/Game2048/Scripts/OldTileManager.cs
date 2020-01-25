using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 4 x 4 Tile
/// 
///  0123 column
/// 0****
/// 1****
/// 2****
/// 3****
/// row
/// </summary>
public class OldTileManager : MonoBehaviour {
    [SerializeField] Image[] tileImages;
    Text[] tileTexts = new Text[16];

    public int[] tilePoints;
    List<int> emptyTileIndices = new List<int>();
    Vector3[][] tilePositions;

    private void OnEnable() {
        InputManager.OnUpKey += MoveUp;
        InputManager.OnDownKey += MoveDown;
        InputManager.OnRightKey += MoveRight;
        InputManager.OnLeftKey += MoveLeft;
    }

    private void OnDisable() {
        InputManager.OnUpKey -= MoveUp;
        InputManager.OnDownKey -= MoveDown;
        InputManager.OnRightKey -= MoveRight;
        InputManager.OnLeftKey -= MoveLeft;
    }

    private void Awake() {
        CheckInitialTile();
        tilePoints = new int[16];

        for (int i = 0; i < tilePoints.Length; i++) {
            tilePoints[i] = -1;
        }

        InitializeTile();
    }

    private void CheckInitialTile() {
        Text t;
        if (tileImages.Length != 16) {
            Debug.LogAssertion("There should be 16 Tile Image");
            return;
        }

        for (int i = 0; i < tileImages.Length; i++) {

            if (!tileImages[i]) {
                Debug.LogAssertion("Tile Image not set");
                return;
            }

            t = tileImages[i].GetComponentInChildren<Text>();
            if (!t) {
                Debug.LogAssertion("Tile Text not set");
                return;
            }

            tileTexts[i] = t;
            tileTexts[i].text = i + "";
        }

        Debug.Log("Tile Image and Text initialized");
    }

    void InitializeTile() {
        InitializeEmptyIndicesList();
        MakeRandomTile(2);
        UpdateAllTiles();
    }

    void InitializeEmptyIndicesList() {
        emptyTileIndices.Clear();

        for (int i = 0; i < tilePoints.Length; i++) {
            emptyTileIndices.Add(i);
        }
    }

    void MakeRandomTile(int newTileCount) {
        var count = newTileCount;

        while (count > 0 && emptyTileIndices.Count > 0) {
            var i = Random.Range(0, emptyTileIndices.Count);
            var index = emptyTileIndices[i];

            int point;

            if (GameManager.Instance.best <= 4) {
                point = 2;
            } else {
                float rand = Random.Range(0.0f, 100.0f);

                point = (rand < GameManager.Instance.chanceToGet4) ? 4 : 2;
            }

            Debug.LogFormat("{0} added", point);
            tilePoints[index] = point;
            UpdateTileAt(index);

            count--;
        }
    }

    void MoveTileUp(int columnNum) {
        int[] tileIndices = new int[4]; // value indices should be in order of calculation

        // if columnNum = 1, i = 1, 5, 9, 13
        for (int i = 0; i < 4; i++) {
            tileIndices[i] = columnNum + 4 * i;
        }

        CalculateTileValue(tileIndices);
    }

    void MoveTileDown(int columnNum) {
        int[] tileIndices = new int[4]; // value indices should be in order of calculation

        // if columnNum = 1, i = 13, 9, 5, 1
        for (int i = 0; i < 4; i++) {
            tileIndices[i] = 12 + columnNum - 4 * i;
        }


        CalculateTileValue(tileIndices);
    }

    void MoveTileRight(int rowNum) {
        int[] tileIndices = new int[4]; // value indices should be in order of calculation

        // if rowNum = 1, i = 7, 6, 5, 4
        for (int i = 0; i < 4; i++) {
            tileIndices[i] = 3 + rowNum * 4 - i * 1;
        }


        CalculateTileValue(tileIndices);
    }

    void MoveTileLeft(int rowNum) {
        int[] tileIndices = new int[4]; // value indices should be in order of calculation

        // if rowNum = 1, i = 4, 5, 6, 7
        for (int i = 0; i < 4; i++) {
            tileIndices[i] = rowNum * 4 + i * 1;
        }

        CalculateTileValue(tileIndices);
    }

    /// <summary>
    /// calculate and decide the next values and positions in a row or column
    /// </summary>
    /// <param name="tileIndices"> value indices should be in order of calculation </param>
    void CalculateTileValue(int[] tileIndices) {
        if (GameManager.Instance.debugging) {
            Debug.Log("calculating tile values");
        }

        int lastVal = -1, lastNonEmptyPos = tileIndices[0];
        bool merged = false;

        for (int i = 0; i < tileIndices.Length; i++) {
            var curPos = tileIndices[i];
            var curVal = tilePoints[curPos];

            if (curVal == -1) {
                continue;
            }

            if (lastVal == curVal && lastVal != -1 && curPos != lastNonEmptyPos) {

                GameManager.Instance.score += lastVal * 2;

                if (GameManager.Instance.best < lastVal * 2) {
                    GameManager.Instance.best = lastVal * 2;
                }

                Debug.LogFormat("best: {0}, score: {1}", GameManager.Instance.best, GameManager.Instance.score);

                merged = true;
                tilePoints[lastNonEmptyPos] = lastVal * 2;
                tilePoints[curPos] = -1;
                lastVal = -1;
                continue;
            }

            lastNonEmptyPos = curPos;
            lastVal = curVal;
        }

        bool moved = MoveTilePosition(tileIndices);
        if ((merged || moved) || Application.isEditor) {
            if (GameManager.Instance.debugging) {
                Debug.Log("updating tiles");
            }

            for (int i = 0; i < tileIndices.Length; i++) {
                UpdateTileAt(tileIndices[i]);
            }
        }
    }

    bool MoveTilePosition(int[] tileIndices) {
        var lastEmptyPos = -1;  // -1 if an empty position not found yet
        bool moved = false;

        for (int i = 0; i < tileIndices.Length; i++) {
            var curPos = tileIndices[i];

            if (tilePoints[curPos] == -1 && lastEmptyPos == -1) {
                lastEmptyPos = curPos;
            } else if (lastEmptyPos != -1 && tilePoints[curPos] != -1) {
                if (GameManager.Instance.debugging) {
                    Debug.Log("moving tile positions");
                }


                tilePoints[lastEmptyPos] = tilePoints[curPos];
                tilePoints[curPos] = -1;
                lastEmptyPos = -1;
                i = lastEmptyPos + 1;
                moved = true;
            }
        }

        return moved;
    }

    void UpdateTileAt(int index) {

        tileTexts[index].text = tilePoints[index] + "";

        if (tilePoints[index] != -1) {
            emptyTileIndices.Remove(index);
        }

        if (tilePoints[index] == -1 && !GameManager.Instance.debugging) {
            tileImages[index].enabled = false;
            tileTexts[index].enabled = false;
        } else {
            tileImages[index].enabled = true;
            tileTexts[index].enabled = true;
        }

        if (tileImages[index].enabled && GameManager.Instance.debugging) {
            tileTexts[index].text += "\n[" + index + "]";
        }
    }

    public void UpdateAllTiles() {
        for (int i = 0; i < tilePoints.Length; i++) {
            tileTexts[i].text = tilePoints[i] + "";

            if (GameManager.Instance.debugging) {
                tileTexts[i].text += "\n[" + i + "]";
            }
        }
    }

    void MoveUp() {
        if (GameManager.Instance.debugging) {
            Debug.Log("Up");
        }

        InitializeEmptyIndicesList();
        for (int i = 0; i < 4; i++) {
            MoveTileUp(i);
        }
        MakeRandomTile(1);
    }

    void MoveDown() {
        if (GameManager.Instance.debugging) {
            Debug.Log("Down");
        }

        InitializeEmptyIndicesList();
        for (int i = 0; i < 4; i++) {
            MoveTileDown(i);
        }
        MakeRandomTile(1);
    }

    void MoveRight() {
        if (GameManager.Instance.debugging) {
            Debug.Log("Right");
        }

        InitializeEmptyIndicesList();
        for (int i = 0; i < 4; i++) {
            MoveTileRight(i);
        }
        MakeRandomTile(1);
    }

    void MoveLeft() {
        if (GameManager.Instance.debugging) {
            Debug.Log("Left");
        }

        InitializeEmptyIndicesList();
        for (int i = 0; i < 4; i++) {
            MoveTileLeft(i);
        }
        MakeRandomTile(1);
    }
}
