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

[RequireComponent(typeof(Building))]
public class MultipleTypeBuildingComponent : BuildingComponent {
    public Type EnumType { get; private set; }//this needs a rework
    public Enum Type { get; set; }
    public SpriteAtlas Atlas { get; private set; }
    public Sprite DefaultSprite { get; set; }
    private string SpriteName => gameObject.GetComponent<InteractableBuildingComponent>().GetBuildingSpriteName();

    /// <summary>
    /// Sets the type of the building to the given enum type. If you want the building to start with a specific type, pass it as the second argument
    /// </summary>
    public MultipleTypeBuildingComponent SetEnumType(Type type, Enum previousType = null) {
        if (!type.IsEnum) throw new ArgumentException("Enum must be an enumerated type");
        EnumType = type;
        SetType(previousType ?? (Enum)Enum.GetValues(EnumType).GetValue(0));
        DefaultSprite = Atlas.GetSprite($"{Enum.GetValues(EnumType).GetValue(0)}");
        return this;
    }

    public void Awake() {
        if (!gameObject.GetComponent<InteractableBuildingComponent>()) gameObject.AddComponent<InteractableBuildingComponent>();
        Atlas = Resources.Load<SpriteAtlas>($"Buildings/{Building.GetType()}Atlas");
        Debug.Assert(Atlas != null, $"Atlas \"{Building.GetType().Name}Atlas\" was not found");

    }

    public void CycleType() {
        int enumLength = Enum.GetValues(EnumType).Length;
        int intValue = Convert.ToInt32(Type);
        intValue = (intValue + 1) % enumLength;
        SetType((Enum)Enum.GetValues(EnumType).GetValue(intValue));
    }

    public void SetType(Enum type) {
        Type = type;
        Sprite sprite = Atlas.GetSprite($"{SpriteName}");
        // Debug.Assert(sprite != null, $"Sprite {SpriteName} was not found in {Building.GetType()}Atlas");
        if (sprite != null) Building.UpdateTexture(sprite);
    }

    public GameObject CreateButton() {
        GameObject button = Building.CreateBuildingButton();
        button.GetComponent<Image>().sprite = DefaultSprite;
        return button;
    }


    /// <summary>
    /// Creates a button for each type of the building
    /// </summary>
    /// <returns>An array with a button for each type, with no parent, caller should call transform.SetParent()</returns>
    public virtual GameObject[] CreateButtonsForAllTypes() {
        List<GameObject> buttons = new();
        Enum currentTypeBackup = Type;
        Type buildingType = Building.GetType();
        foreach (Enum type in Enum.GetValues(EnumType)) {
            GameObject button = new() {
                name = $"{type}",
            };
            SetType(type);
            button.AddComponent<Image>().sprite = Atlas.GetSprite($"{SpriteName}");
            if (Building.GetType() == typeof(Floor)) Debug.Log($"SpriteName: {SpriteName}");
            button.GetComponent<Image>().preserveAspect = true;
            button.AddComponent<UIElement>();
            button.GetComponent<UIElement>().playSounds = true;
            button.GetComponent<UIElement>().ExpandOnHover = true;
            button.AddComponent<Button>().onClick.AddListener(() => {
                BuildingController.SetCurrentBuildingType(buildingType, type);
                BuildingController.SetCurrentAction(Actions.PLACE);
            });
            buttons.Add(button);
        }
        SetType(currentTypeBackup);
        return buttons.ToArray();
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

    public override BuildingData.ComponentData Save() {
        return new(typeof(MultipleTypeBuildingComponent),
                new() {
                    new JProperty("Enum Type", EnumType.ToString()),
                    new JProperty("Type", Type.ToString())
                });
    }

    public override void Load(BuildingData.ComponentData data) {
        SetEnumType(System.Type.GetType(data.componentData.First(x => x.Name == "Enum Type").Value.ToString()));
        SetType((Enum)Enum.Parse(EnumType, data.componentData.First(x => x.Name == "Type").Value.ToString()));
    }
}