using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;
using UnityEngine;
using static Utility.ClassManager;
using UnityEngine.U2D;
using UnityEditor;
using System;

public class Craftables : Building{

    public enum Type{
        Beehouse,
        Keg,
        Cask,
        Furnace,
        MayonnaiseMachine,
        CheesePress,
        SeedMaker,
        Loom,
        OilMaker,
        RecyclingMachine,
        WormBin,
        PreservesJar,
        CharcoalKiln,
        LightningRod,
        SlimeIncubator,
        SlimeEggPress,
        Crystalarium,
        MiniObelisk,
        FarmComputer,
        OstrichIncubator,
        GeodeCrusher,
        SolarPanel,
        Hopper,
        BoneMill,
}
    
    private SpriteAtlas atlas;
    public Type? CraftableType {get; private set;} = null;
    private static int miniObeliskCount;

    public void Awake(){
        baseHeight = 1;
        base.Start();
        atlas = Resources.Load<SpriteAtlas>("Buildings/Placeables/PlaceablesAtlas");
        if (CraftableType == null) SetCraftable(Type.Beehouse);
    } 

    public new void Update(){
        //Debug.Log($"Craftable Type is {CraftableType}");
        if (sprite == null){
            Debug.LogWarning("Sprite is null");
            SetCraftable((Type)CraftableType);
        }
        base.Update();
    }

    public override void Place(Vector3Int position){
        if (CraftableType == Type.MiniObelisk){
            if (miniObeliskCount >= 2) {GetNotificationManager().SendNotification("You can only have 2 mini obelisks at a time"); return;}
            else miniObeliskCount++;
        }
        base.Place(position);
    }

    public override void Delete(){
        if (CraftableType == Type.MiniObelisk) miniObeliskCount--;
        base.Delete();
    }

    public void SetCraftable(Type craftable){
        CraftableType = craftable;
        UpdateTexture(atlas.GetSprite(craftable.ToString()));
    }

