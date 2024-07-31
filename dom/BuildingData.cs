using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingData {

    public class ComponentData {
        public readonly Type componentType;
        public readonly List<KeyValuePair<string, string>> componentData;

        public ComponentData(Type componentType, List<KeyValuePair<string, string>> componentData) {
            this.componentType = componentType;
            this.componentData = componentData;
        }

        public void AddKeyValuePairToData(string key, string value) {
            componentData.Add(new KeyValuePair<string, string>(key, value));
        }

        public override string ToString() {
            string data = $"{componentType}:\n";
            foreach (KeyValuePair<string, string> kvp in componentData) data += $"- {kvp.Key}: {kvp.Value}\n";
            return data;
        }
    }

    public readonly Type type;
    public readonly Vector3Int lowerLeftCorner;
    public readonly ComponentData[] componentData;
    public BuildingData(Type type, Vector3Int lowerLeftCorner, ComponentData[] extraData = null) {
        this.type = type;
        this.lowerLeftCorner = lowerLeftCorner;
        this.componentData = extraData ?? (new ComponentData[0]);
    }

    public BuildingData(string[] stringData) {
        // string[] data = stringData.Split('|');
        // type = Type.GetType(data[0]);
        // lowerLeftCorner = new Vector3Int(int.Parse(data[1]), int.Parse(data[2]), 0);
        // extraData = data[3..];
    }

    public override string ToString() {
        string data = $"{type}\n{lowerLeftCorner.x}, {lowerLeftCorner.y}\n";
        foreach (ComponentData c in componentData) data += c.ToString() + "\n";
        return data;
    }
}
