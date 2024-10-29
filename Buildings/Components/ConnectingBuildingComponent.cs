using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utility;
using static Utility.TilemapManager;

public enum ConnectFlag {
    TOP_ATTACHED = 1,
    BOTTOM_ATTACHED = 2,
    LEFT_ATTACHED = 4,
    RIGHT_ATTACHED = 8,
}

[RequireComponent(typeof(Building))]
public class ConnectingBuildingComponent : BuildingComponent {

    private bool connectsToTop;

    public int GetConnectingFlags() {
        if (Building.CurrentBuildingState != Building.BuildingState.PLACED) return 0;
        List<ConnectFlag> flags = new();
        BuildingType buildingType = gameObject.GetComponent<Building>().type;
        Vector3Int position = gameObject.GetComponent<Building>().Base;
        List<Vector3Int> otherBuildings = BuildingController.buildings.Where(b => b.type == buildingType && b.CurrentBuildingState == Building.BuildingState.PLACED).Select(b => b.Base).ToList();
        Vector3Int[] neighbors = GetCrossAroundPosition(position).ToArray();
        if (otherBuildings.Contains(neighbors[0])) flags.Add(ConnectFlag.LEFT_ATTACHED);
        if (otherBuildings.Contains(neighbors[1])) flags.Add(ConnectFlag.RIGHT_ATTACHED);
        if (connectsToTop) if (otherBuildings.Contains(neighbors[2])) flags.Add(ConnectFlag.BOTTOM_ATTACHED);
        if (connectsToTop) if (otherBuildings.Contains(neighbors[3])) flags.Add(ConnectFlag.TOP_ATTACHED);
        return flags.Cast<int>().Sum();
    }

    public override void Load(BuildingScriptableObject bso) {
        connectsToTop = bso.connectsToTop;

        Building.BuildingPlaced += UpdateAllOtherBuildingOfSameType;
        Building.BuildingPlaced += _ => UpdateSelf();
        Building.BuildingPickedUp += () => UpdateAllOtherBuildingOfSameType(Building.Base);
        Building.BuildingRemoved += () => UpdateAllOtherBuildingOfSameType(Building.Base);
    }

    public override void Load(BuildingData.ComponentData data) { //No saving/load function in this component
        return;
    }

    public override BuildingData.ComponentData Save() {
        return null;
    }

    public void UpdateSelf() {
        Building.UpdateTexture(Building.Atlas.GetSprite($"{GetComponent<InteractableBuildingComponent>().GetBuildingSpriteName()}"));
    }

    public void UpdateAllOtherBuildingOfSameType(Vector3Int position) {
        // UpdateSelf();
        List<Building> otherNeighbouringBuildingsOfSameType = BuildingController.buildings.Where(b => b.type == Building.type && b.transform != transform && AreNeighbors(b.Base, position)).ToList();//All buildings of same type except self
        foreach (Building building in otherNeighbouringBuildingsOfSameType) {
            building.GetComponent<ConnectingBuildingComponent>().UpdateSelf();
        }
    }

    private bool AreNeighbors(Vector3Int positionA, Vector3Int positionB) {
        return Mathf.Abs(positionA.x - positionB.x) + Mathf.Abs(positionA.y - positionB.y) == 1;
    }
}
