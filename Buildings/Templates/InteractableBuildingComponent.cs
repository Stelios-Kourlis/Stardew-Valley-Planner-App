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
        UpdateBuildingButtons();
        // if (Building == null) {
        //     Component[] components = gameObject.GetComponents<Component>();
        //     foreach (Component component in components) {
        //         Debug.Log(component.GetType().ToString());
        //     }
        // }
        Building.BuildingPlaced += CreateBuildingButtons;
    }

    public void OnDestroy() {
        Building.BuildingPlaced -= CreateBuildingButtons;
        if (this is ITieredBuilding) Destroy(gameObject.GetComponent<TieredBuildingComponent>());
        if (this is IAnimalHouse) Destroy(gameObject.GetComponent<AnimalHouseComponent>());
        if (this is IMultipleTypeBuilding) Destroy(gameObject.GetComponent<MultipleTypeBuilding>());
        if (this is IEnterableBuilding) Destroy(gameObject.GetComponent<EnterableBuildingComponent>());
    }

    public void UpdateBuildingButtons() {
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
                buildingInteractions.Add((ButtonTypes)Enum.Parse(typeof(ButtonTypes), $"TIER_{tierStr}"));
            }
        }
        if (Building is IEnterableBuilding) buildingInteractions.Add(ButtonTypes.ENTER);
        if (Building is FishPond) {
            buildingInteractions.Add(ButtonTypes.PLACE_FISH);
            buildingInteractions.Add(ButtonTypes.CHANGE_FISH_POND_DECO);
        }
        BuildingInteractions = buildingInteractions.ToArray();
    }

    private void CreateBuildingButtons() {
        ButtonParentGameObject = GetButtonController().CreateButtonsForBuilding(Building);

    }

    public string GetBuildingSpriteName() {
        string name = "";
        if (Building is IMultipleTypeBuilding multipleTypeBuilding) name += $"{multipleTypeBuilding.Type}";
        name += $"{Building.GetType()}";
        if (Building is ITieredBuilding tieredBuilding) name += $"{tieredBuilding.Tier}";
        return name;
    }

    public void Update() {
        if (ButtonParentGameObject != null) GetButtonController().UpdateButtonPositionsAndScaleForBuilding(Building);
    }

    public void OnMouseRightClick() {
        if (Building.BaseCoordinates.Contains(GetMousePositionInTilemap())) ButtonParentGameObject.SetActive(!ButtonParentGameObject.activeInHierarchy);
    }
}
