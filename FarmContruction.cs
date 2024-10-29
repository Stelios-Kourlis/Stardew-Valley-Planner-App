// #if UNITY_EDITOR
using System.Collections;
using System.IO;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

//This class is only to create the farm "timelapse" in readme
public class FarmContruction : MonoBehaviour {
    // Start is called before the first frame update
    void Start() {
        Debug.Log("Starting screnshot process");
        string path = "A:\\Home\\Game Related\\Stardew Valley Related\\SV Planner App/ShowcaseFinal.svp";
        using StreamReader reader = new(path);
        BuildingController.DeleteAllBuildings(true);
        BuildingController.IsLoadingSave = true;
        string text = reader.ReadToEnd();
        JObject root = JObject.Parse(text);
        int index = 0;
        foreach (JProperty building in root.Properties()) {
            BuildingData data = BuildingSaverLoader.Instance.ParseBuildingFromJson(building);
            BuildingController.PlaceSavedBuilding(data);
            TakeNumberedScreenshot(index);
            index++;
        }
        BuildingController.IsLoadingSave = false;
    }

    public static void TakeNumberedScreenshot(int number) {
        BuildingController.LastBuildingObjectCreated.SetActive(false);
        Vector3 originalPosition = Camera.main.transform.position;
        float originalSize = Camera.main.orthographicSize;

        // Get the bounds of the Tilemap
        var mapTilemap = GameObject.FindGameObjectWithTag("CurrentMap").GetComponent<Tilemap>();
        Bounds tilemapBounds = mapTilemap.localBounds;

        // Calculate the new camera position and size
        Vector3 newPosition = tilemapBounds.center;
        float newWidth = tilemapBounds.size.x;
        float newHeight = tilemapBounds.size.y;
        float aspectRatio = Camera.main.aspect;

        // Set the camera to cover the entire Tilemap
        Camera.main.transform.position = new Vector3(newPosition.x, newPosition.y, originalPosition.z);
        Camera.main.orthographicSize = (newWidth / aspectRatio > newHeight) ? newWidth / (2f * aspectRatio) : newHeight / 2f;

        // Create a new render texture
        RenderTexture renderTexture = new(Screen.width, Screen.height, 24);
        Camera.main.targetTexture = renderTexture;

        // Render the camera's view to the target texture
        Camera.main.Render();

        // Create a new texture and read the active RenderTexture into it
        Texture2D texture = new(Screen.width, Screen.height, TextureFormat.RGB24, false);
        RenderTexture.active = renderTexture;
        texture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        texture.Apply();

        // Convert the texture to a byte array
        byte[] bytes = texture.EncodeToPNG();

        // Write the byte array to a file
        string path = "A:\\Home\\Game Related\\Stardew Valley Related\\SV Planner App\\Screenshots";
        string fileName = $"Screenshot_{number}.png";
        string fullPath = Path.Combine(path, fileName);
        File.WriteAllBytes(fullPath, bytes);


        // Reset the camera and render texture
        Camera.main.targetTexture = null;
        RenderTexture.active = null;

        // Restore the original camera settings
        Camera.main.transform.position = originalPosition;
        Camera.main.orthographicSize = originalSize;


    }
}

// #endif