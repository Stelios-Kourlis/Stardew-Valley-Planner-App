using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static Utility.TilemapManager;
using static Utility.ClassManager;
using System;

public class InteractableBuildingComponent : MonoBehaviour {
    public List<ButtonTypes> BuildingInteractions { get; set; }
    public GameObject ButtonParentGameObject { get; private set; } = null;
    public IInteractableBuilding Building => gameObject.GetComponent<IInteractableBuilding>();
    public Action ButtonsCreated;

    public void Awake() {
        // UpdateBuildingButtons();
        // if (Building == null) {
        //     Component[] components = gameObject.GetComponents<Component>();
        //     foreach (Component component in components) {
        //         Debug.Log(component.GetType().ToString());
        //     }
        // }
        BuildingInteractions = new();
        Building.BuildingPlaced += CreateBuildingButtons;
    }

    public void OnDestroy() {
        Building.BuildingPlaced -= CreateBuildingButtons;
        if (this is ITieredBuilding) Destroy(gameObject.GetComponent<TieredBuildingComponent>());
        if (this is IAnimalHouse) Destroy(gameObject.GetComponent<AnimalHouseComponent>());
        if (this is IMultipleTypeBuilding) Destroy(gameObject.GetComponent<MultipleTypeBuildingComponent>());
        if (this is IEnterableBuilding) Destroy(gameObject.GetComponent<EnterableBuildingComponent>());
    }

    public void AddInteractionToBuilding(ButtonTypes buttonType) {
        BuildingInteractions.Add(buttonType);
    }

    private void CreateBuildingButtons() {
        ButtonParentGameObject = GetButtonController().CreateButtonsForBuilding(Building);
        ButtonsCreated?.Invoke();
    }

    public string GetBuildingSpriteName() {
        // Debug.Log($"sprite name called for {Building.BuildingName}");
        string name = "";
        if (Building is IMultipleTypeBuilding multipleTypeBuildingTemp && Building is ITieredBuilding && multipleTypeBuildingTemp?.Type == null) Debug.LogWarning("If building is tiered and multiple type, add the type component before the tier component");
        if (Building is IMultipleTypeBuilding multipleTypeBuilding) name += $"{multipleTypeBuilding?.Type}";
        name += $"{Building.GetType()}";
        if (Building is ITieredBuilding tieredBuilding) name += $"{tieredBuilding?.Tier ?? 1}";
        if (Building is IConnectingBuilding connectingBuilding) name += $"{connectingBuilding.GetConnectingFlags()}";
        return name;
    }

    public void Update() {
        if (ButtonParentGameObject != null) GetButtonController().UpdateButtonPositionsAndScaleForBuilding(Building);
    }

    public void OnMouseRightClick() {
        if (Building.BaseCoordinates.Contains(GetMousePositionInTilemap())) ButtonParentGameObject.SetActive(!ButtonParentGameObject.activeInHierarchy);
    }
}
