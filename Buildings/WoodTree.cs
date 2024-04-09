using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WoodTree : Building, IMultipleTypeBuilding<WoodTree.Types>{//the name wood tree is a stupid name but Unity already has a class named Tree, oh well
    

    public MultipleTypeBuilding<Types> MultipleTypeBuildingComponent {get; private set;}

    public enum Types{
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

    public override string TooltipMessage => $"{MultipleTypeBuildingComponent.Type} Tree";
    public override void OnAwake(){
        BaseHeight = 1;
        MultipleTypeBuildingComponent = new MultipleTypeBuilding<Types>(this);
        base.OnAwake();
    }

    public override List<MaterialInfo> GetMaterialsNeeded(){
        throw new System.NotImplementedException();
    }

    public override void RecreateBuildingForData(int x, int y, params string[] data){
        Place(new Vector3Int(x, y, 0));
        MultipleTypeBuildingComponent.SetType((Types)System.Enum.Parse(typeof(Types), data[0]));
    }

    public GameObject[] CreateButtonsForAllTypes(){
        return MultipleTypeBuildingComponent.CreateButtonsForAllTypes();
    }
}
