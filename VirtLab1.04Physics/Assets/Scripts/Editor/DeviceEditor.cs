using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DeviceController))]
public class DeviceEditor : Editor {

    private DeviceController device;

    public override void OnInspectorGUI() {
        DrawDefaultInspector();
        device = (DeviceController)target;

        if(GUILayout.Button("Setup Rope")) {
            SetupRope();
        }
    }

    private void SetupRope() {
        //device.SetupRope(1f);
    }

}
