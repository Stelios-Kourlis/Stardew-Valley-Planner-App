using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class BuildingData {

    public class ComponentData {
        public readonly Type componentType;
        public readonly List<JProperty> componentData;
        // public readonly List<BuildingData> buildings;

        public ComponentData(Type componentType, List<JProperty> componentData) {
            this.componentType = componentType;
            this.componentData = componentData;
        }

        public void AddProperty(JProperty property) {
            Debug.Log($"Adding {property}");
            componentData.Add(property);
        }

        public override string ToString() {
            string data = $"{componentType}:\n";
            // foreach (KeyValuePair<string, string> kvp in componentData) data += $"- {kvp.Key}: {kvp.Value}\n";
            return data;
        }

        public void AddToJson(JObject BuildingJObject) {
            JObject componentJObject = new();
            foreach (var property in componentData) {
                componentJObject[property.Name] = property.Value;
            }
            BuildingJObject[componentType.ToString()] = componentJObject;
        }
    }

    public readonly Type buildingType;
    public readonly Vector3Int lowerLeftCorner;
    public readonly ComponentData[] componentData;
    // private readonly JsonSerializerSettings settings = new();
    public BuildingData(Type type, Vector3Int lowerLeftCorner, ComponentData[] extraData = null) {
        this.buildingType = type;
        this.lowerLeftCorner = lowerLeftCorner;
        this.componentData = extraData ?? (new ComponentData[0]);
    }

    public override string ToString() {
        string data = $"Building Type: {buildingType}\nLower Left Corner: {lowerLeftCorner}\n";
        foreach (ComponentData compData in componentData) data += compData.ToString();
        return data;
    }

    public JObject ToJson() {

        JObject buildingData = new() {
            ["Building Type"] = buildingType.ToString(),
            ["Lower Left Corner X"] = lowerLeftCorner.x,
            ["Lower Left Corner Y"] = lowerLeftCorner.y
        };
        foreach (var component in componentData) {
            Debug.Log($"Adding component {component.componentType} to JSON");
            component.AddToJson(buildingData);
        }
        return buildingData;

    }
}
