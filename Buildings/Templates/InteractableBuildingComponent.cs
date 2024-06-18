using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static Utility.TilemapManager;
using static Utility.ClassManager;
using System;

public class InteractableBuildingComponent : MonoBehaviour {
    public ButtonTypes[] BuildingInteractions { get; set; }
    public GameObject ButtonParentGameObject { get; private set; } = null;
    public IInteractableBuilding Building => gameObject.GetComponent<IInteractableBuilding>();

    public void Awake() {
        Building.BuildingPlaced += CreateBuildingButtons;
        List<ButtonTypes> buildingInteractions = new();
        if (Building is ITieredBuilding tieredBuilding) {
            foreach (int tier in Enumerable.Range(1, tieredBuilding.MaxTier)) {
                string tierStr = tier switch {
                    1 => "ONE",
                    2 => "TWO",
                    3 => "THREE",
                    4 => "ONE", //todo placeholder for lack of icon for tier 4
                    _ => "INVALID"
                };
                buildingInteractions.Append((ButtonTypes)Enum.Parse(typeof(ButtonTypes), $"TIER_{tierStr}"));
            }
        }
        if (Building is IEnterableBuilding) buildingInteractions.Append(ButtonTypes.ENTER);
        if (Building is FishPond) {
            buildingInteractions.Append(ButtonTypes.PLACE_FISH);
            buildingInteractions.Append(ButtonTypes.CHANGE_FISH_POND_DECO);
        }
        BuildingInteractions = buildingInteractions.ToArray();
        if (BuildingInteractions.Length == 0) return;
        Debug.Log(BuildingInteractions.Length);
    }

    private void CreateBuildingButtons() {
        ButtonParentGameObject = GetButtonController().CreateButtonsForBuilding(Building);
    }

    public void Update() {
        if (Building.IsPlaced) GetButtonController().UpdateButtonPositionsAndScaleForBuilding(Building);
    }

    public void OnMouseRightClick() {
        if (Building.BaseCoordinates.Contains(GetMousePositionInTilemap())) ButtonParentGameObject.SetActive(!ButtonParentGameObject.activeInHierarchy);
    }
}
