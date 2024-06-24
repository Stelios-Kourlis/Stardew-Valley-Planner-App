using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using UnityEngine;
using static Utility.BuildingManager;

public class Crop : Building, IMultipleTypeBuilding, IExtraActionBuilding {
    public MultipleTypeBuildingComponent MultipleTypeBuildingComponent => gameObject.GetComponent<MultipleTypeBuildingComponent>();

    public enum Types {
        Parsnip,
        BlueJazz,
        Carrot,
        Cauliflower,
        CoffeeBean,
        Garlic,
        GreenBean,
        Kale,
        Potato,
        Rhubarb,
        Strawberry,
        Tulip,
        UnmilledRice,
        Blueberry,
        Corn,
        Hops,
        HotPepper,
        Melon,
        Poppy,
        Radish,
        RedCabbage,
        Starfruit,
        SummerSpangle,
        SummerSquash,
        Sunflower,
        Tomato,
        Wheat,
        Amaranth,
        Artichoke,
        Beet,
        BokChoy,
        Broccoli,
        Cranberries,
        Eggplant,
        FairyRose,
        Grape,
        Pumpkin,
        Yam,
        Powdermelon,
        AncientFruit,
        CactusFruit,
        Pineapple,
        TaroRoot,
        SweetGemBerry,
        TeaLeaves
    }


    public Enum Type => MultipleTypeBuildingComponent.Type;

    public List<ButtonTypes> BuildingInteractions => gameObject.GetComponent<InteractableBuildingComponent>().BuildingInteractions;

    public GameObject ButtonParentGameObject => gameObject.GetComponent<InteractableBuildingComponent>().ButtonParentGameObject;

    public override void OnAwake() {
        BaseHeight = 1;
        gameObject.AddComponent<MultipleTypeBuildingComponent>();
        base.OnAwake();
    }

    public override List<MaterialInfo> GetMaterialsNeeded() {
        return new List<MaterialInfo>{
            new($"{Type} Seeds"),
        };
    }

    public void LoadExtraBuildingData(string[] data) {
        MultipleTypeBuildingComponent.SetType((Types)Enum.Parse(typeof(Types), data[0]));
    }

    public GameObject[] CreateButtonsForAllTypes() {
        return MultipleTypeBuildingComponent.CreateButtonsForAllTypes();
    }

    public void CycleType() => MultipleTypeBuildingComponent.CycleType();

    public void SetType(Enum type) => MultipleTypeBuildingComponent.SetType(type);
}
