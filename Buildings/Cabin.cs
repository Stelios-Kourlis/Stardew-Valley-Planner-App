using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.U2D;
using UnityEngine.UI;
using static Utility.ClassManager;

public class Cabin :Building, ITieredBuilding, IMultipleTypeBuilding<Cabin.Types> {

    public enum Types{
        Wood,
        Plank,
        Stone,
        Beach,
        Neighbor,
        Rustic,
        Trailer
    }
    private static readonly bool[] cabinTypeIsPlaced = new bool[Enum.GetValues(typeof(Types)).Length];
    private MultipleTypeBuilding<Types> multipleTypeBuildingComponent;
    private TieredBuilding tieredBuildingComponent;

    public int Tier => tieredBuildingComponent.Tier;

    private SpriteAtlas Atlas => multipleTypeBuildingComponent != null ? multipleTypeBuildingComponent.Atlas : tieredBuildingComponent.Atlas;

    public Types Type => multipleTypeBuildingComponent.Type;

    public static Types CurrentType {get; private set;}

    public override void OnAwake(){
        BaseHeight = 3;
        buildingName = "Cabin";
        for (int i = 0; i < cabinTypeIsPlaced.Length; i++) cabinTypeIsPlaced[i] = false;
        BuildingInteractions = new ButtonTypes[]{
            ButtonTypes.TIER_ONE,
            ButtonTypes.TIER_TWO,
            ButtonTypes.TIER_THREE,
            ButtonTypes.ENTER,
            ButtonTypes.PAINT
        };
        multipleTypeBuildingComponent = new MultipleTypeBuilding<Types>(this);
        multipleTypeBuildingComponent.DefaultSprite = multipleTypeBuildingComponent.Atlas.GetSprite($"{Types.Wood}1");
       
        tieredBuildingComponent = new TieredBuilding(this, 4);
        SetType(CurrentType);
        SetTier(1);
        base.OnAwake();
    } 

    public override void Place(Vector3Int position){
        if (cabinTypeIsPlaced[(int) Type]){
            GetNotificationManager().SendNotification("You can only have one of each type of cabin", NotificationManager.Icons.ErrorIcon);
            return;
        }
        base.Place(position);
        cabinTypeIsPlaced[(int) Type] = true;
    }

    public override void Delete(){
        cabinTypeIsPlaced[(int) Type] = false;
        base.Delete();
    }

    public override List<MaterialInfo> GetMaterialsNeeded(){
        List<MaterialInfo> level1 = new() { new(100, Materials.Coins) };
        return tieredBuildingComponent.Tier switch{
            2 => new List<MaterialInfo>{
                new(10_000, Materials.Coins),
                new(450, Materials.Wood),
            }.Union(level1).ToList(),
            3 => new List<MaterialInfo>{
                new(60_000, Materials.Coins),
                new(450, Materials.Wood),
                new(150, Materials.Hardwood),
            }.Union(level1).ToList(),
            4 => new List<MaterialInfo>{
                new(160_000, Materials.Coins),
                new(450, Materials.Wood),
                new(150, Materials.Hardwood),
            }.Union(level1).ToList(),
            _ => throw new System.Exception("Invalid Cabin Tier")
        };
    }

    public override string GetBuildingData(){
        return base.GetBuildingData() + $"|{tieredBuildingComponent.Tier}|{multipleTypeBuildingComponent.Type}";
    }

    public override void RecreateBuildingForData(int x, int y, params string[] data){
        tieredBuildingComponent.SetTier(int.Parse(data[0]));
        multipleTypeBuildingComponent.SetType((Types)Enum.Parse(typeof(Types), data[1]));
        Place(new Vector3Int(x,y,0));
    }

    public void SetTier(int tier){
        tieredBuildingComponent.SetTier(tier);
        Sprite sprite;
        Types type = multipleTypeBuildingComponent?.Type ?? Types.Wood;
        sprite = Atlas.GetSprite($"{type}{tier}");
        UpdateTexture(sprite);
    }

    public void CycleType() => multipleTypeBuildingComponent.CycleType();

    public void SetType(Types type){
        multipleTypeBuildingComponent.SetType(type);
        Sprite sprite;
        sprite = Atlas.GetSprite($"{type}{tieredBuildingComponent?.Tier ?? 1}");
        UpdateTexture(sprite);
    }

    public GameObject[] CreateButtonsForAllTypes(){
        List<GameObject> buttons = new();
        foreach (Types type in Enum.GetValues(typeof(Types))){
            GameObject button = Instantiate(Resources.Load<GameObject>("UI/BuildingButton"));
            button.name = $"{type}Button";
            button.GetComponent<Image>().sprite = multipleTypeBuildingComponent.Atlas.GetSprite($"{type}1");
            Type buildingType = GetType();
            BuildingController buildingController = GetBuildingController();
            button.GetComponent<Button>().onClick.AddListener(() => { 
                buildingController.SetCurrentBuildingToMultipleTypeBuilding(buildingType, type);
                CurrentAction = Actions.PLACE; 
                });
            buttons.Add(button);
        }
        return buttons.ToArray();
    }

}
