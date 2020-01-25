using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
/// 
public class TileManager : MonoBehaviour {
    [SerializeField] Tile[] tiles;

    GameManager instance;
    bool debugging { get { return instance.debugging; } }

    class TileInfo {
        public Vector2 transPos;    // transform.position of a TileInfo
        public Tile tile;
        public int point {
            set {
                if (tile != null) {
                    tile.point = value;
                }
            }
        }

        public void SetNextTilePos(Vector2Int pos) {
            tile.nextPos = pos;
        }

        public void Merge(TileInfo info) {
            tile.Merge(info.tile);
            info.tile = null;
        }

        public void Move() {
            tile.MoveTo(transPos);
        }

        public void Activate() {
            tile.Activate(transPos);
        }
    }
    TileInfo[,] tileInfo = new TileInfo[4, 4];  // TileInfo[row][column]

    List<Tile> inactiveTileList = new List<Tile>();
    List<Vector2Int> inactiveTileInfoPos = new List<Vector2Int>();
    bool merged, moved;

    public string GetTileStatsAt(Vector2Int pos) {
        var statsString = "";
        var posString = " at {" + pos.x + ", " + pos.y + "}";
        var info = tileInfo[pos.x, pos.y];

        if (info.tile != null) {
            var name = info.tile.gameObject.name;
            var point = info.tile.point + "";
            statsString += "name: " + name + posString + "\npoint: " + point;
        } else {
            statsString += "no tile" + posString;
        }

        return statsString;
    }

    private void OnEnable() {
        InputManager.OnUpKey += HandleUp;
        InputManager.OnDownKey += HandleDown;
        InputManager.OnRightKey += HandleRight;
        InputManager.OnLeftKey += HandleLeft;
    }

    private void OnDisable() {
        InputManager.OnUpKey -= HandleUp;
        InputManager.OnDownKey -= HandleDown;
        InputManager.OnRightKey -= HandleRight;
        InputManager.OnLeftKey -= HandleLeft;
    }

    void HandleUp() {
        merged = false;
        moved = false;

        if (debugging) {
            Debug.Log("Up");
        }

        for (int i = 0; i < 4; i++) {
            MoveTileUp(new Vector2Int(0, i));
        }

        if (moved || merged) {
            MakeRandomTile(1);
        }
    }

    void HandleDown() {
        merged = false;
        moved = false;
        if (debugging) {
            Debug.Log("Down");
        }


        for (int i = 0; i < 4; i++) {
            MoveTileDown(new Vector2Int(3, i));
        }
        if (moved || merged) {
            MakeRandomTile(1);
        }
    }

    void HandleRight() {
        merged = false;
        moved = false;
        if (debugging) {
            Debug.Log("Right");
        }

        for (int i = 0; i < 4; i++) {
            MoveTileRight(new Vector2Int(i, 3));
        }
        if (moved || merged) {
            MakeRandomTile(1);
        }
    }

    void HandleLeft() {
        merged = false;
        moved = false;
        if (debugging) {
            Debug.Log("Left");
        }

        for (int i = 0; i < 4; i++) {
            MoveTileLeft(new Vector2Int(i, 0));
        }
        if (moved || merged) {
            MakeRandomTile(1);
        }
    }

    void MoveTileUp(Vector2Int firstPos) {
        Vector2Int[] positions = new Vector2Int[4]; // value indices should be in order of calculation

        for (int i = 0; i < 4; i++) {
            positions[i] = firstPos + new Vector2Int(i, 0);
        }

        CalculateTileValue(positions);
    }

    void MoveTileDown(Vector2Int firstPos) {
        Vector2Int[] positions = new Vector2Int[4]; // value indices should be in order of calculation

        for (int i = 0; i < 4; i++) {
            positions[i] = firstPos + new Vector2Int(-i, 0);
        }

        CalculateTileValue(positions);
    }

