using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using static BuildingData;

public class CaveComponent : BuildingComponent {
    private bool hasMushroomBoxes;

    public void ToggleMushroomBoxes() {
        UndoRedoController.AddActionToLog(new MushroomCaveMushroomChangeRecord(this));
        if (hasMushroomBoxes) RemoveMushroomBoxes();
        else AddMushroomBoxes();
    }

    private void AddMushroomBoxes() {
        BuildingScriptableObject mushroomBox = Resources.Load<BuildingScriptableObject>("BuildingScriptableObjects/MushroomBox");
        List<Vector3Int> positions = new(){
                            new Vector3Int(4, 6, 0),
                            new Vector3Int(6, 6, 0),
                            new Vector3Int(8, 6, 0),
                            new Vector3Int(4, 8, 0),
                            new Vector3Int(6, 8, 0),
                            new Vector3Int(8, 8, 0),
                        };
        BuildingController.IsLoadingSave = true;
        UndoRedoController.ignoreAction = true;
        foreach (Vector3Int position in positions) {
            GameObject box = new("Mushroom Box");
            box.transform.parent = BuildingController.CurrentTilemapTransform;
            box.AddComponent<Building>().LoadFromScriptableObject(mushroomBox);
            box.GetComponent<Building>().PlaceBuilding(position);
            box.GetComponent<Tilemap>().color = new Color(1, 1, 1, 1);
        }
        BuildingController.IsLoadingSave = false;
        UndoRedoController.ignoreAction = false;
        Resources.UnloadAsset(mushroomBox);
        hasMushroomBoxes = true;
    }

    private void RemoveMushroomBoxes() {
        BuildingController.IsLoadingSave = true;
        UndoRedoController.ignoreAction = true;
        foreach (Transform building in BuildingController.CurrentTilemapTransform) {
            if (building.name == "Mushroom Box") building.GetComponent<Building>().DeleteBuilding(true);
        }
        BuildingController.IsLoadingSave = false;
        UndoRedoController.ignoreAction = false;
        hasMushroomBoxes = false;
    }

    public override void Load(ComponentData data) {
        if (data.GetComponentDataPropertyValue<string>("Has Mushroom Boxes") == "true") AddMushroomBoxes();
    }

    public override void Load(BuildingScriptableObject bso) {

    }

    public override ComponentData Save() {
        return new(typeof(CaveComponent),
        new() {
                new JProperty("Has Mushroom Boxes", hasMushroomBoxes.ToString()),
            });

    }
}
