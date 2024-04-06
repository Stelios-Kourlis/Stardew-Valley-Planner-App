using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;
using static Utility.BuildingManager;
using static Utility.ClassManager;

/// <summary>
/// Buildings that have multiple types should extend this
/// For Building with multiple tiers, use ITieredBuilding
/// T should be an enum with the types of the building, first element should be a dummy element, second should be the type that will be on the button
/// </summary>

public abstract class MultipleTypeBuilding<T> : Building where T : struct{
    public T Type {get; protected set;}
    public static T CurrentType {get; protected set;}
    protected SpriteAtlas atlas;
    protected Sprite defaultSprite;

    public override void OnAwake(){
        if (!typeof(T).IsEnum) throw new ArgumentException("T must be an enumerated type");
        base.OnAwake();
        atlas = Resources.Load<SpriteAtlas>($"Buildings/{GetType().Name}sAtlas");
        Debug.Assert(atlas != null, $"Atlas \"{GetType().Name}sAtlas\" was not found");
        SetType(CurrentType);
    }

    public virtual void CycleType(){
        int enumLength = Enum.GetValues(typeof(T)).Length;
        int intValue = Convert.ToInt32(Type);
        intValue = (intValue + 1) % enumLength;
        SetType((T)Enum.ToObject(typeof(T), intValue));
    }

    public virtual void SetType(T type){
        CurrentType = type;
        Type = type;
        UpdateTexture(atlas.GetSprite($"{type}"));
    }

    public override GameObject CreateButton(){
        GameObject button = base.CreateButton();
        button.GetComponent<Image>().sprite = defaultSprite;
        return button;
    }

    /// <summary>
    /// Creates a button for each type of the building
    /// </summary>
    /// <returns>An array with a button for each type, with no parent, caller should call transform.SetParent()</returns>
    public virtual GameObject[] CreateButtonsForAllTypes(){
        List<GameObject> buttons = new List<GameObject>();
        foreach (T type in Enum.GetValues(typeof(T))){
            if ((int)(object)type == 0) continue; // skip the first element (None)
            GameObject button = Instantiate(Resources.Load<GameObject>("UI/BuildingButton"));
            button.name = $"{type}Button";
            button.GetComponent<Image>().sprite = atlas.GetSprite($"{type}");

            Type buildingType = GetType();
            BuildingController buildingController = GetBuildingController();
            button.GetComponent<Button>().onClick.AddListener(() => { 
                Debug.Log($"Setting current building to {type}");
                buildingController.SetCurrentBuildingToMultipleTypeBuilding<T>(buildingType, type);
                buildingController.SetCurrentAction(Actions.PLACE); 
                });
            buttons.Add(button);
        }
        return buttons.ToArray();
    }
}