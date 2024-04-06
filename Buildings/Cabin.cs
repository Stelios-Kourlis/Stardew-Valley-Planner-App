using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.U2D;
using static Utility.ClassManager;

public class Cabin : TieredBuilding {//todo: add cabin types

    public enum CabinTypes{//add types
        NO_TYPE,
        Wood,
        Plank,
        Stone
    }

    private CabinTypes type = CabinTypes.Stone;
    private static bool WoodCabinHasBeenPlaced;
    private static bool PlankCabinHasBeenPlaced;
    public static bool StoneCabinHasBeenPlaced;

    public override string TooltipMessage => "Right Click For More Options";

    public override void OnAwake(){
        baseHeight = 3;
        buildingInteractions = new ButtonTypes[]{
            ButtonTypes.TIER_ONE,
            ButtonTypes.TIER_TWO,
            ButtonTypes.TIER_THREE,
            ButtonTypes.ENTER,
            ButtonTypes.PAINT
        };
        MaxTier = 3;
        base.OnAwake();
    }

    public void CycleType(){
        type = type switch{
            CabinTypes.Wood => CabinTypes.Plank,
            CabinTypes.Plank => CabinTypes.Stone,
            CabinTypes.Stone => CabinTypes.Wood,
            _ => throw new System.Exception("Invalid Cabin Type")
        };
    }

    public void SetType(CabinTypes type){
        this.type = type;
    }   

    public override void Place(Vector3Int position){
        if (type == CabinTypes.Wood){
            if (WoodCabinHasBeenPlaced) {GetNotificationManager().SendNotification("Wood Cabin Has Already Been Placed"); return;}
            WoodCabinHasBeenPlaced = true;
        }else if (type == CabinTypes.Plank){
            if (PlankCabinHasBeenPlaced) {GetNotificationManager().SendNotification("Plank Cabin Has Already Been Placed"); return;}
            PlankCabinHasBeenPlaced = true;
        }else if (type == CabinTypes.Stone){
            Debug.Log($"Had Stone Cabin Been Placed: {StoneCabinHasBeenPlaced}");
            if (StoneCabinHasBeenPlaced) {GetNotificationManager().SendNotification("Stone Cabin Has Already Been Placed"); return;}
            else{
                StoneCabinHasBeenPlaced = true;
                Debug.Log($"Placed Stone Cabin {StoneCabinHasBeenPlaced}");
            }
        }
        base.Place(position);
    }

    public override void Delete(){
        if (type == CabinTypes.Wood) WoodCabinHasBeenPlaced = false;
        else if (type == CabinTypes.Plank) PlankCabinHasBeenPlaced = false;
        else if (type == CabinTypes.Stone) StoneCabinHasBeenPlaced = false;
        base.Delete();
    }

    public override List<MaterialInfo> GetMaterialsNeeded(){
        List<MaterialInfo> level1 = type switch{
            CabinTypes.Wood => new List<MaterialInfo>(){
                new MaterialInfo(10, Materials.Wood),
            },
            CabinTypes.Plank => new List<MaterialInfo>(){
                new MaterialInfo(5, Materials.Wood),
                new MaterialInfo(10, Materials.Fiber)

            },
            CabinTypes.Stone => new List<MaterialInfo>(){
                new MaterialInfo(10, Materials.Stone),
            },
            _ => throw new System.Exception("Invalid Cabin Type")
        };
        return Tier switch{
            2 => new List<MaterialInfo>{
                new MaterialInfo(10_000, Materials.Coins),
                new MaterialInfo(450, Materials.Wood),
            }.Union(level1).ToList(),
            3 => new List<MaterialInfo>{
                new MaterialInfo(60_000, Materials.Coins),
                new MaterialInfo(450, Materials.Wood),
                new MaterialInfo(150, Materials.Hardwood),
            }.Union(level1).ToList(),
            4 => new List<MaterialInfo>{
                new MaterialInfo(160_000, Materials.Coins),
                new MaterialInfo(450, Materials.Wood),
                new MaterialInfo(150, Materials.Hardwood),
            }.Union(level1).ToList(),
            _ => throw new System.Exception("Invalid Cabin Tier")
        };
    }

    public override string GetBuildingData(){
        return base.GetBuildingData() + $"|{Tier}|{type}";
    }

    public override void RecreateBuildingForData(int x, int y, params string[] data){
        Tier = int.Parse(data[0]);
        type = (CabinTypes)Enum.Parse(typeof(CabinTypes), data[1]);
        Place(new Vector3Int(x,y,0));
    }
}
