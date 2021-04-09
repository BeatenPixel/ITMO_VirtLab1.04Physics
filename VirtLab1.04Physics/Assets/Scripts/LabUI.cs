using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;

public class LabUI : MonoBehaviour {

    public static LabUI inst;

    public GameObject startButton;
    public GameObject stopButton;
    public GameObject resetButton;
    public GameObject freeCameraTip;

    public GameObject removeRunsButton;
    public GameObject copyRunsButton;
    public Image copyRunsImage;

    public TextMeshProUGUI startButtonText;
    public TextMeshProUGUI prevRunsText;

    public RectTransform controllsWindow;
    public TMP_Dropdown cameraModeDropdown;

    public ModifiedSlider massParameter;
    public ModifiedSlider radiusParameter;
    public ModifiedSlider heightParameter;

    public TextMeshProUGUI timeText;
    public Color greenColor;

    private bool controllsAreShown;
    private Vector2 controllsStartScale;
    private List<LabRunInfo> prevLabRuns = new List<LabRunInfo>();
    private int runsCount;

    internal void InternalAwake() {
        inst = this;
        controllsStartScale = controllsWindow.localScale;
        massParameter.OnValueChanged += OnMassChanged;
        radiusParameter.OnValueChanged += OnRadiusChanged;
        heightParameter.OnValueChanged += OnHeightChanged;
        removeRunsButton.SetActive(false);
        copyRunsButton.SetActive(false);
    }

    internal void InternalStart() {
        massParameter.InternalStart();
        radiusParameter.InternalStart();
        heightParameter.InternalStart();
    }

    internal void InternalUpdate() {
        if (LabMain.inst.state == LabState.RUNNING) {
            timeText.text = LabMain.inst.device.time.ToString("F2");
        }

        if (Input.GetKeyDown(KeyCode.P)) {
            WebGLCopyAndPasteAPI.PassCopyToBrowser("SLFDKJS:DFJ");
        }
    }

    public void SetCameraMode(int mode) {
        cameraModeDropdown.value = mode - 1;
    }

    public void ShowFreeCameraTip(bool show) {
        freeCameraTip.SetActive(show);
    }

    public void EndLab() {
        timeText.color = Color.white;
        startButtonText.text = "Старт";

        startButton.SetActive(true);
        stopButton.SetActive(false);

        massParameter.slider.enabled = true;
        radiusParameter.slider.enabled = true;
        heightParameter.slider.enabled = true;
        massParameter.inputField.enabled = true;
        radiusParameter.inputField.enabled = true;
        heightParameter.inputField.enabled = true;

        DeviceController device = LabMain.inst.device;
        LabRunInfo runInfo = new LabRunInfo() {
            mass = device.m,
            radius = device.r,
            h0 = device.h0,
            t = device.time,
            str = $"#{runsCount + 1} m={device.m} r={device.r} h0={device.h0} <color=#FF0000>t={device.time.ToString("F2")}</color>\n"
        };

        prevLabRuns.Add(runInfo);

        if (prevLabRuns.Count > 5) {
            prevLabRuns.RemoveAt(0);
        }

        string prevRunsTextStr = "";
        for (int i = prevLabRuns.Count - 1; i >= 0; i--) {
            prevRunsTextStr += prevLabRuns[i].str;
        }

        prevRunsText.text = prevRunsTextStr;
        runsCount++;

        removeRunsButton.SetActive(true);
        copyRunsButton.SetActive(true);
    }


    #region UIUserEvents

    public void OnRemoveRunsClick() {
        prevLabRuns.Clear();
        runsCount = 0;

        removeRunsButton.SetActive(false);
        copyRunsButton.SetActive(false);

        prevRunsText.text = "";
    }

    public void OnCopyRunsClick() {
        
        string copyStr = $"m\tr\th0\tt\n";        
        for (int i = prevLabRuns.Count-1; i >= 0; i--) {
            LabRunInfo runInfo = prevLabRuns[i];
            copyStr += $"{runInfo.mass}\t{ runInfo.radius}\t{ runInfo.h0}\t{ runInfo.t.ToString("F2")}\n";
        }

        GUIUtility.systemCopyBuffer = copyStr;

        DOTween.Kill(copyRunsImage, true);
        Color prevColor = copyRunsImage.color;
        copyRunsImage.color = greenColor;
        copyRunsImage.DOColor(prevColor, 0.7f);
    }

    public void OnStartButtonClick() {
        startButton.SetActive(false);
        stopButton.SetActive(true);

        LabMain.inst.StartLab();
        timeText.color = Color.red;

        massParameter.slider.enabled = false;
        radiusParameter.slider.enabled = false;
        heightParameter.slider.enabled = false;
        massParameter.inputField.enabled = false;
        radiusParameter.inputField.enabled = false;
        heightParameter.inputField.enabled = false;
    }

    public void OnStopButtonClick() {
        startButton.SetActive(true);
        stopButton.SetActive(false);

        LabMain.inst.StopLab();
        timeText.color = Color.white;
        startButtonText.text = "Продолжить";
    }

    public void OnResetButtonClick() {
        LabMain.inst.ResetLab();
        timeText.color = Color.white;
        timeText.text = "0.0";
        startButtonText.text = "Старт";
        startButton.SetActive(true);
        stopButton.SetActive(false);

        massParameter.slider.enabled = true;
        radiusParameter.slider.enabled = true;
        heightParameter.slider.enabled = true;
        massParameter.inputField.enabled = true;
        radiusParameter.inputField.enabled = true;
        heightParameter.inputField.enabled = true;
    }

    public void OnShowControllsButtonClick() {
        DOTween.Kill(controllsWindow, true);
        controllsAreShown = !controllsAreShown;

        if (controllsAreShown) {
            controllsWindow.localScale = controllsStartScale * 0.8f;
            controllsWindow.gameObject.SetActive(true);
            controllsWindow.DOScale(controllsStartScale, 0.2f).SetEase(Ease.OutBack);
        } else {
            controllsWindow.DOScale(controllsStartScale * 0.8f, 0.2f).SetEase(Ease.InBack).OnComplete(() => {
                controllsWindow.gameObject.SetActive(false);
            });
        }
    }

    public void OnChangeCameraModeClick(int value) {
        LabMain.inst.cameraController.SetCameraMode(value);
    }

    public void OnMassChanged(float v) {
        LabMain.inst.device.UpdateMass(v);
    }

    public void OnRadiusChanged(float v) {
        LabMain.inst.device.UpdateRadius(v, radiusParameter.normalizedValue);
    }

    public void OnHeightChanged(float v) {
        LabMain.inst.device.UpdateHeight(v);
    }

    #endregion

    public struct LabRunInfo {
        public float mass;
        public float radius;
        public float h0;
        public float t;
        public string str;
    }

}
