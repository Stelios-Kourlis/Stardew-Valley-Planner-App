using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;
using static Utility.BuildingManager;
using static Utility.ClassManager;

public class MultipleTypeBuilding<T> where T : Enum{
    public T Type {get; set;}
    public static T CurrentType {get; set;}
    public SpriteAtlas Atlas {get; private set;}
    public Sprite DefaultSprite {get; set;}
    public Building Building {get; private set;}
    private readonly bool buildingHasOtherInterfaces;

    public MultipleTypeBuilding(Building building){
        if (!typeof(T).IsEnum) throw new ArgumentException("T must be an enumerated type");
        Building = building;
        buildingHasOtherInterfaces = BuildingHasMoreThanOneBuildingInterface(Building, typeof(IMultipleTypeBuilding<>));
        Atlas = Resources.Load<SpriteAtlas>($"Buildings/{Building.GetType().Name}Atlas");
        Debug.Assert(Atlas != null, $"Atlas \"{Building.GetType().Name}Atlas\" was not found");
        if (buildingHasOtherInterfaces) return;
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
        if (buildingHasOtherInterfaces) return;
        Sprite sprite = Atlas.GetSprite($"{type}");
        Debug.Assert(sprite != null, $"Sprite {type} was not found in {Building.GetType()}Atlas");
        Building.UpdateTexture(sprite);
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
            GameObject button = GameObject.Instantiate(Resources.Load<GameObject>("UI/BuildingButton"));
            button.name = $"{type}";
            button.GetComponent<Image>().sprite = Atlas.GetSprite($"{type}");

            Type buildingType = Building.GetType();
            BuildingController buildingController = GetBuildingController();
            button.GetComponent<Button>().onClick.AddListener(() => { 
                buildingController.SetCurrentBuildingToMultipleTypeBuilding(buildingType, type);
                Building.CurrentAction = Actions.PLACE; 
                });
            buttons.Add(button);
        }
        return buttons.ToArray();
    }
}