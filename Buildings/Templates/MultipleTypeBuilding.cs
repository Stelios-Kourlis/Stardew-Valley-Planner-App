using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;
using static Utility.BuildingManager;
using static Utility.ClassManager;

public class MultipleTypeBuilding : MonoBehaviour {
    private Type enumType;
    public Enum Type { get; set; }
    public static Enum CurrentType { get; set; }
    public SpriteAtlas Atlas { get; private set; }
    public Sprite DefaultSprite { get; set; }
    public IMultipleTypeBuilding Building => gameObject.GetComponent<IMultipleTypeBuilding>();
    private string SpriteName => gameObject.GetComponent<InteractableBuildingComponent>().GetBuildingSpriteName();

    public MultipleTypeBuilding SetEnumType(Type type) {
        if (!type.IsEnum) throw new ArgumentException("Enum must be an enumerated type");
        enumType = type;
        SetType(CurrentType ?? (Enum)Enum.GetValues(enumType).GetValue(0));
        DefaultSprite = Atlas.GetSprite($"{Enum.GetValues(enumType).GetValue(0)}");
        return this;
    }

    public void Awake() {
        if (!gameObject.GetComponent<InteractableBuildingComponent>()) gameObject.AddComponent<InteractableBuildingComponent>();
        Atlas = Resources.Load<SpriteAtlas>($"Buildings/{Building.GetType()}Atlas");
        Debug.Assert(Atlas != null, $"Atlas \"{Building.GetType().Name}Atlas\" was not found");

    }

    public void CycleType() {
        int enumLength = Enum.GetValues(enumType).Length;
        int intValue = Convert.ToInt32(Type);
        intValue = (intValue + 1) % enumLength;
        SetType((Enum)Enum.GetValues(enumType).GetValue(intValue));
    }

    public void SetType(Enum type) {
        CurrentType = type;
        Type = type;
        Sprite sprite = Atlas.GetSprite($"{SpriteName}");
        Debug.Assert(sprite != null, $"Sprite {SpriteName} was not found in {Building.GetType()}Atlas");
        Building.UpdateTexture(sprite);
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
        Enum currentTypeBackup = CurrentType;
        foreach (Enum type in Enum.GetValues(enumType)) {
            GameObject button = Instantiate(Resources.Load<GameObject>("UI/BuildingButton"));
            button.name = $"{type}";
            SetType(type);
            button.GetComponent<Image>().sprite = Atlas.GetSprite($"{SpriteName}");
            Type buildingType = Building.GetType();
            button.GetComponent<Button>().onClick.AddListener(() => {
                BuildingController.SetCurrentBuildingType(buildingType, type);
                BuildingController.SetCurrentAction(Actions.PLACE);
            });
            buttons.Add(button);
        }
        SetType(currentTypeBackup);
        return buttons.ToArray();
    }
}