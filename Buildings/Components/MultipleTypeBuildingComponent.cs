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
    public BuildingVariant[] variants;
    public int CurrentVariantIndex { get; private set; }
    public string CurrentType => System.Text.RegularExpressions.Regex.Replace(variants[CurrentVariantIndex].variantName, "(?<!^)([A-Z])", " $1");
    public string CurrentTypeRaw => variants[CurrentVariantIndex].variantName;

    /// <summary>
    /// Sets the type of the building to the given enum type. If you want the building to start with a specific type, pass it as the second argument
    /// </summary>
    public MultipleTypeBuildingComponent SetEnumType(BuildingVariant[] types) {
        variants = types;
        Building.UpdateTexture(variants[CurrentVariantIndex].variantSprite);
        return this;
    }

    public void Awake() {
        if (!gameObject.GetComponent<InteractableBuildingComponent>()) gameObject.AddComponent<InteractableBuildingComponent>();

    }

    public void CycleType() {
        CurrentVariantIndex = (CurrentVariantIndex + 1) % variants.Length;
        SetType(CurrentVariantIndex);
    }

    public void SetType(int newTypeIndex) {
        CurrentVariantIndex = newTypeIndex;
        Sprite sprite = variants[CurrentVariantIndex].variantSprite;
        if (sprite != null) Building.UpdateTexture(sprite);
    }

    public List<MaterialCostEntry> GetMaterialsNeeded() {
        return variants[CurrentVariantIndex].variantCost;
    }

    public static GameObject[] CreateButtonsForAllTypes(BuildingScriptableObject bso) {
        List<GameObject> buttons = new();
        foreach (BuildingVariant variant in bso.variants) {
            GameObject button = new() {
                name = $"{variant.variantName}",
            };
            button.AddComponent<Image>().sprite = variant.variantSprite;
            button.GetComponent<Image>().preserveAspect = true;
            button.AddComponent<UIElement>();
            button.GetComponent<UIElement>().tooltipMessage = $"{System.Text.RegularExpressions.Regex.Replace(variant.variantName, "(?<!^)([A-Z])", " $1")} {(bso.typeName != BuildingType.Craftables ? bso.BuildingName : string.Empty)}";
            button.GetComponent<UIElement>().playSounds = false;
            button.GetComponent<UIElement>().ExpandOnHover = false;
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
                    new JProperty("Type Index", CurrentVariantIndex)
                });
    }

    public override void Load(BuildingData.ComponentData data) {
        SetType(data.GetComponentDataPropertyValue<int>("Type Index"));
    }

}