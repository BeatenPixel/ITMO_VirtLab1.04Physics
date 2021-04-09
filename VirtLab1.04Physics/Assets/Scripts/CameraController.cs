using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraController : MonoBehaviour {

    public LabCameraMode cameraMode;
    public List<CameraLocation> cameraLocations;
    private CameraLocation freeCameraLocation;

    public float moveSpeed = 7f;
    public float orthoMoveSpeed = 14f;
    public float moveSmooth = 10f;
    public float camZoomSpeed = 2f;
    public Transform camT;
    public Camera cam;

    public float sensitivity = 200f;
    public float maxAngleX = 80f;
    private float rotX;
    private float rotY;

    private Vector3 targetPos;
    private bool canMove;

    internal void InternalStart() {
        freeCameraLocation = cameraLocations[0];
        targetPos = camT.position;
        rotX = camT.rotation.eulerAngles.x;
        rotY = camT.rotation.eulerAngles.y;
        Cursor.lockState = CursorLockMode.None;
    }

    internal void InternalUpdate() {        
        Controlls();
    }

    internal void InternalLateUpdate() {
        if (canMove) {
            if (Cursor.lockState == CursorLockMode.Locked) {
                Look();
                Move();
            }

            if (cameraMode == LabCameraMode.FRONT) {
                MoveOrthographic();
            }
        }
    }

    private void Controlls() {
        if(Input.GetKeyDown(KeyCode.C)) {
            if(cameraMode == LabCameraMode.FREE) {
                canMove = !canMove;
                if (canMove) {
                    Cursor.lockState = CursorLockMode.Locked;
                    LabUI.inst.ShowFreeCameraTip(true);
                } else {
                    Cursor.lockState = CursorLockMode.None;
                    LabUI.inst.ShowFreeCameraTip(false);
                }
            } else {
                LabUI.inst.SetCameraMode((int)LabCameraMode.FREE);
            }            
        }
    }    

    private void ToggleLockCursor() {
        if (Cursor.lockState == CursorLockMode.Locked) {
            Cursor.lockState = CursorLockMode.None;
        } else {
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    private void Look() {
        float rx = -Input.GetAxis("Mouse Y") * sensitivity;
        float ry = Input.GetAxis("Mouse X") * sensitivity;

        rotY += ry * Time.deltaTime;

        rotX = Mathf.Clamp(rotX + rx * Time.deltaTime, -maxAngleX, maxAngleX);

        camT.rotation = Quaternion.Euler(0, rotY, 0) * Quaternion.Euler(rotX, 0, 0);
    }

    private void Move() {
        int inputQE = 0;
        if (Input.GetKey(KeyCode.Q)) {
            inputQE = -1;
        } else if (Input.GetKey(KeyCode.E)) {
            inputQE = 1;
        }

        Vector3 v = camT.forward * Input.GetAxisRaw("Vertical") + camT.right * Input.GetAxisRaw("Horizontal") + camT.up * inputQE;
        v = v.normalized * moveSpeed;

        targetPos += v * Time.deltaTime;
        camT.position = Vector3.Lerp(camT.position, targetPos, Time.deltaTime * moveSmooth);
    }

    private void MoveOrthographic() {
        float inputQE = Input.mouseScrollDelta.y;

        cam.orthographicSize = Mathf.Clamp(cam.orthographicSize + Time.deltaTime * camZoomSpeed * -inputQE, 0.5f, 3);

        Vector3 v = camT.up * Input.GetAxisRaw("Vertical") + camT.right * Input.GetAxisRaw("Horizontal");
        v = v.normalized * orthoMoveSpeed;

        targetPos += v * Time.deltaTime;
        camT.position = Vector3.Lerp(camT.position, targetPos, Time.deltaTime * moveSmooth);
    }

    public void SetCameraMode(int mode) {
        LabCameraMode prevMode = cameraMode;
        cameraMode = (LabCameraMode)(mode + 1);

        if(cameraMode == LabCameraMode.FREE) {
            Cursor.lockState = CursorLockMode.Locked;
            cam.orthographic = false;

            camT.position = freeCameraLocation.pos;
            camT.rotation = Quaternion.Euler(freeCameraLocation.rotation);

            targetPos = camT.position;
            rotX = camT.rotation.eulerAngles.x;
            rotY = camT.rotation.eulerAngles.y;

            LabUI.inst.ShowFreeCameraTip(true);
            EventSystem.current.SetSelectedGameObject(null);
            canMove = true;
        } else {           
            if(prevMode == LabCameraMode.FREE) {
                freeCameraLocation.pos = camT.position;
                freeCameraLocation.rotation = camT.rotation.eulerAngles;
                freeCameraLocation = cameraLocations[0];
            }

            Cursor.lockState = CursorLockMode.None;
            CameraLocation location = cameraLocations[mode - 1];
            camT.position = location.pos;
            camT.rotation = Quaternion.Euler(location.rotation);

            if (location.isOrthographic) {
                cam.orthographic = true;
                cam.orthographicSize = location.orthographicSize;
            } else {
                cam.orthographic = false;
            }

            LabUI.inst.ShowFreeCameraTip(false);

            if (cameraMode == LabCameraMode.FRONT) {
                canMove = true;
            } else {
                canMove = false;
            }
        }
    }

    public enum LabCameraMode {
        NONE,
        FREE,
        DEFAULT,
        FRONT,
        MAHOVIC,
        WEIGHTS
    }

    [System.Serializable]
    public struct CameraLocation {
        public Vector3 pos;
        public Vector3 rotation;
        public bool isOrthographic;
        public float orthographicSize;
    }

}
