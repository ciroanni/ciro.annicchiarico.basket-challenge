using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SliderManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI sliderValueLabel;
    private Slider slider;

    void Awake()
    {
        slider = GetComponent<Slider>();
    }  

    private void OnEnable()
    {
        if (slider != null)
        {
            slider.onValueChanged.AddListener(HandleSliderChanged);
        }
    } 

    private void HandleSliderChanged(float value)
    {
        if (sliderValueLabel != null)
        {
            sliderValueLabel.text = value.ToString() + "s";
        }
    }
}