    void MoveTileRight(Vector2Int firstPos) {
        Vector2Int[] positions = new Vector2Int[4]; // value indices should be in order of calculation

        for (int i = 0; i < 4; i++) {
            positions[i] = firstPos + new Vector2Int(0, -i);
        }

        CalculateTileValue(positions);
    }

    void MoveTileLeft(Vector2Int firstPos) {
        Vector2Int[] positions = new Vector2Int[4]; // value indices should be in order of calculation

        for (int i = 0; i < 4; i++) {
            positions[i] = firstPos + new Vector2Int(0, i);
        }

        CalculateTileValue(positions);
    }

    void CalculateTileValue(Vector2Int[] positions) {
        if (instance.debugging) {
            //Debug.Log("calculating tile values");
        }

        int lastVal = -1;
        Vector2Int lastNonEmptyPos = positions[0];

        for (int i = 0; i < positions.Length; i++) {
            var curPos = positions[i];
            var curTileInfo = tileInfo[positions[i].x, positions[i].y];
            var lastTileInfo = tileInfo[lastNonEmptyPos.x, lastNonEmptyPos.y];

            if (curTileInfo == null || lastTileInfo == null) {
                Debug.LogAssertion("tile is null");
                continue;
            }

            if (curTileInfo.tile == null) {
                continue;
            }

            var curVal = curTileInfo.tile.point;

            if (lastVal == curVal && lastTileInfo.tile != null && curPos != lastNonEmptyPos) {

                instance.score += lastVal * 2;

                if (instance.best < lastVal * 2) {
                    instance.best = lastVal * 2;
                }

                Debug.LogFormat("best: {0}, score: {1}", instance.best, instance.score);

                if (!inactiveTileInfoPos.Contains(curPos)) {
                    inactiveTileInfoPos.Add(curPos);
                }

                if (!inactiveTileList.Contains(curTileInfo.tile)) {
                    inactiveTileList.Add(curTileInfo.tile);
                }

                lastTileInfo.point = lastVal * 2;
                curTileInfo.point = -1;
                lastTileInfo.Merge(curTileInfo);
                lastTileInfo.SetNextTilePos(positions[i]);
                merged = true;

                lastVal = -1;
                continue;
            }

            lastNonEmptyPos = curPos;
            lastVal = curVal;
        }

        MoveTilePosition(positions);
    }

    void MoveTilePosition(Vector2Int[] positions) {
        if (instance.debugging) {
            //Debug.Log("calculating tile positions");
        }

        var lastEmptyPos = new Vector2Int(-1, -1);  // (-1, -1) if an empty position not found yet
        int lastEmptyIndex = -1;

        for (int i = 0; i < positions.Length; i++) {

            var curPos = positions[i];
            var curTileInfo = tileInfo[positions[i].x, positions[i].y];
            //var lastTileInfo = tileInfo[lastNonEmptyPos.x, lastNonEmptyPos.y];

            if (curTileInfo.tile == null) {
                if (lastEmptyPos == new Vector2Int(-1, -1)) {
                    lastEmptyPos = curPos;
                    lastEmptyIndex = i;
                }
            } else {
                if (lastEmptyPos != new Vector2Int(-1, -1)) {
                    if (instance.debugging) {
                        Debug.Log("moving tile positions");
                    }

                    var newInfo = tileInfo[lastEmptyPos.x, lastEmptyPos.y]; // tile = null
                    newInfo.tile = curTileInfo.tile;
                    curTileInfo.tile = null;
                    newInfo.SetNextTilePos(lastEmptyPos);
                    newInfo.Move();
                    moved = true;

                    if (!inactiveTileInfoPos.Contains(curPos)) {
                        inactiveTileInfoPos.Add(curPos);
                    }
                    if (inactiveTileInfoPos.Contains(lastEmptyPos)) {
                        inactiveTileInfoPos.Remove(lastEmptyPos);
                    }

                    lastEmptyPos = new Vector2Int(-1, -1);
                    i = lastEmptyIndex++;
                } else {
                    if (curTileInfo.tile.mergedTile != null) {
                        moved = true;
                    }

                    curTileInfo.Move(); // this is for when curTileInfo.tile doesn't move but there is a merged tile
                }
            }
        }
    }

