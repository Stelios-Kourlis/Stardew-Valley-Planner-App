using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static BuildingData;
using Newtonsoft.Json;
using UnityEditor;
using SFB;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;
using Newtonsoft.Json.Linq;

// [RequireComponent(typeof(Building))]
public class BuildingSaverLoader : MonoBehaviour {

    BuildingData buildingData;

    [ContextMenu("Save Building")]
    public BuildingData SaveBuilding() {
        if (!gameObject.TryGetComponent(out Building Building)) throw new Exception("Building component not found");
        if (Building.CurrentBuildingState != Building.BuildingState.PLACED) throw new Exception("Building is not placed, cannot save");

        List<ComponentData> extraData = new();
        foreach (BuildingComponent component in gameObject.GetComponents<BuildingComponent>()) {
            if (component.Save() != null) extraData.Add(component.Save());
        }
        buildingData = new(Building.GetType(), Building.BaseCoordinates[0], extraData.ToArray());
        return buildingData;
    }

    public void LoadBuilding(BuildingData data) {
        // if (!data.buildingType.IsAssignableFrom(typeof(Building))) throw new Exception($"Invalid building type {data.buildingType}");
        Building Building = gameObject.AddComponent(data.buildingType) as Building;
        Building.PlaceBuilding(data.lowerLeftCorner);
        buildingData = data;
        foreach (ComponentData compData in data.componentData) {
            BuildingComponent component = gameObject.GetComponent(compData.componentType) as BuildingComponent;
            component.Load(compData);
        }
    }

    [ContextMenu("Print Building Data (JSON)")]
    private void PrintBuildingDataJSON() {
        SaveBuilding();
        Debug.Log(buildingData.ToJson());
    }

    public static List<BuildingData> SaveAllBuildings() {
        List<BuildingData> buildingData = new();
        Debug.Log("Saving all buildings");
        Debug.Log($"Building count: {BuildingController.buildings.Count}");
        foreach (Building building in BuildingController.buildings) {
            Debug.Log($"Saving {building.BuildingName} (index: {BuildingController.buildings.IndexOf(building)})");
            if (building.TryGetComponent(out BuildingSaverLoader saverLoader)) buildingData.Add(saverLoader.SaveBuilding());
            else throw new Exception("BuildingSaverLoader component not found");
        }
        // foreach (BuildingData data in buildingData) Debug.Log(data.buildingType);
        return buildingData;
    }

    /// <summary>
    /// Save the current state of the buildings to a file
    /// </summary>
    /// <returns>true, if the user saved, false if the user cancelled</returns>
    [MenuItem("BuildingManager/Save To File")]
    public static bool SaveToFile() {
        string defaultSavePath = PlayerPrefs.GetString("DefaultSavePath", Application.dataPath);
        string savePath = StandaloneFileBrowser.SaveFilePanel("Choose a save location", defaultSavePath, "Farm", "svp"); ;
        if (savePath != "") {
            string directoryPath = Path.GetDirectoryName(savePath);
            PlayerPrefs.SetString("DefaultSavePath", directoryPath);
            using StreamWriter writer = new(savePath);
            List<BuildingData> allData = SaveAllBuildings();
            JObject root = new();
            foreach (var data in allData) root.Add(allData.IndexOf(data).ToString(), data.ToJson());
            writer.Write(root.ToString());
        }
        return savePath != "";
    }

    /// <summary>
    /// Load a farm from a file
    /// </summary>
    /// <returns>true, if the user chose a file, false if the user cancelled</returns>
    [MenuItem("BuildingManager/Load From File")]
    public static bool LoadFromFile() {
        string defaultLoadPath = PlayerPrefs.GetString("DefaultLoadPath", Application.dataPath);
        var paths = StandaloneFileBrowser.OpenFilePanel("Open File", defaultLoadPath, new ExtensionFilter[] { new("Stardew Valley Planner Files", "svp") }, false);
        Type currentType = BuildingController.currentBuildingType;
        if (paths.Length > 0) {
            string directoryPath = Path.GetDirectoryName(paths[0]);
            PlayerPrefs.SetString("DefaultLoadPath", directoryPath);
            using StreamReader reader = new(paths[0]);
            BuildingController.DeleteAllBuildings(true);
            BuildingController.IsLoadingSave = true;
            string text = reader.ReadToEnd();
            JObject root = JObject.Parse(text);
            foreach (JProperty building in root.Properties()) {
                BuildingData data = ParseBuildingFromJson(building);
                BuildingController.PlaceSavedBuilding(data);
            }
            BuildingController.IsLoadingSave = false;
        }
        BuildingController.SetCurrentBuildingType(currentType);
        return paths.Length > 0;
    }

    private static BuildingData ParseBuildingFromJson(JProperty jsonText) { //do it with JObjects

        static ComponentData ParseComponentFromJson(JProperty jsonText) {
            Debug.Log($"Loading component: {jsonText}");
            Type componentType = Type.GetType(jsonText.Name);
            Dictionary<string, string> componentData = new();
            foreach (JProperty item in jsonText.Value.Cast<JProperty>()) {
                componentData.Add(item.Name, item.Value.ToString());
            }
            return new(componentType, componentData);
        }

        Debug.Log($"Loading building: {jsonText}");

        JObject buildingData = (JObject)jsonText.Value;
        string buildingName = buildingData.Value<string>("Building Type");
        int lowerLeftX = buildingData.Value<int>("Lower Left Corner X");
        int lowerLeftY = buildingData.Value<int>("Lower Left Corner Y");

        List<ComponentData> componentData = new();
        foreach (var component in buildingData.Properties().Skip(3)) {
            componentData.Add(ParseComponentFromJson(component));
        }

        Debug.Log(new BuildingData(Type.GetType(buildingName), new Vector3Int(lowerLeftX, lowerLeftY, 0), componentData.ToArray()));
        return new BuildingData(Type.GetType(buildingName), new Vector3Int(lowerLeftX, lowerLeftY, 0), componentData.ToArray());

    }
}
