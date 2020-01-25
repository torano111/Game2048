using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Tile : MonoBehaviour {
    Image tileImage;
    Text tileText;
    GameManager instance;
    bool activeTile;

    [HideInInspector] public Tile mergedTile = null;
    public int point;

    Vector2Int lastTilePos = new Vector2Int(-1, -1);
    Vector2Int nextTilePos_;
    public Vector2Int nextPos {
        set {
            lastTilePos = nextTilePos_;
            nextTilePos_ = value;
        }
    }

    private void Awake() {
        point = 0;
        instance = GameManager.Instance;

        InitializeImageAndText();
    }

    void InitializeImageAndText() {
        tileImage = GetComponent<Image>();
        tileText = GetComponentInChildren<Text>();

        if (!tileImage) {
            Debug.LogAssertion("Tile Image not found");
        }

        if (!tileText) {
            Debug.LogAssertion("Tile Text not found");
        }
    }

    public void Merge(Tile tile) {
        mergedTile = tile;
    }

    public void MoveTo(Vector3 pos) {
        if (!activeTile)
            return;

        instance.animeCompleted = false;
        Sequence seq = DOTween.Sequence();
        seq.Append(transform.DOMove(pos, instance.animeDuration));

        // todo : this causes a bug such that somethimes tile transform.positon not changed after it is generated alghough TileInfo is correct
        /*
        // if the tile didn't move, don't animate it
        if (nextTilePos_ != lastTilePos && lastTilePos != new Vector2Int(-1, -1)) {
            // move animation with pos
            seq.Append(transform.DOMove(pos, instance.moveAnimDuration));
        }
        */

        if (mergedTile != null) {
            MoveAndDeactivate(pos);
        }

        seq.OnComplete(() => {
            ChangeColorAndPoint();
        });
    }

    void ChangeColorAndPoint() {
        tileText.text = point + "";
        // change color
        float max = Mathf.Log(instance.maxPoint, 2);
        float now = Mathf.Log(point, 2);
        float rate = now / max;
        Color color = Color.Lerp(instance.lowestTileColor, instance.highestTileColor, rate);
        tileImage.color = color;

        if (instance.debugging) {
            //Debug.Log("rate" + rate);
        }
    }

    void MoveAndDeactivate(Vector3 pos) {
        // set lower render order
        // move animation
        // deactivate
        Sequence seq = DOTween.Sequence()
            .Append(mergedTile.transform.DOMove(pos, instance.animeDuration))
            .OnComplete(() => {
                mergedTile.Deactivate();
                mergedTile = null;
            });
    }

    public void Activate(Vector3 pos) {
        activeTile = true;
        lastTilePos = new Vector2Int(-1, -1);
        transform.position = pos;

        //activate animation
        Sequence seq = DOTween.Sequence()
            .SetDelay(instance.animeDuration / 2.0f)
            .AppendCallback(() => {
                gameObject.SetActive(true);
                ChangeColorAndPoint();
            })
            .Append(transform.DOScale(0.5f, instance.animeDuration / 10.0f))
            .Append(transform.DOScale(1.2f, instance.animeDuration / 2.0f))
            .Append(transform.DOScale(1.0f, instance.animeDuration / 2.0f))
            .OnComplete(() => {
                instance.animeCompleted = true;
            });
    }

    public void Deactivate() {
        if (instance.debugging) {
            Debug.LogFormat("deactivate {0}", gameObject.name);
        }

        activeTile = false;
        gameObject.SetActive(false);
    }
}
