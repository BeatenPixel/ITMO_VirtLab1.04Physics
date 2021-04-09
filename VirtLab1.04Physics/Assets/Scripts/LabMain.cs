using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LabMain : MonoBehaviour {

    public static LabMain inst;

    public LabState state;

    public CameraController cameraController;
    public DeviceController device;

    public LabUI ui;

    private void Awake() {
        inst = this;

        InitializeApplication();

        ui.InternalAwake();
    }

    private void Start() {
        InitializeLab();

        cameraController.InternalStart();
        device.InternalStart();

        ui.InternalStart();
    }

    private void Update() {
        if (state == LabState.RUNNING) {
            device.InternalUpdate();
        }

        cameraController.InternalUpdate();
        ui.InternalUpdate();

        if(Input.GetKeyDown(KeyCode.F)) {
            Screen.fullScreen = !Screen.fullScreen;
        }
    }

    private void LateUpdate() {
        cameraController.InternalLateUpdate();
    }

    private void InitializeLab() {
        state = LabState.READY;
    }

    public void StartLab() {
        if (state == LabState.PAUSE) {
            state = LabState.RUNNING;
        } else {
            state = LabState.RUNNING;
            device.StartDevice();
        }
    }

    public void StopLab() {
        state = LabState.PAUSE;
        device.StopDevice();
    }

    public void ResetLab() {
        state = LabState.READY;
        device.ResetDevice();
    }

    public void EndLab() {
        state = LabState.END;
        LabUI.inst.EndLab();
    }

    private void InitializeApplication() {
        Application.targetFrameRate = 60;
    }

}

public enum LabState {
    NONE,
    READY,
    RUNNING,
    PAUSE,
    END
}
