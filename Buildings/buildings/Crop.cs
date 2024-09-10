using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using UnityEngine;
using static Utility.BuildingManager;

public class Crop : Building, IExtraActionBuilding {
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

    public override void OnAwake() {
        BaseHeight = 1;
        BuildingName = "Crop";
        base.OnAwake();
        CanBeMassPlaced = true;
        gameObject.AddComponent<MultipleTypeBuildingComponent>().SetEnumType(typeof(Types));

    }

    public override List<MaterialCostEntry> GetMaterialsNeeded() {
        return new List<MaterialCostEntry>{
            new($"{MultipleTypeBuildingComponent.Type} Seeds"),
        };
    }
}
