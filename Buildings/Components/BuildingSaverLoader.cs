using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static BuildingData;

public class BuildingSaverLoader : MonoBehaviour {

    BuildingData buildingData;

    [ContextMenu("Save Building")]
    public BuildingData SaveBuilding() {
        if (!gameObject.TryGetComponent(out Building Building)) throw new System.Exception("Building component not found");
        if (Building.CurrentBuildingState != Building.BuildingState.PLACED) throw new System.Exception("Building is not placed, cannot save");

        List<ComponentData> extraData = new();
        if (TryGetComponent(out TieredBuildingComponent tieredBuildingComponent)) {
            ComponentData tierData = new(typeof(TieredBuildingComponent),
                new List<KeyValuePair<string, string>> {
                    new("Tier", tieredBuildingComponent.Tier.ToString())
                });
            extraData.Add(tierData);
        }
        if (TryGetComponent(out MultipleTypeBuildingComponent multipleTypeBuildingComponent)) {
            ComponentData multipleTypeData = new(typeof(MultipleTypeBuildingComponent),
                new List<KeyValuePair<string, string>> {
                    new("Enum Type", multipleTypeBuildingComponent.EnumType.ToString()),
                    new("Type", multipleTypeBuildingComponent.Type.ToString())
                });
            extraData.Add(multipleTypeData);
        }
        if (TryGetComponent(out AnimalHouseComponent animalHouseComponent)) {
            ComponentData animalHouseData = new(typeof(AnimalHouseComponent),
                new List<KeyValuePair<string, string>> {
                    new("Animals In Building", animalHouseComponent.AnimalsInBuilding.Count.ToString())
                });
            foreach (var kvp in animalHouseComponent.AnimalsInBuilding) {
                animalHouseData.AddKeyValuePairToData(kvp.Key.ToString(), kvp.Value.ToString());
            }
            extraData.Add(animalHouseData);
        }
        // Debug.Log(Building.GetType());
        // Debug.Log(Building.BaseCoordinates[0]);
        // Debug.Log(new BuildingData(Building.GetType(), Building.BaseCoordinates[0], extraData.ToArray()));
        buildingData = new(Building.GetType(), Building.BaseCoordinates[0], extraData.ToArray());
        return buildingData;
    }

    public void LoadBuilding(BuildingData data) {
        if (!data.type.IsAssignableFrom(typeof(Building))) throw new System.Exception($"Invalid building type {data.type}");
        gameObject.AddComponent(data.type);
        Building Building = gameObject.GetComponent<Building>();
        Building.PlaceBuilding(data.lowerLeftCorner);
        this.buildingData = data;
        foreach (ComponentData compData in data.componentData) {
            switch (compData.componentType) {
                case Type t when t == typeof(TieredBuildingComponent):
                    TieredBuildingComponent tieredBuildingComponent = gameObject.AddComponent<TieredBuildingComponent>();
                    tieredBuildingComponent.SetTier(int.Parse(compData.componentData[0].Value));
                    break;
                case Type t when t == typeof(MultipleTypeBuildingComponent):
                    MultipleTypeBuildingComponent multipleTypeBuildingComponent = gameObject.AddComponent<MultipleTypeBuildingComponent>();
                    multipleTypeBuildingComponent.SetEnumType(Type.GetType(compData.componentData[0].Value));
                    multipleTypeBuildingComponent.SetType((Enum)Enum.Parse(multipleTypeBuildingComponent.EnumType, compData.componentData[1].Value));
                    break;
                case Type t when t == typeof(AnimalHouseComponent):
                    AnimalHouseComponent animalHouseComponent = gameObject.AddComponent<AnimalHouseComponent>();
                    for (int i = 1; i < int.Parse(compData.componentData[0].Value); i++) {
                        animalHouseComponent.AddAnimal(Enum.Parse<Animals>(compData.componentData[i].Key));
                    }
                    break;
            }
        }
    }
}
