using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;
using static Utility.BuildingManager;
using static Utility.ClassManager;

[Serializable]
public class BuildingVariant {
    public string variantName;
    public Sprite variantSprite;
    public List<MaterialCostEntry> variantCost;
}

[RequireComponent(typeof(Building))]
public class MultipleTypeBuildingComponent : BuildingComponent {
    // public Type EnumType { get; private set; }//this needs a rework
    // public Enum Type { get; set; }
    public BuildingVariant[] variants;
    public int CurrentVariantIndex { get; private set; }
    public string CurrentType => variants[CurrentVariantIndex].variantName;
    public SpriteAtlas Atlas { get; private set; }
    // public Sprite DefaultSprite { get; set; }
    private string SpriteName => gameObject.GetComponent<InteractableBuildingComponent>().GetBuildingSpriteName();

    /// <summary>
    /// Sets the type of the building to the given enum type. If you want the building to start with a specific type, pass it as the second argument
    /// </summary>
    public MultipleTypeBuildingComponent SetEnumType(BuildingVariant[] types) {
        this.variants = types;
        // SetType(previousType ?? (Enum)Enum.GetValues(EnumType).GetValue(0));
        // DefaultSprite = Atlas.GetSprite($"{Enum.GetValues(EnumType).GetValue(0)}");
        return this;
    }

    public void Awake() {
        if (!gameObject.GetComponent<InteractableBuildingComponent>()) gameObject.AddComponent<InteractableBuildingComponent>();
        Atlas = Resources.Load<SpriteAtlas>($"Buildings/{Building.type}Atlas");
        Debug.Assert(Atlas != null, $"Atlas \"{Building.type}Atlas\" was not found");

    }

    public void CycleType() {
        CurrentVariantIndex = (CurrentVariantIndex + 1) % variants.Length;
        SetType(CurrentVariantIndex);
    }

    public void SetType(int newTypeIndex) {
        CurrentVariantIndex = newTypeIndex;
        Sprite sprite = Atlas.GetSprite($"{SpriteName}");
        if (sprite != null) Building.UpdateTexture(sprite);
    }

    public List<MaterialCostEntry> GetMaterialsNeeded() {
        return variants[CurrentVariantIndex].variantCost;
    }

    public static GameObject[] CreateButtonsForAllTypes(BuildingScriptableObject bso) {
        List<GameObject> buttons = new();
        foreach (var variant in bso.variants) {
            GameObject button = new() {
                name = $"{variant.variantName}",
            };
            button.AddComponent<Image>().sprite = variant.variantSprite;
            button.GetComponent<Image>().preserveAspect = true;
            button.AddComponent<UIElement>();
            button.GetComponent<UIElement>().playSounds = true;
            button.GetComponent<UIElement>().ExpandOnHover = true;
            button.AddComponent<Button>().onClick.AddListener(() => {
                BuildingController.SetCurrentBuildingType(bso.typeName, bso.variants.ToList().IndexOf(variant));
                BuildingController.SetCurrentAction(Actions.PLACE);
            });
            buttons.Add(button);
        }
        return buttons.ToArray();
    }

    public static GameObject[] CreateButtonsForAllTypes(BuildingType type) {
        BuildingScriptableObject bso = Resources.Load<BuildingScriptableObject>($"BuildingScriptableObjects/{type}");
        GameObject[] buttons = CreateButtonsForAllTypes(bso);
        Resources.UnloadAsset(bso);
        return buttons;
    }

    // public static GameObject[] CreateButtonsForAllTypes(Type enumType, Type buildingType) { //can this even be made static?
    //     List<GameObject> buttons = new();
    //     // Enum currentTypeBackup = Type;
    //     SpriteAtlas atlas = Resources.Load<SpriteAtlas>($"Buildings/{buildingType}Atlas");
    //     foreach (Enum type in Enum.GetValues(enumType)) {
    //         GameObject button = new() {
    //             name = $"{type}",
    //         };
    //         button.AddComponent<Image>().sprite = atlas.GetSprite($"{type}"); ;
    //         // SetType(type);
    //         // button.GetComponent<Image>().sprite = Atlas.GetSprite($"{SpriteName}");
    //         button.AddComponent<UIElement>();
    //         // Type buildingType = Building.GetType();
    //         button.AddComponent<Button>().onClick.AddListener(() => {
    //             // BuildingController.SetCurrentBuildingType(buildingType, type);
    //             BuildingController.SetCurrentAction(Actions.PLACE);
    //         });
    //         buttons.Add(button);
    //     }
    //     // SetType(currentTypeBackup);
    //     Resources.UnloadAsset(atlas);
    //     return buttons.ToArray();
    // }

    public void Load(BuildingScriptableObject bso) {
        SetEnumType(bso.variants);
    }

    public override BuildingData.ComponentData Save() {
        return new(typeof(MultipleTypeBuildingComponent),
                new() {
                    new JProperty("Variants", new JArray(variants.Select(variant => variant.variantName))),
                    new JProperty("Type", CurrentVariantIndex)
                });
    }

    public override void Load(BuildingData.ComponentData data) {
        BuildingVariant[] enumTypes = ((JArray)data.componentData.First(x => x.Name == "Variants").Value).ToObject<BuildingVariant[]>();
        SetEnumType(enumTypes);
        SetType((int)data.componentData.First(x => x.Name == "Type").Value);
    }

}