    void CheckListCount() {
        var count = 0;

        foreach (Tile tile in tiles) {
            if (!tile.gameObject.active) {
                count++;
            }
        }

        if (inactiveTileInfoPos.Count != inactiveTileList.Count) {
            Debug.LogWarning("inactiveTileInfoPos.Count != inactiveTileList.Count");
        }

        if (inactiveTileList.Count != count) {
            Debug.LogWarning("inactiveTileList.Count != inactive Tile gameobject count");
        }
    }

    public void MakeRandomTile(int newTileCount) {

        var count = newTileCount;

        while (count > 0 && inactiveTileInfoPos.Count > 0 && inactiveTileList.Count > 0) {
            int i = Random.Range(0, inactiveTileInfoPos.Count);

            Vector2Int pos = inactiveTileInfoPos[i];
            MakeTileAt(pos);

            count--;
        }
    }

    public void MakeTileAt(Vector2Int pos) {
        if (tileInfo[pos.x, pos.y].tile != null) {
            Debug.LogWarning("can't make a tile where a tile already exists");
            return;
        }

        int point;

        if (instance.best <= 4) {
            point = 2;
        } else {
            float rand = Random.Range(0.0f, 100.0f);

            point = (rand < instance.chanceToGet4) ? 4 : 2;
        }

        var tile = inactiveTileList[0];
        inactiveTileList.RemoveAt(0);

        inactiveTileInfoPos.Remove(pos);
        TileInfo info = tileInfo[pos.x, pos.y];
        info.tile = tile;
        info.point = point;
        info.Activate();

        if (instance.debugging) {
            Debug.LogFormat("{0} added at {1}", info.tile.gameObject.name, pos);
        }

        if (inactiveTileInfoPos.Count == 0 && isGameOver()) {
            instance.EndGame();
        }
    }

    bool isGameOver() {
        for (int row = 0; row < tileInfo.GetLength(0); row++)
            for (int column = 0; column < tileInfo.GetLength(1); column++) {
                if (tileInfo[row, column].tile != null) {
                    if (column + 1 < tileInfo.GetLength(1)) {
                        if ((tileInfo[row, column + 1].tile != null && tileInfo[row, column + 1].tile.point == tileInfo[row, column].tile.point) || tileInfo[row, column + 1].tile == null) {
                            return false;
                        }
                    }

                    if (row + 1 < tileInfo.GetLength(0)) {
                        if ((tileInfo[row + 1, column].tile != null && tileInfo[row + 1, column].tile.point == tileInfo[row, column].tile.point) || tileInfo[row + 1, column].tile == null) {
                            return false;
                        }
                    }
                }
            }


        return true;
    }

    private void Awake() {
        instance = GameManager.Instance;
    }

    void InitializeTiles() {
        if (tiles.Length != 16) {
            Debug.LogAssertion("There should be 16 tiles");
        }

        for (int i = 0; i < tiles.Length; i++) {
            int row = i / 4;
            int column = i % 4;
            inactiveTileInfoPos.Add(new Vector2Int(row, column));
            inactiveTileList.Add(tiles[i]);
            tiles[i].Deactivate();

            tileInfo[row, column] = new TileInfo();
            TileInfo info = tileInfo[row, column];
            info.transPos = tiles[i].transform.position;    // initialize TileInfo position with the initial transform.position of Tile
        }

        if (!instance.debugging) {
            MakeRandomTile(2);
        }
    }

    // Use this for initialization
    void Start() {
        InitializeTiles();
    }

    // Update is called once per frame
    void Update() {

    }
}
