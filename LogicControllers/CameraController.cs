using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using Utility;

public class CameraController : MonoBehaviour {

    public static CameraController Instance { get; private set; }
    private Camera mainCamera;
    private Tilemap mapTilemap;
    private Slider scrollScaleSlider;
    private Slider moveScaleSlider;
    private Bounds tilemapBounds;
    [SerializeField] private float moveScale = 6;
    private float scrollScale = 0.5f;
    private float storedScrollScale, storedMoveScale;
    private bool isMouseDown;
    private Vector3 oldMousePosition;
    public float blurMultiplier;

    void Start() {
        Instance = this;

        isMouseDown = false;
        mainCamera = gameObject.GetComponent<Camera>();
        oldMousePosition = Input.mousePosition;
        blurMultiplier = 5;

        if (PlayerPrefs.GetInt("FullScreen", 1) == 1) Screen.fullScreen = true;
        else Screen.fullScreen = false;
    }

    public void UpdateTilemapBounds() {
        if (BuildingController.CurrentTilemapTransform == null) return;

        mapTilemap = BuildingController.CurrentTilemapTransform.GetComponent<Tilemap>();
        mapTilemap.CompressBounds(); //awlays compress bounds
        // Debug.Log($"Updates tilemap bounds for {mapTilemap.name}");
        tilemapBounds = mapTilemap.localBounds;
    }

    private void ClampCameraToBounds() {
        float clampedX, clampedY;
        Vector3 cameraPosition = mainCamera.transform.position;
        if (tilemapBounds.Contains(cameraPosition)) return; //if the camera is inside the bounds, don't clamp it;
        // Debug.Log(tilemapBounds);

        if (BuildingController.isInsideBuilding.Key) { //keep only the camera center in bounds
            clampedX = Mathf.Clamp(cameraPosition.x, tilemapBounds.min.x, tilemapBounds.max.x);
            clampedY = Mathf.Clamp(cameraPosition.y, tilemapBounds.min.y, tilemapBounds.max.y);
        }
        else { //keep the whole camera within bounds
            // Calculate the camera's vertical size based on its orthographic size and aspect ratio
            float verticalSize = Camera.main.orthographicSize * 2;
            float horizontalSize = verticalSize * Camera.main.aspect;

            float minX = tilemapBounds.min.x + horizontalSize / 2;
            float maxX = tilemapBounds.max.x - horizontalSize / 2;
            float minY = tilemapBounds.min.y + verticalSize / 2;
            float maxY = tilemapBounds.max.y - verticalSize / 2;

            // Clamp the camera's position to the adjusted bounds
            clampedX = Mathf.Clamp(cameraPosition.x, minX, maxX);
            clampedY = Mathf.Clamp(cameraPosition.y, minY, maxY);


        }

        mainCamera.transform.position = new Vector3(clampedX, clampedY, cameraPosition.z);

        // if (Input.GetKey(KeyCode.C)) {
        //     mainCamera.transform.position = tilemapBounds.center;
        //     mainCamera.transform.position = new Vector3(mainCamera.transform.position.x, mainCamera.transform.position.y, -10);
        // }
        // Debug.Log(tilemapBounds.Contains(mainCamera.transform.position));
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetKeyDown(KeyCode.Mouse2)) {
            isMouseDown = true;
            oldMousePosition = Input.mousePosition;
        }
        else if (Input.GetKeyUp(KeyCode.Mouse2)) {
            isMouseDown = false;
        }

        if (isMouseDown) {
            // Calculate the difference in mouse position
            Vector3 mouseDelta = oldMousePosition - Input.mousePosition;
            mouseDelta.z = 0f;

            // Move the camera based on the mouse movement
            Vector3 movement = moveScale * Time.deltaTime * new Vector3(mouseDelta.x, mouseDelta.y, 0f);
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
        }
        else { //else store values and lock
            storedMoveScale = moveScale;
            storedScrollScale = scrollScale;
            moveScale = 0;
            scrollScale = 0;
        }
    }

    public void SetPosition(Vector3 position) {
        mainCamera.transform.position = new Vector3(position.x, position.y, -10);
    }

    public void SetPositionSmooth(Vector3 position, float totalTime) {
        position.z = -10;
        StartCoroutine(ObjectMover.MoveWorldObjectInConstantTime(transform, transform.position, position, totalTime));
    }

    public void SetSize(float size) {
        mainCamera.orthographicSize = size;
    }

    public Vector3 GetPosition() { return mainCamera.transform.position; }

    public float GetZoom() { return mainCamera.orthographicSize; }

    public void TakePictureOfMap() {
        TilemapManager.TakePictureOfMap();
    }

    /// <summary>
    /// Add a blur effect to the camera, the blur intensity is determined by the targetPanel's distance from the endPosition
    /// </summary>
    /// <param name="targetPanel">The panel that you want the blur to adjust to</param>
    // public IEnumerator BlurBasedOnDistance(GameObject targetPanel, Vector3 blurFocusPosition) { //todo reenable blurring
    //     PostProcessVolume volume = gameObject.GetComponent<PostProcessVolume>();
    //     volume.profile.TryGetSettings(out DepthOfField depthOfField);
    //     Vector3 cutOffPoint = new(targetPanel.transform.position.x, Screen.height + 500, 0);
    //     float maxDistance = Vector3.Distance(blurFocusPosition, cutOffPoint);

    //     while (targetPanel.GetComponent<IToggleablePanel>().IsMoving) {
    //         float currentDistance = Vector3.Distance(targetPanel.transform.position, blurFocusPosition);
    //         float normalizedDistance = Mathf.Clamp01(currentDistance / maxDistance);
    //         float blurValue = Mathf.Lerp(0.1f, 5f, normalizedDistance);
    //         // Debug.Log(blurValue);
    //         depthOfField.focusDistance.value = blurValue;
    //         yield return null;
    //     }
    // }

    public void ToggleFullscren() {
        Debug.Log("Toggling Fullscreen");
        Screen.fullScreen = !Screen.fullScreen;
        PlayerPrefs.SetInt("FullScreen", Screen.fullScreen ? 1 : 0);
    }
}
