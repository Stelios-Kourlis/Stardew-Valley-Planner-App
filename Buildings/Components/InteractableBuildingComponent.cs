using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static Utility.TilemapManager;
using static Utility.ClassManager;
using System;

[RequireComponent(typeof(Building))]
public class InteractableBuildingComponent : BuildingComponent {
    [field: SerializeField] public HashSet<ButtonTypes> BuildingInteractions { get; set; }
    public GameObject ButtonParentGameObject { get; private set; } = null;
    public Action ButtonsCreated;
    public static Action BuildingWasRightClicked { get; set; }
    bool wereButtonOpenOnBuildingPickup = false;

    public void Awake() {
        BuildingInteractions = new();
        Building.BuildingPlaced += _ => BuildingWasPlaced();//ignore position places
        Building.BuildingPickedUp += BuildingPickedUp;
    }

    public void OnDestroy() {
        Building.BuildingPlaced -= _ => BuildingWasPlaced();
        Destroy(gameObject.GetComponent<TieredBuildingComponent>());
        Destroy(gameObject.GetComponent<FishPondComponent>());
        Destroy(gameObject.GetComponent<AnimalHouseComponent>());
        Destroy(gameObject.GetComponent<MultipleTypeBuildingComponent>());
        Destroy(gameObject.GetComponent<EnterableBuildingComponent>());
    }

    public void BuildingPickedUp() {
        if (ButtonParentGameObject == null) return;
        wereButtonOpenOnBuildingPickup = ButtonParentGameObject.activeSelf;
        ButtonParentGameObject.SetActive(false);
    }

    public void AddInteractionToBuilding(ButtonTypes buttonType) {
        BuildingInteractions.Add(buttonType);
    }

    public void BuildingWasPlaced() {
        // Debug.Log(ButtonParentGameObject);
        if (ButtonParentGameObject == null) {
            ButtonParentGameObject = GetButtonController().CreateButtonsForBuilding(Building);
            ButtonsCreated?.Invoke();
        }
        else ButtonParentGameObject.SetActive(wereButtonOpenOnBuildingPickup);
    }

    public string GetBuildingSpriteName() {
        string name = "";
        // if (Building is IMultipleTypeBuilding multipleTypeBuildingTemp && Building is ITieredBuilding && multipleTypeBuildingTemp?.Type == null) Debug.LogWarning("If building is tiered and multiple type, add the type component before the tier component");
        if (Building.BuildingGameObject.TryGetComponent(out MultipleTypeBuildingComponent multipleTypeBuildingComponent)) name += $"{multipleTypeBuildingComponent.Type}";
        name += $"{Building.GetType()}";
        if (Building.BuildingGameObject.TryGetComponent(out TieredBuildingComponent tieredBuildingComponent)) name += $"{tieredBuildingComponent.Tier}";
        if (Building.BuildingGameObject.TryGetComponent(out ConnectingBuildingComponent connectingBuildingComponent)) name += $"{connectingBuildingComponent.GetConnectingFlags()}";
        return name;
    }

    public string GetBuildingInsideSpriteName() {
        string name = "";
        name += $"{Building.GetType()}";
        if (name.Contains("Cabin")) name = "House";
        if (Building.BuildingGameObject.TryGetComponent(out TieredBuildingComponent tieredBuildingComponent)) name += $"{tieredBuildingComponent.Tier}";
        return name;
    }

    public void Update() {
        if (ButtonParentGameObject != null) GetButtonController().UpdateButtonPositionsAndScaleForBuilding(Building);
    }

    public void OnMouseRightClick() {
        if (!BuildingController.isInsideBuilding.Key) ButtonParentGameObject.SetActive(!ButtonParentGameObject.activeSelf);
        BuildingWasRightClicked?.Invoke();
    }

    public override BuildingData.ComponentData Save() {
        return null;
    }

    public override void Load(BuildingData.ComponentData data) {
        return;
    }
}
