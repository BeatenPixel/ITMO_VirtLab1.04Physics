using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ModifiedSlider : MonoBehaviour {

    public Slider slider;
    public TMP_InputField inputField;
    public float minValue = 0;
    public float maxValue = 1;
    public float step = 0.2f;
    public float value = 0;
    public int signsAfterComma = 1;
    public float normalizedValue {
        get {
            return (value - minValue) / (maxValue - minValue);
        }
    }

    public Action<float> OnValueChanged;
    private Color startTextColor;

    internal void InternalStart() {
        startTextColor = inputField.textComponent.color;
        slider.minValue = minValue;
        slider.maxValue = maxValue;
        slider.value = normalizedValue;
        if (OnValueChanged != null) {
            OnValueChanged(value);
        }
    }

    public void OnSliderValueChanged(float v) {
        value = minValue + Mathf.RoundToInt((v-minValue)/step)*step;
        inputField.text = value.ToString("F" + signsAfterComma);
        inputField.textComponent.color = startTextColor;
        slider.value = value;
        if (OnValueChanged != null) {
            OnValueChanged(value);
        }
    }

    public void OnInputFieldChanged(string str) {
        Debug.Log("Inpuchanged " + str);
        str = str.Replace(',', '.');
        float m;        

        if (float.TryParse(str, out m)) {
            inputField.textComponent.color = startTextColor;
            m = Mathf.Clamp(m, minValue, maxValue);
            value = m;
            slider.SetValueWithoutNotify(value);
            inputField.text = value.ToString("F" + signsAfterComma);
            if (OnValueChanged != null) {
                OnValueChanged(value);
            }
        } else {
            inputField.textComponent.color = Color.red;
            Debug.Log("error");
        }
    }

}
