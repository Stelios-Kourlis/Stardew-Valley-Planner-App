using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using UnityEngine;
using static Utility.BuildingManager;

public class Crop : MultipleTypeBuilding<Crop.Types>{

    public enum Types{
        NO_CROP,
        BlueJazz,
        Carrot,
        Cauliflower,
        CoffeeBean,
        Garlic,
        GreenBean,
        Kale,
        Parsnip,
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
    
    public override string TooltipMessage => Type.ToString();

    public override void OnAwake(){
        baseHeight = 1;
        if (CurrentType == Types.NO_CROP) CurrentType = Types.Parsnip;
        base.OnAwake();
        defaultSprite = atlas.GetSprite($"Parsnip");
    }

    public override List<MaterialInfo> GetMaterialsNeeded(){
        throw new System.NotImplementedException();//todo add materials
    }

    public override void RecreateBuildingForData(int x, int y, params string[] data){
        Place(new Vector3Int(x, y, 0));
        Type = (Types)System.Enum.Parse(typeof(Types), data[0]);
        UpdateTexture(atlas.GetSprite(Type.ToString()));
    }

    // public static void CreateButtonForCrops(){
    //     Transform contentGameObjectTransform = GameObject.Find("FloorSelectBar").transform.GetChild(0).GetChild(0);
    //     foreach (Types type in Enum.GetValues(typeof(Types))){
    //         if (type == Types.NO_CROP) continue; // skip the first element (None)
    //         Debug.Log($"Creating button for {type}, of enum {typeof(Types)}");
    //         CreateButton2<Crop.Types>(type.ToString(), $"Buildings/{type}", contentGameObjectTransform, null, type);
    //     }
    // }
}
