using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using Utility;

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
    public float blurMultiplier;

    void Start() {
        isMouseDown = false;
        mainCamera = gameObject.GetComponent<Camera>();
        oldMousePosition = Input.mousePosition;
        UpdateCameraBounds();
        blurMultiplier = 5;

        if (PlayerPrefs.GetInt("FullScreen", 1) == 1) Screen.fullScreen = true;
        else Screen.fullScreen = false;
        //scrollScaleSlider.onValueChanged.AddListener(newScrollScale => { if (scrollScale != 0) scrollScale = newScrollScale; });
        //moveScaleSlider.onValueChanged.AddListener(newMoveScale => { if (moveScale != 0) moveScale = newMoveScale; });
    }

    public void UpdateCameraBounds() {
        if (!BuildingController.isInsideBuilding.Key) {
            mapTilemap = GameObject.FindGameObjectWithTag("CurrentMap").GetComponent<Tilemap>();
            tilemapBounds = mapTilemap.cellBounds;
        }
        else {
            mapTilemap = BuildingController.isInsideBuilding.Value.GetComponent<Tilemap>();
            tilemapBounds = mapTilemap.cellBounds;

            // // Increase the bounds by 1 tile in each direction //todo maybe increase tilemap by 1 tile each dir?
            // tilemapBounds = new BoundsInt(
            //     originalBounds.xMin - 1, // Decrease xMin by 1
            //     originalBounds.yMin - 1, // Decrease yMin by 1
            //     originalBounds.zMin, // Z remains unchanged
            //     originalBounds.size.x + 2, // Increase width by 2 (1 tile on each side)
            //     originalBounds.size.y + 2, // Increase height by 2 (1 tile on each side)
            //     originalBounds.size.z // Depth remains unchanged
            // );
        }
    }

    private void ClampCameraToBounds() {
        UpdateCameraBounds();
        Vector3 cameraPosition = mainCamera.transform.position;

        // Calculate the camera's vertical size based on its orthographic size and aspect ratio
        float verticalSize = Camera.main.orthographicSize * 2;
        float horizontalSize = verticalSize * Camera.main.aspect;

        // Calculate the center of the tilemap
        Vector3 tilemapCenter = mapTilemap.cellBounds.center;

        // Calculate tilemap size
        float tilemapWidth = tilemapBounds.size.x;
        float tilemapHeight = tilemapBounds.size.y;

        // Check if the tilemap is smaller than the camera's view
        if (tilemapWidth <= horizontalSize && tilemapHeight <= verticalSize) {
            // If smaller, lock the camera to the center of the tilemap
            mainCamera.transform.position = new Vector3(tilemapCenter.x, tilemapCenter.y, cameraPosition.z);
        }
        else {
            // Adjust bounds to account for the camera's size
            float minX = tilemapBounds.min.x + horizontalSize / 2;
            float maxX = tilemapBounds.max.x - horizontalSize / 2;
            float minY = tilemapBounds.min.y + verticalSize / 2;
            float maxY = tilemapBounds.max.y - verticalSize / 2;

            // Clamp the camera's position to the adjusted bounds
            float clampedX = Mathf.Clamp(cameraPosition.x, minX, maxX);
            float clampedY = Mathf.Clamp(cameraPosition.y, minY, maxY);

            // Update the camera's position
            mainCamera.transform.position = new Vector3(clampedX, clampedY, cameraPosition.z);
        }
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
        mainCamera.transform.position = new Vector3(position.x, position.y, mainCamera.transform.position.z);
    }

    public void SetSize(float size) {
        mainCamera.orthographicSize = size;
    }

    public Vector3 GetPosition() { return mainCamera.transform.position; }

    public void TakePictureOfMap() {
        TilemapManager.TakePictureOfMap();
    }

    /// <summary>
    /// Add a blur effect to the camera, the blur intensity is determined by the targetPanel's distance from the endPosition
    /// </summary>
    /// <param name="targetPanel">The panel that you want the blur to adjust to</param>
    public IEnumerator BlurBasedOnDistance(GameObject targetPanel, Vector3 blurFocusPosition) {
        PostProcessVolume volume = gameObject.GetComponent<PostProcessVolume>();
        volume.profile.TryGetSettings(out DepthOfField depthOfField);
        Vector3 cutOffPoint = new(targetPanel.transform.position.x, Screen.height + 500, 0);
        float maxDistance = Vector3.Distance(blurFocusPosition, cutOffPoint);

        while (targetPanel.GetComponent<IToggleablePanel>().IsMoving) {
            float currentDistance = Vector3.Distance(targetPanel.transform.position, blurFocusPosition);
            float normalizedDistance = Mathf.Clamp01(currentDistance / maxDistance);
            float blurValue = Mathf.Lerp(0.1f, 5f, normalizedDistance);
            // Debug.Log(blurValue);
            depthOfField.focusDistance.value = blurValue;
            yield return null;
        }
    }

    public void ToggleFullscren() {
        Debug.Log("Toggling Fullscreen");
        Screen.fullScreen = !Screen.fullScreen;
        PlayerPrefs.SetInt("FullScreen", Screen.fullScreen ? 1 : 0);
    }
}
