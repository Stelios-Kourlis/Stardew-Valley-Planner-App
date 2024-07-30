using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetBowl : Building, IExtraActionBuilding {
    public enum Types {
        Default,
        Stone,
        Hay
    }

    public MultipleTypeBuildingComponent MultipleTypeBuildingComponent => gameObject.GetComponent<MultipleTypeBuildingComponent>();

    public HashSet<ButtonTypes> BuildingInteractions => gameObject.GetComponent<InteractableBuildingComponent>().BuildingInteractions;

    public GameObject ButtonParentGameObject => gameObject.GetComponent<InteractableBuildingComponent>().ButtonParentGameObject;

    public override void OnAwake() {
        BaseHeight = 2;
        BuildingName = "Pet Bowl";
        base.OnAwake();
        gameObject.AddComponent<MultipleTypeBuildingComponent>().SetEnumType(typeof(Types));
    }

    public override List<MaterialCostEntry> GetMaterialsNeeded() {
        return new(){
            new(5_000, Materials.Coins),
            new(25, Materials.Hardwood)
        };
    }

    public string GetExtraData() {
        return $"|{MultipleTypeBuildingComponent.Type}";
    }

    public void LoadExtraBuildingData(string[] data) {
        MultipleTypeBuildingComponent.SetType((Types)Enum.Parse(typeof(Types), data[0]));
    }
}
