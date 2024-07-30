using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.U2D;
using UnityEngine.UI;
using static Utility.ClassManager;

public class Cabin : Building, IExtraActionBuilding {

    public enum Types {
        Wood,
        Plank,
        Stone,
        Beach,
        Neighbor,
        Rustic,
        Trailer
    }
    private static readonly bool[] cabinTypeIsPlaced = new bool[Enum.GetValues(typeof(Types)).Length];
    private MultipleTypeBuildingComponent MultipleTypeBuildingComponent => gameObject.GetComponent<MultipleTypeBuildingComponent>();
    private TieredBuildingComponent TieredBuildingComponent => gameObject.GetComponent<TieredBuildingComponent>();
    private InteractableBuildingComponent InteractableBuildingComponent => gameObject.GetComponent<InteractableBuildingComponent>();
    private EnterableBuildingComponent EnterableBuildingComponent => gameObject.GetComponent<EnterableBuildingComponent>();

    public Enum Type => gameObject.GetComponent<MultipleTypeBuildingComponent>()?.Type;

    // public static Types CurrentType { get; private set; }

    public HashSet<ButtonTypes> BuildingInteractions => InteractableBuildingComponent.BuildingInteractions;

    public GameObject ButtonParentGameObject => InteractableBuildingComponent.ButtonParentGameObject;

    public HashSet<Vector3Int> InteriorUnavailableCoordinates => EnterableBuildingComponent.InteriorUnavailableCoordinates;

    public HashSet<Vector3Int> InteriorPlantableCoordinates => EnterableBuildingComponent.InteriorPlantableCoordinates;

    public override void OnAwake() {
        BaseHeight = 3;
        BuildingName = "Cabin";
        base.OnAwake();
        for (int i = 0; i < cabinTypeIsPlaced.Length; i++) cabinTypeIsPlaced[i] = false;
        gameObject.AddComponent<MultipleTypeBuildingComponent>().SetEnumType(typeof(Types));
        gameObject.AddComponent<TieredBuildingComponent>().SetMaxTier(4);
        gameObject.AddComponent<EnterableBuildingComponent>();
        // multipleTypeBuildingComponent.DefaultSprite = multipleTypeBuildingComponent.Atlas.GetSprite($"{Types.Wood}1");

        // SetType(CurrentType);
        // SetTier(1);
    }

    public void PerformExtraActionsOnPlace(Vector3Int position) {
        if (cabinTypeIsPlaced[Convert.ToInt32(Type)]) {//this check is not enforced
            GetNotificationManager().SendNotification("You can only have one of each type of cabin", NotificationManager.Icons.ErrorIcon);
            return;
        }
        cabinTypeIsPlaced[Convert.ToInt32(Type)] = true;
    }

    public void PerformExtraActionsOnDelete() {
        cabinTypeIsPlaced[Convert.ToInt32(Type)] = false;
    }

    public override List<MaterialCostEntry> GetMaterialsNeeded() {
        List<MaterialCostEntry> level1 = new() { new(100, Materials.Coins) };
        return TieredBuildingComponent.Tier switch {
            2 => new List<MaterialCostEntry>{
                new(10_000, Materials.Coins),
                new(450, Materials.Wood),
            }.Union(level1).ToList(),
            3 => new List<MaterialCostEntry>{
                new(60_000, Materials.Coins),
                new(450, Materials.Wood),
                new(150, Materials.Hardwood),
            }.Union(level1).ToList(),
            4 => new List<MaterialCostEntry>{
                new(160_000, Materials.Coins),
                new(450, Materials.Wood),
                new(150, Materials.Hardwood),
            }.Union(level1).ToList(),
            _ => throw new System.Exception("Invalid Cabin Tier")
        };
    }

    public string GetExtraData() {
        return $"{TieredBuildingComponent.Tier}|{MultipleTypeBuildingComponent.Type}";
    }

    public void LoadExtraBuildingData(string[] data) {
        TieredBuildingComponent.SetTier(int.Parse(data[0]));
        MultipleTypeBuildingComponent.SetType((Types)Enum.Parse(typeof(Types), data[1]));
    }

    // public void SetTier(int tier) {
    //     tieredBuildingComponent.SetTier(tier);
    //     Sprite sprite;
    //     Types type = multipleTypeBuildingComponent?.Type ?? Types.Wood;
    //     // sprite = Atlas.GetSprite($"{type}{tier}");
    //     // UpdateTexture(sprite);
    // }

    public void SetTier(int tier) => TieredBuildingComponent.SetTier(tier);

    public void CycleType() => MultipleTypeBuildingComponent.CycleType();

    public void SetType(Enum type) => MultipleTypeBuildingComponent.SetType(type);
    public GameObject[] CreateButtonsForAllTypes() => MultipleTypeBuildingComponent.CreateButtonsForAllTypes();

    public void ToggleEditBuildingInterior() => EnterableBuildingComponent.ToggleEditBuildingInterior();

    public void EditBuildingInterior() => EnterableBuildingComponent.EditBuildingInterior();

    public void ExitBuildingInteriorEditing() => EnterableBuildingComponent.ExitBuildingInteriorEditing();

    public void OnMouseRightClick() {
        if (!BuildingController.isInsideBuilding.Key) ButtonParentGameObject.SetActive(!ButtonParentGameObject.activeSelf);
    }
}
