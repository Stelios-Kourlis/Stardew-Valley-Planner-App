using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WoodTree : Building, IMultipleTypeBuilding, IExtraActionBuilding {//the name wood tree is a stupid name but Unity already has a class named Tree, oh well


    public MultipleTypeBuildingComponent MultipleTypeBuildingComponent { get; private set; }

    public enum Types {
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

    public Enum Type => gameObject.GetComponent<MultipleTypeBuildingComponent>().Type;

    public List<ButtonTypes> BuildingInteractions => gameObject.GetComponent<InteractableBuildingComponent>().BuildingInteractions;

    public GameObject ButtonParentGameObject => gameObject.GetComponent<InteractableBuildingComponent>().ButtonParentGameObject;

    public override void OnAwake() {
        BuildingName = "Tree";
        BaseHeight = 1;
        base.OnAwake();
        MultipleTypeBuildingComponent = gameObject.AddComponent<MultipleTypeBuildingComponent>().SetEnumType(typeof(Types));

    }

    public override List<MaterialInfo> GetMaterialsNeeded() {
        throw new System.NotImplementedException();
    }

    public string AddToBuildingData() {
        return $"{Type}";
    }

    public void LoadExtraBuildingData(string[] data) {
        MultipleTypeBuildingComponent.SetType((Types)System.Enum.Parse(typeof(Types), data[0]));
    }

    public GameObject[] CreateButtonsForAllTypes() => MultipleTypeBuildingComponent.CreateButtonsForAllTypes();

    public void CycleType() => MultipleTypeBuildingComponent.CycleType();
    public void SetType(Enum type) => MultipleTypeBuildingComponent.SetType(type);
}
