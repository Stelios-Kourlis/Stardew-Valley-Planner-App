using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using static Utility.ClassManager;

public class CameraController : MonoBehaviour {
    private Camera mainCamera;
    private Tilemap mapTilemap;
    private Slider scrollScaleSlider;
    private Slider moveScaleSlider;
    private BoundsInt tilemapBounds;
    //public float threshold = 0.1f;
    [SerializeField] private float moveScale = 6;
    private float scrollScale = 0.5f;
    private float storedScrollScale, storedMoveScale;
    private bool isMouseDown;
    private Vector3 oldMousePosition;
    // Start is called before the first frame update
    void Start() {
        isMouseDown = false;
        mainCamera = gameObject.GetComponent<Camera>();
        oldMousePosition = Input.mousePosition;
        UpdateCameraBounds();
        //scrollScaleSlider.onValueChanged.AddListener(newScrollScale => { if (scrollScale != 0) scrollScale = newScrollScale; });
        //moveScaleSlider.onValueChanged.AddListener(newMoveScale => { if (moveScale != 0) moveScale = newMoveScale; });
    }

    public void UpdateCameraBounds() {
        mapTilemap = GameObject.FindGameObjectWithTag("CurrentMap").GetComponent<Tilemap>();
        tilemapBounds = mapTilemap.cellBounds;
    }

    private void ClampCameraToBounds() {
        //UpdateCameraBounds();
        Vector3 cameraPosition = mainCamera.transform.position;

        float minX = tilemapBounds.min.x + Camera.main.orthographicSize * Camera.main.aspect;
        float maxX = tilemapBounds.max.x - Camera.main.orthographicSize * Camera.main.aspect;
        float minY = tilemapBounds.min.y + Camera.main.orthographicSize;
        float maxY = tilemapBounds.max.y - Camera.main.orthographicSize;

        float clampedX = Mathf.Clamp(cameraPosition.x, minX, maxX);
        float clampedY = Mathf.Clamp(cameraPosition.y, minY, maxY);

        mainCamera.transform.position = new Vector3(clampedX, clampedY, cameraPosition.z);
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetKeyDown(KeyCode.Mouse2)) {
            isMouseDown = true;
            oldMousePosition = Input.mousePosition;
        } else if (Input.GetKeyUp(KeyCode.Mouse2)) {
             isMouseDown = false;
        }

        if (isMouseDown) {
            // Calculate the difference in mouse position
            Vector3 mouseDelta = oldMousePosition - Input.mousePosition;
            mouseDelta.z = 0f;

            // Move the camera based on the mouse movement
            Vector3 movement = new Vector3(mouseDelta.x, mouseDelta.y, 0f) * moveScale * Time.deltaTime;
            transform.Translate(movement, Space.World);

            // Update the last mouse position
            oldMousePosition = Input.mousePosition;
        }

        ClampCameraToBounds();

        //Camera Size Control
        if (Input.mouseScrollDelta.y != 0f) {
            if (!EventSystem.current.IsPointerOverGameObject()) {
                float newCamSize = mainCamera.orthographicSize - Input.mouseScrollDelta.y * scrollScale;

                newCamSize = Mathf.Clamp(newCamSize, 6f, 22.28f); //Where did I even get these numbers from????
                mainCamera.orthographicSize = newCamSize;
            }
        }
    }

    public void SetScrollScale(int newScale) {
        scrollScale = newScale;
    }

    //Stop the camera from being moved and zoomed
    public void ToggleCameraLock() {
        if (moveScale == 0 && scrollScale == 0) { // if locked reverse lock
            moveScale = storedMoveScale;
            scrollScale = storedScrollScale;
        } else { //else store values and lock
            storedMoveScale = moveScale;
            storedScrollScale = scrollScale;
            moveScale = 0;
            scrollScale = 0;
        }
    }

    public void SetPosition(Vector3 position) {
        mainCamera.transform.position = position;
    }

    public Vector3 GetPosition() { return mainCamera.transform.position; }
}
