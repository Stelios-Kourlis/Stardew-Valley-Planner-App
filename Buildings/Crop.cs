using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using UnityEngine;
using UnityEngine.U2D;

public class Crop : Building{

    public enum Type{
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
    
    public Type type {get; private set;}
    private SpriteAtlas atlas;

    public override string TooltipMessage => type.ToString();
    public override void OnAwake(){
        baseHeight = 1;
        base.OnAwake();
        atlas = Resources.Load<SpriteAtlas>("Buildings/CropsAtlas");
        SetType(Type.Hops);
    }

    public void SetType(Type type){
        this.type = type;
        sprite = atlas.GetSprite(type.ToString());
    }

    public override List<MaterialInfo> GetMaterialsNeeded(){
        throw new System.NotImplementedException();
    }

    public override void RecreateBuildingForData(int x, int y, params string[] data){
        Place(new Vector3Int(x, y, 0));
        type = (Type)System.Enum.Parse(typeof(Type), data[0]);
    }
}
