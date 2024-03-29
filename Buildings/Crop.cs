using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using UnityEngine;
using UnityEngine.U2D;

public class Crop : Building, IMultipleTypeBuilding<Crop.Types>{

    public enum Types{
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
    
    public Types Type {get; private set;}
    private SpriteAtlas atlas;

    public override string TooltipMessage => Type.ToString();

    // Type IMultipleTypeBuilding<Type>.Type => throw new System.NotImplementedException();

    public override void OnAwake(){
        baseHeight = 1;
        base.OnAwake();
        atlas = Resources.Load<SpriteAtlas>("Buildings/CropsAtlas");
        SetType(Types.Hops);
    }

    public void SetType(Types type){
        Type = type;
        sprite = atlas.GetSprite(type.ToString());
    }

    public override List<MaterialInfo> GetMaterialsNeeded(){
        throw new System.NotImplementedException();//todo add materials
    }

    public override void RecreateBuildingForData(int x, int y, params string[] data){
        Place(new Vector3Int(x, y, 0));
        Type = (Types)System.Enum.Parse(typeof(Types), data[0]);
    }

    public void CycleType(){
        SetType((Types)(((int)Type + 1) % System.Enum.GetValues(typeof(Types)).Length));
    }
}
