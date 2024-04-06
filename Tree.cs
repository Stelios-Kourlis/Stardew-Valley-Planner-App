using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree : MultipleTypeBuilding<Tree.Types>{
    public override string TooltipMessage => $"{Type} Tree";

    public enum Types{
        NO_TYPE,
        Oak,
        Maple,
        Pine,
        Mahogany,
        Cherry,
        Apricot,
        Orange,
        Peach,
        Pomegranate,
        Apple,
        Banana,
        Mango,
        Mushroom,
        Mystic,
        OakGreenRain,
        MapleGreenRain,
        PineGreenRain
    }

    public override void OnAwake(){
        baseHeight = 1;
        if (CurrentType == Types.NO_TYPE) CurrentType = Types.Oak;
        base.OnAwake();
        defaultSprite = atlas.GetSprite("Oak");
    }

    public override List<MaterialInfo> GetMaterialsNeeded(){
        throw new System.NotImplementedException();
    }

    public override void RecreateBuildingForData(int x, int y, params string[] data){
        Place(new Vector3Int(x, y, 0));
        Type = (Types)System.Enum.Parse(typeof(Types), data[0]);
        UpdateTexture(atlas.GetSprite(Type.ToString()));
    }
}
