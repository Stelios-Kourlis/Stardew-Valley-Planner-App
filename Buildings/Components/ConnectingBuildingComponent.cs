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
    public void Awake() {
        if (!gameObject.GetComponent<InteractableBuildingComponent>()) gameObject.AddComponent<InteractableBuildingComponent>();
        // if (Building is IConnectingBuilding connectingBuilding) connectingBuilding.UpdateSelf();
    }

    public int GetConnectingFlags() {
        if (Building.CurrentBuildingState != Building.BuildingState.PLACED) return 0;
        List<ConnectFlag> flags = new();
        Type buildingType = gameObject.GetComponent<Building>().GetType();
        Vector3Int position = gameObject.GetComponent<Building>().BaseCoordinates[0];
        List<Vector3Int> otherBuildings = BuildingController.buildings.Where(b => b.GetType() == buildingType).Select(b => b.BaseCoordinates[0]).ToList();
        Vector3Int[] neighbors = GetCrossAroundPosition(position).ToArray();
        if (otherBuildings.Contains(neighbors[0])) flags.Add(ConnectFlag.LEFT_ATTACHED);
        if (otherBuildings.Contains(neighbors[1])) flags.Add(ConnectFlag.RIGHT_ATTACHED);
        if (otherBuildings.Contains(neighbors[2])) flags.Add(ConnectFlag.BOTTOM_ATTACHED);
        if (connectsToTop) if (otherBuildings.Contains(neighbors[3])) flags.Add(ConnectFlag.TOP_ATTACHED);
        return flags.Cast<int>().Sum();
    }

    public void Load(BuildingScriptableObject bso) {
        connectsToTop = bso.connectsToTop;
    }

    public override void Load(BuildingData.ComponentData data) { //No saving/load function in this component
        return;
    }

    public override BuildingData.ComponentData Save() {
        return null;
    }

    public void UpdateSelf() {
        Building.UpdateTexture(Building.Atlas.GetSprite($"{gameObject.GetComponent<InteractableBuildingComponent>().GetBuildingSpriteName()}"));
    }

    public void UpdateAllOtherBuildingOfSameType() {
        Type buildingType = gameObject.GetComponent<Building>().GetType();
        List<Building> otherBuildingsOfSameType = BuildingController.buildings.Where(b => b.type == Building.type && b.transform != transform).ToList();//All buildings of same type except self
        foreach (Building building in otherBuildingsOfSameType) {
            building.GetComponent<ConnectingBuildingComponent>().UpdateSelf();
        }
    }
}
