using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.U2D;
using static Utility.TilemapManager;
using static Utility.ClassManager;
using static Utility.SpriteManager;
using UnityEngine.Tilemaps;

public class Fence : Building, IMultipleTypeBuilding<Fence.Types>, IConnectingBuilding{

    public enum Types{
        Wood,
        Stone,
        Iron,
        Hardwood
    }

    public override string TooltipMessage => "";
    public MultipleTypeBuilding<Types> MultipleTypeBuildingComponent {get; private set;}
    public ConnectingBuilding ConnectingBuildingComponent {get; private set;}
    public SpriteAtlas Atlas => MultipleTypeBuildingComponent.Atlas;
    public Types CurrentType {get; private set;}
    public Types Type {get; private set;}
    public static event Action<Vector3Int> FenceWasPlaced;
    private static readonly List<Vector3Int> otherFences = new();

    public override void OnAwake(){
        BaseHeight = 1;
        base.OnAwake();
        MultipleTypeBuildingComponent = new MultipleTypeBuilding<Types>(this);
        ConnectingBuildingComponent = new ConnectingBuilding();
        MultipleTypeBuildingComponent.DefaultSprite = Atlas.GetSprite($"WoodFence0");
        SetType(CurrentType);
        FenceWasPlaced += AnotherFenceWasPlaced;
    }

    public override List<MaterialInfo> GetMaterialsNeeded(){
        return MultipleTypeBuildingComponent.Type switch{
            Types.Wood => new List<MaterialInfo>(){
                new(2, Materials.Wood)
            },
            Types.Stone => new List<MaterialInfo>(){
                new(2, Materials.Stone)
            },
            Types.Iron => new List<MaterialInfo>(){
                new(1, Materials.IronBar)
            },
            Types.Hardwood => new List<MaterialInfo>(){
                new(1, Materials.Hardwood)
            },
            _ => throw new System.Exception("Invalid Fence Type")
        };
    }

    public override void RecreateBuildingForData(int x, int y, params string[] data){
        Place(new Vector3Int(x,y,0));
        SetType((Types)Enum.Parse(typeof(Types), data[0]));
        UpdateTexture(Atlas.GetSprite($"{Type}Fence0"));
    }



    public override void Place(Vector3Int position){
        base.Place(position);
        otherFences.Add(position);
        FenceWasPlaced?.Invoke(position);
    }

    protected override void PlacePreview(Vector3Int position){
        base.PlacePreview(position);    
        UpdateTexture(MultipleTypeBuildingComponent.Atlas.GetSprite($"{Type}{ConnectingBuildingComponent.GetConnectingFlagsNoTop(position, otherFences)}"));
    }

    public override void Delete(){
        otherFences.Remove(BaseCoordinates[0]);
        FenceWasPlaced -= AnotherFenceWasPlaced;
        FenceWasPlaced?.Invoke(BaseCoordinates[0]);
        base.Delete();
    }

    private void AnotherFenceWasPlaced(Vector3Int position){
        if (BaseCoordinates?.Contains(position) ?? true) return;
        UpdateTexture(MultipleTypeBuildingComponent.Atlas.GetSprite($"{Type}{ConnectingBuildingComponent.GetConnectingFlags(BaseCoordinates[0], otherFences)}"));
    }

    public GameObject[] CreateButtonsForAllTypes(){
        List<GameObject> buttons = new();
        foreach (Types type in Enum.GetValues(typeof(Types))){
            if ((int)(object)type == 0) continue; // skip the first element (None)
            GameObject button = Instantiate(Resources.Load<GameObject>("UI/BuildingButton"));
            button.name = $"{type}Button";
            button.GetComponent<Image>().sprite = Atlas.GetSprite($"{type}Fence0");

            Type buildingType = GetType();
            BuildingController buildingController = GetBuildingController();
            button.GetComponent<Button>().onClick.AddListener(() => { 
                Debug.Log($"Setting current building to {type}");
                buildingController.SetCurrentBuildingToMultipleTypeBuilding<Types>(buildingType, type);
                buildingController.SetCurrentAction(Actions.PLACE); 
                });
            buttons.Add(button);
        }
        return buttons.ToArray();
    }

    public int GetConnectingFlags(Vector3Int position, List<Vector3Int> otherBuildings) => ConnectingBuildingComponent.GetConnectingFlags(position, otherBuildings);

    public void CycleType(){
        int enumLength = Enum.GetValues(typeof(Types)).Length;
        int intValue = (int) Type;
        intValue = (intValue + 1) % enumLength;
        SetType((Types)Enum.ToObject(typeof(Types), intValue));
    }

    public void SetType(Types type){
        CurrentType = type;
        Type = type;
        UpdateTexture(MultipleTypeBuildingComponent.Atlas.GetSprite($"{type}Fence0"));
    }
}
