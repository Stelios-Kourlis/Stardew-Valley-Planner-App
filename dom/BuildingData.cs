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

        /// <summary>
        /// The JSON property that contains the component data
        /// Name = The Component Type
        /// Value = The Component Data as a JObject
        /// </summary>
        public JProperty ComponentDataJProperty { get; private set; }

        public static List<Type> loadPriority = new(){
            typeof(ConnectingBuildingComponent),
            typeof(FishPondComponent),
            typeof(MultipleTypeBuildingComponent),
            typeof(TieredBuildingComponent),
            typeof(AnimalHouseComponent),
            typeof(EnterableBuildingComponent),
            typeof(HouseExtensionsComponent),
            typeof(FlooringComponent),
            typeof(WallsComponent),
        };

        public ComponentData(Type componentType, JObject componentData) {
            this.componentType = componentType;
            ComponentDataJProperty = new JProperty(componentType.ToString(), componentData);
            // this.loadPriority = loadPriority;
        }

        public ComponentData(Type componentType) {
            this.componentType = componentType;
            ComponentDataJProperty = new JProperty(componentType.ToString(), new JObject());
            // this.loadPriority = loadPriority;
        }

        public void AddProperty(JProperty property) {
            // Debug.Log($"Adding {property}");
            ComponentDataJProperty.Value.Value<JObject>().Add(property);
        }

        public JObject GetComponentDataObject() {
            return ComponentDataJProperty.Value.Value<JObject>();
        }

        public JProperty GetComponentDataProperty(string propertyName) {
            return GetComponentDataObject().Value<JProperty>(propertyName);
        }

        public bool TryGetComponentDataProperty(string propertyName, out JProperty property) {
            property = GetComponentDataObject().Value<JProperty>(propertyName);
            return property != null;
        }

        public IEnumerable<JProperty> GetAllComponentDataProperties() {
            return GetComponentDataObject().Properties();
        }

        public T GetComponentDataPropertyValue<T>(string propertyName) {
            return GetComponentDataObject().Value<T>(propertyName);
        }

        public override string ToString() {
            string data = $"{componentType}:\n";
            return data;
        }

        // public JObject GetComponentDataJObject() {
        //     BuildingJObject.Add(componentType.ToString(), componentData);
        // }
    }

    public readonly BuildingType buildingType;
    public readonly Vector3Int lowerLeftCorner;
    public readonly ComponentData[] componentData;
    public BuildingData(BuildingType type, Vector3Int lowerLeftCorner, ComponentData[] extraData = null) {
        this.buildingType = type;
        this.lowerLeftCorner = lowerLeftCorner;
        // this.loadPriority = loadPriority;
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
            ["Origin"] = new JArray { lowerLeftCorner.x, lowerLeftCorner.y },
            // ["Lower Left Corner X"] = lowerLeftCorner.x,
            // ["Lower Left Corner Y"] = lowerLeftCorner.y
        };
        foreach (ComponentData component in componentData) {
            buildingData.Add(component.ComponentDataJProperty);
            // Debug.Log($"Adding component {component.componentType} to JSON");
            // component.AddToJson(buildingData);
        }
        return buildingData;

    }
}
