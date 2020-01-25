using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DOTweenTest : MonoBehaviour {
    public Transform[] transforms;
    public float moveAmount;

    // Use this for initialization
    void Start() {
        foreach (Transform trans in transforms) {
            Sequence seq = DOTween.Sequence()
                .OnStart(() => {
                    Debug.Log("start");
                })
                .OnUpdate(() => {
                    Debug.Log("update");
                })
                .OnComplete(() => {
                    Debug.Log("complete");
                })
                .Append(trans.DOMoveY(moveAmount, 1))
                .Append(trans.DOMoveX(moveAmount, 1).SetRelative())
                .Join(trans.DORotate(new Vector3(90, 0, 0), 1).SetRelative())
                .Append(trans.DOMoveY(-moveAmount, 1).SetRelative())
                .Join(trans.DORotate(new Vector3(90, 0, 0), 1).SetRelative())
                .Append(trans.DOMoveX(-moveAmount, 1).SetRelative())
                .SetLoops(-1);
        }
    }

    // Update is called once per frame
    void Update() {

    }
}
