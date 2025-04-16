using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using UnityEngine.U2D;
using static FlooringComponent;
using static WallsComponent;

[CreateAssetMenu(fileName = "building", menuName = "ScriptableObjects/Building", order = 1)]
public class BuildingScriptableObject : ScriptableObject {

    public string BuildingName {
        get {
            return Regex.Replace(typeName.ToString(), "(?<!^)([A-Z])", " $1");
        }
    }
    public BuildingType typeName;
    public int baseHeight, baseWidth;
    public bool canBeMassPlaced;
    public List<MaterialCostEntry> materialsNeeded;
    public Sprite defaultSprite;
    public SpriteAtlas atlas;
    public bool canBeDeleted;
    public bool canBePickedUp;

    public bool isMultipleType;
    public BuildingVariant[] variants;


    public bool isTiered;
    public BuildingTier[] tiers;

    public bool isAnimalHouse;
    public AnimalTiers[] animalsPerTier;

    public bool isConnecting;
    public bool connectsToTop;

    public bool isFishPond;

    public bool isCave;

    public bool isPaintable;
    public Sprite paintMask;

    public bool isEnterable;
    public ButtonTypes[] interiorInteractions;
    public WallsPerTier[] interiorWalls;
    public FlooringPerTier[] interiorFlooring;
    public bool hasInteriorExtensions;
    public BehaviourScript extraBehaviourType;

    public Categories category;
    public bool isExpanded;

}


[Serializable]
public class BehaviourScript {
    public string typeName;

    public Type Type {
        get { return string.IsNullOrEmpty(typeName) ? null : Type.GetType(typeName); }
        set { typeName = value.AssemblyQualifiedName; }
    }
}
