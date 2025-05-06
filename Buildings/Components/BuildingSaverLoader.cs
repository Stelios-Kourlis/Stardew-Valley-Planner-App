using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static BuildingData;
using Newtonsoft.Json;
#if UNITY_EDITOR
using UnityEditor;
#endif
using SFB;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using static Utility.ClassManager;
using TMPro;
using System.Diagnostics;
using static MapController;

// [RequireComponent(typeof(Building))]
public class BuildingSaverLoader : MonoBehaviour {
    public static BuildingSaverLoader Instance;

    public void Awake() {
        if (Instance == null) Instance = this;
        else Destroy(this);
    }

    [ContextMenu("Save Building")]
    public BuildingData SaveBuilding(Building building) {
        if (building.CurrentBuildingState != Building.BuildingState.PLACED) throw new Exception("Building is not placed, cannot save");

        List<ComponentData> extraData = new();
        foreach (BuildingComponent component in building.gameObject.GetComponents<BuildingComponent>()) {
            if (component.Save() != null) extraData.Add(component.Save());
        }

        return new(building.type, building.Base, extraData.ToArray()); ;
    }

    public void LoadBuilding(BuildingData data) {
        Building building = BuildingController.CreateNewBuildingGameObject(data.buildingType).GetComponent<Building>();
        if (building.PlaceBuilding(data.lowerLeftCorner)) {
            LoadSavedComponents(building, data);
        }
        else throw new Exception($"Failed to place building {data.buildingType}");

    }

    // public void LoadData() {
    //     LoadBuilding(buildingData);
    // }

    public void LoadSavedComponents(Building building, BuildingData buildingData) {
        foreach (ComponentData compData in buildingData.componentData.OrderBy(compData => ComponentData.loadPriority.IndexOf(compData.componentType))) {
            BuildingComponent component = building.gameObject.GetComponent(compData.componentType) as BuildingComponent;
            component.Load(compData);
        }
    }

    public List<BuildingData> SaveAllBuildings() {
        List<BuildingData> buildingData = new();
        foreach (Building building in BuildingController.buildings) {
            if (!building.gameObject.scene.name.ToString().Contains("Map Scene")) continue; //buildings inside other buildings are handled by EnterableBuildingComponent
            buildingData.Add(SaveBuilding(building));
        }
        return buildingData;
    }

    /// <summary>
    /// Save the current state of the buildings to a file
    /// </summary>
    /// <returns>true, if the user saved, false if the user cancelled</returns>
    public bool SaveToFile() {
        string defaultSavePath = PlayerPrefs.GetString("DefaultSavePath", Application.dataPath);
        string savePath = StandaloneFileBrowser.SaveFilePanel("Choose a save location", defaultSavePath, "Farm", "svp"); ;
        if (savePath != "") {
            string directoryPath = Path.GetDirectoryName(savePath);
            PlayerPrefs.SetString("DefaultSavePath", directoryPath);
            using StreamWriter writer = new(savePath);
            List<BuildingData> allData = SaveAllBuildings();
            JObject root = new() {
                { "Farm", MapController.Instance.CurrentMapType.ToString()  }
            };
            foreach (var data in allData) root.Add(allData.IndexOf(data).ToString(), data.ToJson());
            writer.Write(Regex.Unescape(root.ToString(Formatting.Indented)));
        }
        return savePath != "";
    }

    /// <summary>
    /// Load a farm from a file
    /// </summary>
    /// <returns>true, if the user chose a file, false if the user cancelled</returns>
    public bool LoadFromFile() {
        string defaultLoadPath = PlayerPrefs.GetString("DefaultLoadPath", Application.dataPath);
        var paths = StandaloneFileBrowser.OpenFilePanel("Open File", defaultLoadPath, new ExtensionFilter[] { new("Stardew Valley Planner Files", "svp") }, false);
        BuildingType currentType = BuildingController.currentBuildingType;
        if (paths.Length > 0) {
            StartCoroutine(BeginBuildingLoading(paths[0]));
        }
        BuildingController.SetCurrentBuildingType(currentType);
        return paths.Length > 0;
    }

    IEnumerator BeginBuildingLoading(string dirPath) {
        string directoryPath = Path.GetDirectoryName(dirPath);
        PlayerPrefs.SetString("DefaultLoadPath", directoryPath);
        using StreamReader reader = new(dirPath);
        string text = reader.ReadToEnd();
        JObject root = JObject.Parse(text);
        if (root["Farm"] != null)
            yield return MapController.Instance.SetMap((MapTypes)Enum.Parse(typeof(MapTypes), root["Farm"].ToString()));
        BuildingController.DeleteAllBuildings(true);
        BuildingController.IsLoadingSave = true;
        GameObject loadProgressPrefab = Resources.Load<GameObject>("UI/LoadProgress");
        GameObject loadProgress = Instantiate(loadProgressPrefab, GetCanvasGameObject().transform);
        RectTransform progressBarRectTransform = loadProgress.transform.Find("Bar").Find("BarFill").GetComponent<RectTransform>();
        progressBarRectTransform.sizeDelta = new Vector2(0, progressBarRectTransform.sizeDelta.y);
        int buildingCount = root.Properties().Count() - 1; // -1 because of the farm type
        int buildingsLoaded = 0;
        Stopwatch stopwatch = Stopwatch.StartNew();
        int targetFrameRate = 10; //fps
        long maxFrameTimeMs = 1000 / targetFrameRate;
        foreach (JProperty building in root.Properties()) {
            if (building.Name == "Farm") continue; //skip farm type
            BuildingData data = ParseBuildingFromJson(building);
            LoadBuilding(data);
            buildingsLoaded++;
            if (stopwatch.ElapsedMilliseconds >= maxFrameTimeMs) {
                stopwatch.Restart();
                float progress = (float)buildingsLoaded / buildingCount;
                loadProgress.transform.Find("ProgressText").GetComponent<TMP_Text>().text = $"{buildingsLoaded}/{buildingCount} ({progress * 100:F2}%)";
                progressBarRectTransform.sizeDelta = new Vector2(progress * 600, progressBarRectTransform.sizeDelta.y);
                yield return null;
            }
        }
        Destroy(loadProgress);
        BuildingController.IsLoadingSave = false;
    }

    public BuildingData ParseBuildingFromJson(JProperty jsonText) { //do it with JObjects

        static ComponentData ParseComponentFromJson(JProperty jsonText) {
            // Debug.Log($"Loading component: {jsonText}");
            Type componentType = Type.GetType(jsonText.Name);
            JObject componentData = new();
            foreach (JProperty item in jsonText.Value.Cast<JProperty>()) componentData.Add(item);
            return new(componentType, componentData);
        }

        // Debug.Log($"Loading building: {jsonText}");

        JObject buildingData = (JObject)jsonText.Value;
        string buildingName = buildingData["Building Type"].Value<string>();
        Vector3Int origin = new(buildingData["Origin"][0].Value<int>(), buildingData["Origin"][1].Value<int>());
        // int lowerLeftX = buildingData["Lower Left Corner X"].Value<int>();
        // int lowerLeftY = buildingData["Lower Left Corner Y"].Value<int>();

        List<ComponentData> componentData = new();
        foreach (var component in buildingData.Properties().Skip(2)) {
            componentData.Add(ParseComponentFromJson(component));
        }

        return new BuildingData((BuildingType)Enum.Parse(typeof(BuildingType), buildingName), origin, componentData.ToArray());

    }
}
