using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using UnityEngine;
using static Utility.BuildingManager;

public class Crop: Building, IMultipleTypeBuilding<Crop.Types>{
    public MultipleTypeBuilding<Types> MultipleTypeBuildingComponent {get; private set;}

    public enum Types{
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
    
    public override string TooltipMessage => MultipleTypeBuildingComponent.Type.ToString();

    public Types Type => MultipleTypeBuildingComponent.Type;

    public override void OnAwake(){
        BaseHeight = 1;
        MultipleTypeBuildingComponent = new MultipleTypeBuilding<Types>(this);
        base.OnAwake();
    }

    public override List<MaterialInfo> GetMaterialsNeeded(){
        throw new System.NotImplementedException();//todo add materials
    }

    public override void RecreateBuildingForData(int x, int y, params string[] data){
        Place(new Vector3Int(x, y, 0));
        MultipleTypeBuildingComponent.SetType((Types)Enum.Parse(typeof(Types), data[0]));
    }

    public GameObject[] CreateButtonsForAllTypes(){
        return MultipleTypeBuildingComponent.CreateButtonsForAllTypes();
    }

    public void CycleType() => MultipleTypeBuildingComponent.CycleType();

    public void SetType(Types type) => MultipleTypeBuildingComponent.SetType(type);
}
