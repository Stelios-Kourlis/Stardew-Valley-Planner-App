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

public class MultipleTypeBuilding<T> where T : Enum{
    public T Type {get; private set;}
    public static T CurrentType {get; private set;}
    private SpriteAtlas Atlas {get; set;}
    public Sprite DefaultSprite {get; private set;}
    public Building Building {get; private set;}

    public MultipleTypeBuilding(Building building){
        if (!typeof(T).IsEnum) throw new ArgumentException("T must be an enumerated type");
        Building = building;
        Atlas = Resources.Load<SpriteAtlas>($"Buildings/{Building.GetType().Name}sAtlas");
        Debug.Assert(Atlas != null, $"Atlas \"{Building.GetType().Name}sAtlas\" was not found");
        SetType(CurrentType);
        DefaultSprite = Atlas.GetSprite($"{Enum.GetValues(typeof(T)).GetValue(0)}");
    }

    public void CycleType(){
        int enumLength = Enum.GetValues(typeof(T)).Length;
        int intValue = Convert.ToInt32(Type);
        intValue = (intValue + 1) % enumLength;
        SetType((T)Enum.ToObject(typeof(T), intValue));
    }

    public void SetType(T type){
        CurrentType = type;
        Type = type;
        Building.UpdateTexture(Atlas.GetSprite($"{type}"));
        Debug.Assert(Atlas.GetSprite($"{type}") != null, $"Sprite for {type} was not found in {Atlas.name}");
    }

    public GameObject CreateButton(){
        GameObject button = Building.CreateButton();
        button.GetComponent<Image>().sprite = DefaultSprite;
        return button;
    }

    /// <summary>
    /// Creates a button for each type of the building
    /// </summary>
    /// <returns>An array with a button for each type, with no parent, caller should call transform.SetParent()</returns>
    public virtual GameObject[] CreateButtonsForAllTypes(){
        List<GameObject> buttons = new();
        foreach (T type in Enum.GetValues(typeof(T))){
            // if ((int)(object)type == 0) continue; // skip the first element (None)
            GameObject button = GameObject.Instantiate(Resources.Load<GameObject>("UI/BuildingButton"));
            button.name = $"{type}Button";
            button.GetComponent<Image>().sprite = Atlas.GetSprite($"{type}");

            Type buildingType = Building.GetType();
            BuildingController buildingController = GetBuildingController();
            button.GetComponent<Button>().onClick.AddListener(() => { 
                buildingController.SetCurrentBuildingToMultipleTypeBuilding(buildingType, type);
                buildingController.SetCurrentAction(Actions.PLACE); 
                });
            buttons.Add(button);
        }
        return buttons.ToArray();
    }

    public static explicit operator MultipleTypeBuilding<T>(Component v)
    {
        throw new NotImplementedException();
    }
}