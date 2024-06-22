using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WoodTree : Building/*, IMultipleTypeBuilding, IExtraActionBuilding */{//the name wood tree is a stupid name but Unity already has a class named Tree, oh well


    // public MultipleTypeBuilding MultipleTypeBuildingComponent { get; private set; }

    // public enum Types {
    //     Oak,
    //     Maple,
    //     Pine,
    //     Mahogany,
    //     Cherry,
    //     Apricot,
    //     Orange,
    //     Peach,
    //     Pomegranate,
    //     Apple,
    //     Banana,
    //     Mango,
    //     Mushroom,
    //     Mystic,
    //     OakGreenRain,
    //     MapleGreenRain,
    //     PineGreenRain
    // }

    // public Enum Type => MultipleTypeBuildingComponent.Type;

    // public override void OnAwake() {
    //     BuildingName = "Tree";
    //     BaseHeight = 1;
    //     MultipleTypeBuildingComponent = gameObject.AddComponent<MultipleTypeBuilding>();
    //     base.OnAwake();
    // }

    // public override List<MaterialInfo> GetMaterialsNeeded() {
    //     throw new System.NotImplementedException();
    // }

    // public string AddToBuildingData() {
    //     return $"{Type}";
    // }

    // // public void LoadExtraBuildingData(string[] data) {
    // //     MultipleTypeBuildingComponent.SetType((Types)System.Enum.Parse(typeof(Types), data[0]));
    // // }

    // public GameObject[] CreateButtonsForAllTypes() => MultipleTypeBuildingComponent.CreateButtonsForAllTypes();

    // public void CycleType() => MultipleTypeBuildingComponent.CycleType();
    // public void SetType(Enum type) => MultipleTypeBuildingComponent.SetType(type);
    public override List<MaterialInfo> GetMaterialsNeeded() {
        throw new NotImplementedException();
    }
}
