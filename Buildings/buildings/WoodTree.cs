using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WoodTree : Building, IExtraActionBuilding {//the name wood tree is a stupid name but Unity already has a class named Tree, oh well


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

    public HashSet<ButtonTypes> BuildingInteractions => gameObject.GetComponent<InteractableBuildingComponent>().BuildingInteractions;

    public GameObject ButtonParentGameObject => gameObject.GetComponent<InteractableBuildingComponent>().ButtonParentGameObject;


    public void PerformExtraActionsOnPlace(Vector3Int position) {
        BaseCoordinates = new Vector3Int[] { position + new Vector3Int(1, 0, 0) };
    }

    public override void OnAwake() {
        BuildingName = "Tree";
        BaseHeight = 1;
        base.OnAwake();
        MultipleTypeBuildingComponent = gameObject.AddComponent<MultipleTypeBuildingComponent>().SetEnumType(typeof(Types));

    }

    public override List<MaterialCostEntry> GetMaterialsNeeded() {
        throw new System.NotImplementedException();
    }

    public string GetExtraData() {
        return $"{MultipleTypeBuildingComponent.Type}";
    }

    public void LoadExtraBuildingData(string[] data) {
        MultipleTypeBuildingComponent.SetType((Types)System.Enum.Parse(typeof(Types), data[0]));
    }
}
