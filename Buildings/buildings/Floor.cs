using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Tilemaps;
using static Utility.ClassManager;
using static Utility.SpriteManager;
using static Utility.TilemapManager;

public class Floor : Building, IConnectingBuilding {

    public enum Types {
        WOOD_FLOOR,
        RUSTIC_PLANK_FLOOR,
        STRAW_FLOOR,
        WEATHERED_FLOOR,
        CRYSTAL_FLOOR,
        STONE_FLOOR,
        STONE_WALKWAY_FLOOR,
        BRICK_FLOOR,
        WOOD_PATH,
        GRAVEL_PATH,
        COBBLESTONE_PATH,
        STEPPING_STONE_PATH,
        CRYSTAL_PATH
    }

    public override string TooltipMessage => "";
    public MultipleTypeBuildingComponent MultipleTypeBuildingComponent { get; private set; }
    public ConnectingBuildingComponent ConnectingBuildingComponent { get; private set; }

    public HashSet<ButtonTypes> BuildingInteractions => gameObject.GetComponent<InteractableBuildingComponent>().BuildingInteractions;

    public GameObject ButtonParentGameObject => gameObject.GetComponent<InteractableBuildingComponent>().ButtonParentGameObject;


    public override void OnAwake() {
        BaseHeight = 1;
        base.OnAwake();
        BuildingName = "Floor";
        CanBeMassPlaced = true;
        MultipleTypeBuildingComponent = gameObject.AddComponent<MultipleTypeBuildingComponent>().SetEnumType(typeof(Types));
        ConnectingBuildingComponent = gameObject.AddComponent<ConnectingBuildingComponent>();
        BuildingPlaced += _ => gameObject.GetComponent<ConnectingBuildingComponent>().UpdateAllOtherBuildingOfSameType();
        BuildingRemoved += gameObject.GetComponent<ConnectingBuildingComponent>().UpdateAllOtherBuildingOfSameType;
    }

    protected void PerformExtraActionsOnPlacePreview(Vector3Int position) {
        UpdateTexture(MultipleTypeBuildingComponent.Atlas.GetSprite($"{gameObject.GetComponent<InteractableBuildingComponent>().GetBuildingSpriteName()}"));
    }

    public void PerformExtraActionsOnPlace(Vector3Int position) {
        UpdateTexture(MultipleTypeBuildingComponent.Atlas.GetSprite($"{gameObject.GetComponent<InteractableBuildingComponent>().GetBuildingSpriteName()}"));
        // FloorWasAltered?.Invoke(position);
    }

    public void PerformExtraActionsOnDelete() {
        BuildingPlaced -= _ => gameObject.GetComponent<ConnectingBuildingComponent>().UpdateAllOtherBuildingOfSameType();
        // FloorWasAltered?.Invoke(BaseCoordinates[0]);
    }

    public override List<MaterialCostEntry> GetMaterialsNeeded() {
        return MultipleTypeBuildingComponent.Type switch {
            Types.WOOD_FLOOR => new List<MaterialCostEntry>(){
                new(1, Materials.Wood)
            },
            Types.RUSTIC_PLANK_FLOOR => new List<MaterialCostEntry>(){
                new(1, Materials.Wood)
            },
            Types.STRAW_FLOOR => new List<MaterialCostEntry>(){
                new(1, Materials.Wood),
                new(1, Materials.Fiber)
            },
            Types.WEATHERED_FLOOR => new List<MaterialCostEntry>(){
                new(1, Materials.Wood)
            },
            Types.CRYSTAL_FLOOR => new List<MaterialCostEntry>(){
                new(1, Materials.RefinedQuartz)
            },
            Types.STONE_FLOOR => new List<MaterialCostEntry>(){
                new(1, Materials.Stone)
            },
            Types.STONE_WALKWAY_FLOOR => new List<MaterialCostEntry>(){
                new(1, Materials.Stone)
            },
            Types.BRICK_FLOOR => new List<MaterialCostEntry>(){
                new(2, Materials.Clay),
                new(5, Materials.Stone)
            },
            Types.WOOD_PATH => new List<MaterialCostEntry>(){
                new(1, Materials.Wood)
            },
            Types.GRAVEL_PATH => new List<MaterialCostEntry>(){
                new(1, Materials.Stone)
            },
            Types.COBBLESTONE_PATH => new List<MaterialCostEntry>(){
                new(1, Materials.Stone)
            },
            Types.STEPPING_STONE_PATH => new List<MaterialCostEntry>(){
                new(1, Materials.Stone)
            },
            Types.CRYSTAL_PATH => new List<MaterialCostEntry>(){
                new(1, Materials.RefinedQuartz)
            },
            _ => throw new Exception("Invalid Floor Type")
        };
    }

    public string GetExtraData() {
        return $"{MultipleTypeBuildingComponent.Type}";
    }

    public void LoadExtraBuildingData(string[] data) {
        MultipleTypeBuildingComponent.SetType((Types)Enum.Parse(typeof(Types), data[0]));
    }

    public void UpdateSelf() {
        UpdateTexture(MultipleTypeBuildingComponent.Atlas.GetSprite($"{gameObject.GetComponent<InteractableBuildingComponent>().GetBuildingSpriteName()}"));
    }

    public GameObject[] CreateButtonsForAllTypes() {
        List<GameObject> buttons = new();
        foreach (Types type in Enum.GetValues(typeof(Types))) {
            GameObject button = GameObject.Instantiate(Resources.Load<GameObject>("UI/BuildingButton"));
            button.name = $"{type}";
            button.GetComponent<Image>().sprite = MultipleTypeBuildingComponent.Atlas.GetSprite($"{type}0");

            Type buildingType = GetType();
            button.GetComponent<Button>().onClick.AddListener(() => {
                BuildingController.SetCurrentBuildingType(buildingType, type);
                BuildingController.SetCurrentAction(Actions.PLACE);
            });
            buttons.Add(button);
        }
        return buttons.ToArray();
    }

    public int GetConnectingFlags(bool includeTop = true) => ConnectingBuildingComponent?.GetConnectingFlags() ?? 0;
}
