using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static Utility.TilemapManager;
using static Utility.ClassManager;
using System;

public class InteractableBuildingComponent {
    public ButtonTypes[] BuildingInteractions { get; }
    public GameObject ButtonParentGameObject { get; private set; }
    public IInteractableBuilding Building { get; }

    public InteractableBuildingComponent(IInteractableBuilding building) {
        Building = building;
        List<ButtonTypes> buildingInteractions = new();
        if (building is ITieredBuilding tieredBuilding) {
            foreach (int tier in Enumerable.Range(1, tieredBuilding.MaxTier)) {
                string tierStr = tier switch {
                    1 => "ONE",
                    2 => "TWO",
                    3 => "THREE",
                    4 => "FOUR",
                    _ => "INVALID"
                };
                buildingInteractions.Append((ButtonTypes)Enum.Parse(typeof(ButtonTypes), $"TIER_{tierStr}"));
            }
        }
        if (building is IEnterableBuilding) buildingInteractions.Append(ButtonTypes.ENTER);
        if (building is FishPond) {
            buildingInteractions.Append(ButtonTypes.PLACE_FISH);
            buildingInteractions.Append(ButtonTypes.CHANGE_FISH_POND_DECO);
        }
        BuildingInteractions = buildingInteractions.ToArray();
        if (BuildingInteractions.Length == 0) return;
        Debug.Log(BuildingInteractions.Length);
        // Debug.Log(building.BuildingInteractions != null);
        // GetButtonController().CreateButtonsForBuilding(building);
    }

    private void CreateButtons() {
        ButtonParentGameObject = GetButtonController().CreateButtonsForBuilding(Building);
    }
    public void OnMouseRightClick() {
        if (Building.BaseCoordinates.Contains(GetMousePositionInTilemap())) ButtonParentGameObject.SetActive(!ButtonParentGameObject.activeInHierarchy);
    }
}
