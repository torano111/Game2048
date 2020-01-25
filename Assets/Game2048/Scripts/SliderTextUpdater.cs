using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class SliderTextUpdater : MonoBehaviour {
    [SerializeField] Text text;
    Slider slider;

    private void Awake() {
        slider = GetComponent<Slider>();    
    }
    
    // Update is called once per frame
    void Update () {
        text.text = slider.value + "";
	}
}
