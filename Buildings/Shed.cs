using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.U2D;

public class Shed : Building, ITieredBuilding, IEnterableBuilding {
    public TieredBuilding TieredBuildingComponent {get; private set;}
    public EnterableBuilding EnterableBuildingComponent {get; private set;}

    public int Tier => TieredBuildingComponent.Tier;
    public Vector3Int[] InteriorUnavailableCoordinates {get; private set;}

    public Vector3Int[] InteriorPlantableCoordinates {get; private set;}

    public override void OnAwake(){
        buildingName = "Shed";
        TieredBuildingComponent = new TieredBuilding(this, 2);
        EnterableBuildingComponent = new EnterableBuilding(this){
            interriorSprite = Resources.Load<Sprite>($"BuildingInsides/{buildingName}{Tier}")
        };
        BaseHeight = 3;
        BuildingInteractions = new ButtonTypes[]{
            ButtonTypes.TIER_ONE,
            ButtonTypes.TIER_TWO,
            ButtonTypes.PAINT,
            ButtonTypes.ENTER,
        };
        base.OnAwake();
    }

    public override List<MaterialInfo> GetMaterialsNeeded(){
        return TieredBuildingComponent.Tier switch{
            1 => new List<MaterialInfo>{
                new(15000, Materials.Coins),
                new(300, Materials.Wood),
            },
            2 => new List<MaterialInfo>{
                new(35000, Materials.Coins),
                new(850, Materials.Wood),
                new(300, Materials.Stone)
            },
            _ => throw new System.ArgumentException($"Invalid tier {TieredBuildingComponent.Tier}")
        };
    }
    
    public override string GetBuildingData(){
        return base.GetBuildingData() + $"|{TieredBuildingComponent.Tier}";
    }

    public override void RecreateBuildingForData(int x, int y, params string[] data){
        OnAwake();
        Place(new Vector3Int(x,y,0));
        TieredBuildingComponent.SetTier(int.Parse(data[0]));
    }

    public void SetTier(int tier) => TieredBuildingComponent.SetTier(tier);

    public void ToggleEditBuildingInterior() => EnterableBuildingComponent.ToggleEditBuildingInterior();

    public void EditBuildingInterior() => EnterableBuildingComponent.EditBuildingInterior();

    public void ExitBuildingInteriorEditing() => EnterableBuildingComponent.ExitBuildingInteriorEditing();
}
