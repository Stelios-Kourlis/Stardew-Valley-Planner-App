using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetBowl : Building, IMultipleTypeBuilding, IExtraActionBuilding {
    public enum Types {
        Default,
        Stone,
        Hay
    }

    public MultipleTypeBuildingComponent MultipleTypeBuildingComponent => gameObject.GetComponent<MultipleTypeBuildingComponent>();

    public Enum Type => MultipleTypeBuildingComponent.Type;

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
        return $"|{Type}";
    }

    public void LoadExtraBuildingData(string[] data) {
        SetType((Types)Enum.Parse(typeof(Types), data[0]));
    }

    public void CycleType() => MultipleTypeBuildingComponent.CycleType();

    public void SetType(Enum type) => MultipleTypeBuildingComponent.SetType(type);

    public GameObject[] CreateButtonsForAllTypes() => MultipleTypeBuildingComponent.CreateButtonsForAllTypes();
}
