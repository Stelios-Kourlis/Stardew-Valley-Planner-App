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

    [field: SerializeField]
    public int CurrentVariantIndex { get; private set; }
    public string CurrentType => System.Text.RegularExpressions.Regex.Replace(variants[CurrentVariantIndex].variantName, "(?<!^)([A-Z])", " $1");
    public string CurrentTypeRaw => variants[CurrentVariantIndex].variantName;

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

    public override void Load(BuildingScriptableObject bso) {
        variants = bso.variants;
        Building.UpdateTexture(variants[CurrentVariantIndex].variantSprite);
    }

    public override BuildingData.ComponentData Save() {
        return new(typeof(MultipleTypeBuildingComponent),
                new() {
                    new JProperty("Type Index", CurrentVariantIndex)
                });
    }

    public override void Load(BuildingData.ComponentData data) {
        Debug.Log($"index: {data.GetComponentDataPropertyValue<int>("Type Index")}");
        SetType(data.GetComponentDataPropertyValue<int>("Type Index"));
    }

}