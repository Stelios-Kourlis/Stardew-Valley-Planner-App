using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.U2D;
using UnityEngine.UI;
using static Utility.ClassManager;

public class Cabin : Building, ITieredBuilding, IMultipleTypeBuilding, IExtraActionBuilding {

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
    public int Tier => gameObject.GetComponent<TieredBuildingComponent>()?.Tier ?? 1;

    public Enum Type => gameObject.GetComponent<MultipleTypeBuildingComponent>()?.Type;

    // public static Types CurrentType { get; private set; }

    public List<ButtonTypes> BuildingInteractions => InteractableBuildingComponent.BuildingInteractions;

    public GameObject ButtonParentGameObject => InteractableBuildingComponent.ButtonParentGameObject;

    public int MaxTier => TieredBuildingComponent.MaxTier;

    public override void OnAwake() {
        BaseHeight = 3;
        BuildingName = "Cabin";
        base.OnAwake();
        for (int i = 0; i < cabinTypeIsPlaced.Length; i++) cabinTypeIsPlaced[i] = false;
        gameObject.AddComponent<MultipleTypeBuildingComponent>().SetEnumType(typeof(Types));
        gameObject.AddComponent<TieredBuildingComponent>().SetMaxTier(4);
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

    public override List<MaterialInfo> GetMaterialsNeeded() {
        List<MaterialInfo> level1 = new() { new(100, Materials.Coins) };
        return TieredBuildingComponent.Tier switch {
            2 => new List<MaterialInfo>{
                new(10_000, Materials.Coins),
                new(450, Materials.Wood),
            }.Union(level1).ToList(),
            3 => new List<MaterialInfo>{
                new(60_000, Materials.Coins),
                new(450, Materials.Wood),
                new(150, Materials.Hardwood),
            }.Union(level1).ToList(),
            4 => new List<MaterialInfo>{
                new(160_000, Materials.Coins),
                new(450, Materials.Wood),
                new(150, Materials.Hardwood),
            }.Union(level1).ToList(),
            _ => throw new System.Exception("Invalid Cabin Tier")
        };
    }

    public string AddToBuildingData() {
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

    // public GameObject[] CreateButtonsForAllTypes() {
    //     List<GameObject> buttons = new();
    //     foreach (Types type in Enum.GetValues(typeof(Types))) {
    //         GameObject button = Instantiate(Resources.Load<GameObject>("UI/BuildingButton"));
    //         button.name = $"{type}Button";
    //         button.GetComponent<Image>().sprite = MultipleTypeBuildingComponent.Atlas.GetSprite($"{type}1");
    //         Type buildingType = GetType();
    //         button.GetComponent<Button>().onClick.AddListener(() => {
    //             BuildingController.SetCurrentBuildingToMultipleTypeBuilding(buildingType, type);
    //             BuildingController.SetCurrentAction(Actions.PLACE);
    //         });
    //         buttons.Add(button);
    //     }
    //     return buttons.ToArray();
    // }

}
