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
        if (Building.BuildingGameObject.TryGetComponent(out MultipleTypeBuildingComponent multipleTypeBuildingComponent)) name += $"{multipleTypeBuildingComponent.CurrentTypeRaw}";
        name += $"{Building.type}";
        if (Building.BuildingGameObject.TryGetComponent(out TieredBuildingComponent tieredBuildingComponent)) name += $"{tieredBuildingComponent.Tier}";
        if (Building.BuildingGameObject.TryGetComponent(out ConnectingBuildingComponent connectingBuildingComponent)) name += $"{connectingBuildingComponent.GetConnectingFlags()}";
        // Debug.Log($"Generated name {name} for {Building.BuildingName}");
        return name;
    }

    public string GetBuildingInsideSpriteName() {
        string name = "";
        name += $"{Building.type}";
        if (name.Contains("Cabin")) name = "House";
        if (Building.BuildingGameObject.TryGetComponent(out TieredBuildingComponent tieredBuildingComponent)) name += $"{tieredBuildingComponent.Tier}";
        // Debug.Log($"Generated name {name} for {Building.BuildingName} Interior");
        return name;
    }

    private void UpdateButtonParentGameObject() {
        if (ButtonParentGameObject == null) return;
        if (!ButtonParentGameObject.activeInHierarchy) return;
        GetButtonController().UpdateButtonPositionsAndScaleForBuilding(Building);
    }

    /// <summary>
    /// Returns the transform of the button with the given type
    /// </summary>
    /// <returns>The transform of the button, might be null if the building doesn't have that interaction, or the buttons haven't been created yet</returns>
    public RectTransform GetInteractionButtonTransform(ButtonTypes buttonType) {
        if (ButtonParentGameObject == null) return null;
        return ButtonParentGameObject.transform.Find(buttonType.ToString()).GetComponent<RectTransform>();

    }

    public void OnMouseRightClick() {
        if (ButtonParentGameObject == null) return;
        if (!BuildingController.playerLocation.isInsideBuilding) ButtonParentGameObject.SetActive(!ButtonParentGameObject.activeSelf);
        UpdateButtonParentGameObject();
        BuildingWasRightClicked?.Invoke();
    }

    public void OnTierChange(int oldTier, int newTier) {
        TieredBuildingComponent tieredBuildingComponent = gameObject.GetComponent<TieredBuildingComponent>();
        IEnumerable<BuildingData> buildingsThatWillBeDeleted;
        IEnumerable<Animals> animalsThatWillBeRemoved;
        if (GetComponent<TieredBuildingComponent>().TryGetComponent(out AnimalHouseComponent animalHouseComponent)) {
            animalsThatWillBeRemoved = animalHouseComponent.GetAnimalsThatWillBeRemovedOnTierChange(newTier);
            animalHouseComponent.UpdateMaxAnimalCapacity(newTier);
        }
        else animalsThatWillBeRemoved = Enumerable.Empty<Animals>();

        if (GetComponent<TieredBuildingComponent>().TryGetComponent(out EnterableBuildingComponent enterableBuildingComponent)) {
            buildingsThatWillBeDeleted = enterableBuildingComponent.GetInteriorBuildings().Select(building => BuildingSaverLoader.Instance.SaveBuilding(building));
            enterableBuildingComponent.UpdateBuildingInterior();
        }
        else buildingsThatWillBeDeleted = Enumerable.Empty<BuildingData>();

        UndoRedoController.AddActionToLog(new BuildingTierChangeRecord(tieredBuildingComponent, (oldTier, newTier), buildingsThatWillBeDeleted, animalsThatWillBeRemoved));
    }


    public override BuildingData.ComponentData Save() {
        return null;
    }

    public override void Load(BuildingData.ComponentData data) {
        return;
    }

    public override void Load(BuildingScriptableObject bso) {
        BuildingInteractions = new();
        Building.BuildingPlaced += _ => BuildingWasPlaced();
        Building.BuildingPickedUp += BuildingPickedUp;
        CameraController.Instance.cameraMoved += UpdateButtonParentGameObject;
    }
}