    public override List<MaterialInfo> GetMaterialsNeeded(){
        return CraftableType switch{
            Type.Beehouse => new List<MaterialInfo>(){
                new MaterialInfo(40, Materials.Wood),
                new MaterialInfo(8, Materials.Coal),
                new MaterialInfo(1, Materials.IronBar),
                new MaterialInfo(1, Materials.MapleSyrup)
            },
            Type.Keg => new List<MaterialInfo>(){
                new MaterialInfo(30, Materials.Wood),
                new MaterialInfo(1, Materials.CopperBar),
                new MaterialInfo(1, Materials.IronBar),
                new MaterialInfo(1, Materials.OakResin)
            },
            Type.Cask => new List<MaterialInfo>(){
                new MaterialInfo(20, Materials.Wood),
                new MaterialInfo(1, Materials.Hardwood),
            },
            Type.Furnace => new List<MaterialInfo>(){
                new MaterialInfo(20, Materials.CopperBar),
                new MaterialInfo(25, Materials.Stone),
            },
            Type.MayonnaiseMachine => new List<MaterialInfo>(){
                new MaterialInfo(15, Materials.Wood),
                new MaterialInfo(1, Materials.CopperBar),
                new MaterialInfo(1, Materials.EarthCrystal),
                new MaterialInfo(15, Materials.Stone)
            },
            Type.CheesePress => new List<MaterialInfo>(){
                new MaterialInfo(45, Materials.Wood),
                new MaterialInfo(45, Materials.Stone),
                new MaterialInfo(10, Materials.Hardwood),
                new MaterialInfo(1, Materials.CopperBar)
            },
            Type.SeedMaker => new List<MaterialInfo>(){
                new MaterialInfo(25, Materials.Wood),
                new MaterialInfo(10, Materials.Coal),
                new MaterialInfo(1, Materials.GoldBar),
            },
            Type.Loom => new List<MaterialInfo>(){
                new MaterialInfo(60, Materials.Wood),
                new MaterialInfo(30, Materials.Fiber),
                new MaterialInfo(1, Materials.PineTar),
            },
            Type.OilMaker => new List<MaterialInfo>(){
                new MaterialInfo(20, Materials.Hardwood),
                new MaterialInfo(1, Materials.GoldBar),
                new MaterialInfo(50, Materials.Slime)
            },
            Type.RecyclingMachine => new List<MaterialInfo>(){
                new MaterialInfo(25, Materials.Wood),
                new MaterialInfo(25, Materials.Stone),
                new MaterialInfo(1, Materials.IronBar),
            },
            Type.WormBin => new List<MaterialInfo>(){
                new MaterialInfo(25, Materials.Hardwood),
                new MaterialInfo(1, Materials.GoldBar),
                new MaterialInfo(1, Materials.IronBar),
                new MaterialInfo(50, Materials.Fiber)
            },
            Type.PreservesJar => new List<MaterialInfo>(){
                new MaterialInfo(50, Materials.Wood),
                new MaterialInfo(40, Materials.Stone),
                new MaterialInfo(8, Materials.Coal),
            },
            Type.CharcoalKiln => new List<MaterialInfo>(){
                new MaterialInfo(20, Materials.CopperBar),
                new MaterialInfo(2, Materials.CopperBar),
            },
            Type.LightningRod => new List<MaterialInfo>(){
                new MaterialInfo(1, Materials.IronBar),
                new MaterialInfo(1, Materials.RefinedQuartz),
                new MaterialInfo(1, Materials.BatWing),
            },
            Type.SlimeIncubator => new List<MaterialInfo>(){
                new MaterialInfo(2, Materials.IridiumBar),
                new MaterialInfo(100, Materials.Slime)
            },
            Type.SlimeEggPress => new List<MaterialInfo>(){
                new MaterialInfo(25, Materials.Coal),
                new MaterialInfo(1, Materials.FireQuartz),
                new MaterialInfo(1, Materials.BatteryPack),
            },
            Type.Crystalarium => new List<MaterialInfo>(){
                new MaterialInfo(99, Materials.Stone),
                new MaterialInfo(2, Materials.IridiumBar),
                new MaterialInfo(5, Materials.GoldBar),
                new MaterialInfo(1, Materials.BatteryPack),
            },
            Type.MiniObelisk => new List<MaterialInfo>(){
                new MaterialInfo(30, Materials.Hardwood),
                new MaterialInfo(20, Materials.SolarEssence),
                new MaterialInfo(3, Materials.GoldBar),
            },
            Type.FarmComputer => new List<MaterialInfo>(){
                new MaterialInfo(50, Materials.DwarfGadget),
                new MaterialInfo(1, Materials.BatteryPack),
                new MaterialInfo(10, Materials.RefinedQuartz),
            },
            Type.OstrichIncubator => new List<MaterialInfo>(){
                new MaterialInfo(50, Materials.BoneFragment),
                new MaterialInfo(50, Materials.Hardwood),
                new MaterialInfo(20, Materials.CinderShard),
            },
            Type.GeodeCrusher => new List<MaterialInfo>(){
                new MaterialInfo(2, Materials.GoldBar),
                new MaterialInfo(50, Materials.Stone),
                new MaterialInfo(1, Materials.Diamond),
            },
            Type.SolarPanel => new List<MaterialInfo>(){
                new MaterialInfo(5, Materials.GoldBar),
                new MaterialInfo(5, Materials.IronBar),
                new MaterialInfo(10, Materials.RefinedQuartz),
            },
            Type.Hopper => new List<MaterialInfo>(){
                new MaterialInfo(10, Materials.Hardwood),
                new MaterialInfo(1, Materials.IridiumBar),
                new MaterialInfo(1, Materials.RadioactiveBar),
            },
            Type.BoneMill => new List<MaterialInfo>(){
                new MaterialInfo(10, Materials.BoneFragment),
                new MaterialInfo(3, Materials.Clay),
                new MaterialInfo(20, Materials.Stone),
            },
            _ => throw new InvalidOperationException("Unexpected craftable type")
        };
    }

    public override void RecreateBuildingForData(int x, int y, params string[] data){
        Place(new Vector3Int(x,y,0));
        CraftableType = (Type)int.Parse(data[4]);
    }

    public override string GetBuildingData(){
        return base.GetBuildingData() + $"|{(int)CraftableType}";
    }

}
