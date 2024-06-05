using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;
using UnityEngine;
using static Utility.ClassManager;
using static Utility.TilemapManager;
using UnityEngine.U2D;
using UnityEditor;
using System;

public class Craftables : Building, IMultipleTypeBuilding<Craftables.Types>, IRangeEffectBuilding, IExtraActionBuilding {

    public enum Types {
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
        MushroomLog,
        BaitMaker,
        Dehydrator,
        HeavyFurnace,
        StatueOfBlessing,
        DwarfStatue,
        Anvil,
        Forge,
        FishSmoker,
        DeluxeWormBin
    }

    public Types Type => MultipleBuildingComponent.Type;
    public Types CurrentType => MultipleBuildingComponent.Type;
    private static int miniObeliskCount;
    public override string TooltipMessage => Type.ToString();
    public MultipleTypeBuilding<Types> MultipleBuildingComponent { get; private set; }
    public RangeEffectBuilding RangeEffectBuildingComponent { get; private set; }

    public override void OnAwake() {
        BaseHeight = 1;
        miniObeliskCount = 0;
        base.OnAwake();
        MultipleBuildingComponent = new MultipleTypeBuilding<Types>(this);
        RangeEffectBuildingComponent = new RangeEffectBuilding(this);
    }

    protected void PerformExtraActionsOnPlacePreview(Vector3Int position) {
        if (Type == Types.MushroomLog) ShowEffectRange(GetAreaAroundPosition(position, 3).ToArray()); //7x7
        if (Type == Types.Beehouse) ShowEffectRange(GetRangeOfBeehouse(position).ToArray()); //look wiki
    }

    public void PerformExtraActionsOnPlace(Vector3Int position) {
        if (Type == Types.MiniObelisk) {
            if (miniObeliskCount >= 2) { GetNotificationManager().SendNotification("You can only have 2 mini obelisks at a time", NotificationManager.Icons.ErrorIcon); return; }
            else miniObeliskCount++;
        }
        HideEffectRange();
    }

    public void PerformExtraActionsOnDelete() {
        if (Type == Types.MiniObelisk) miniObeliskCount--;
    }

    public void SetType(Types type) => MultipleBuildingComponent.SetType(type);

    public override List<MaterialInfo> GetMaterialsNeeded() {
        return Type switch {
            Types.Beehouse => new List<MaterialInfo>(){
                new(40, Materials.Wood),
                new(8, Materials.Coal),
                new(1, Materials.IronBar),
                new(1, Materials.MapleSyrup)
            },
            Types.Keg => new List<MaterialInfo>(){
                new(30, Materials.Wood),
                new(1, Materials.CopperBar),
                new(1, Materials.IronBar),
                new(1, Materials.OakResin)
            },
            Types.Cask => new List<MaterialInfo>(){
                new(20, Materials.Wood),
                new(1, Materials.Hardwood),
            },
            Types.Furnace => new List<MaterialInfo>(){
                new(20, Materials.CopperBar),
                new(25, Materials.Stone),
            },
            Types.MayonnaiseMachine => new List<MaterialInfo>(){
                new(15, Materials.Wood),
                new(1, Materials.CopperBar),
                new(1, Materials.EarthCrystal),
                new(15, Materials.Stone)
            },
            Types.CheesePress => new List<MaterialInfo>(){
                new(45, Materials.Wood),
                new(45, Materials.Stone),
                new(10, Materials.Hardwood),
                new(1, Materials.CopperBar)
            },
            Types.SeedMaker => new List<MaterialInfo>(){
                new(25, Materials.Wood),
                new(10, Materials.Coal),
                new(1, Materials.GoldBar),
            },
            Types.Loom => new List<MaterialInfo>(){
                new(60, Materials.Wood),
                new(30, Materials.Fiber),
                new(1, Materials.PineTar),
            },
            Types.OilMaker => new List<MaterialInfo>(){
                new(20, Materials.Hardwood),
                new(1, Materials.GoldBar),
                new(50, Materials.Slime)
            },
            Types.RecyclingMachine => new List<MaterialInfo>(){
                new(25, Materials.Wood),
                new(25, Materials.Stone),
                new(1, Materials.IronBar),
            },
            Types.WormBin => new List<MaterialInfo>(){
                new(15, Materials.Hardwood),
                new(1, Materials.GoldBar),
                new(1, Materials.IronBar),
                new(50, Materials.Fiber)
            },
            Types.PreservesJar => new List<MaterialInfo>(){
                new(50, Materials.Wood),
                new(40, Materials.Stone),
                new(8, Materials.Coal),
            },
            Types.CharcoalKiln => new List<MaterialInfo>(){
                new(20, Materials.CopperBar),
                new(2, Materials.CopperBar),
            },
            Types.LightningRod => new List<MaterialInfo>(){
                new(1, Materials.IronBar),
                new(1, Materials.RefinedQuartz),
                new(1, Materials.BatWing),
            },
            Types.SlimeIncubator => new List<MaterialInfo>(){
                new(2, Materials.IridiumBar),
                new(100, Materials.Slime)
            },
            Types.SlimeEggPress => new List<MaterialInfo>(){
                new(25, Materials.Coal),
                new(1, Materials.FireQuartz),
                new(1, Materials.BatteryPack),
            },
            Types.Crystalarium => new List<MaterialInfo>(){
                new(99, Materials.Stone),
                new(2, Materials.IridiumBar),
                new(5, Materials.GoldBar),
                new(1, Materials.BatteryPack),
            },
            Types.MiniObelisk => new List<MaterialInfo>(){
                new(30, Materials.Hardwood),
                new(20, Materials.SolarEssence),
                new(3, Materials.GoldBar),
            },
            Types.FarmComputer => new List<MaterialInfo>(){
                new(50, Materials.DwarfGadget),
                new(1, Materials.BatteryPack),
                new(10, Materials.RefinedQuartz),
            },
            Types.OstrichIncubator => new List<MaterialInfo>(){
                new(50, Materials.BoneFragment),
                new(50, Materials.Hardwood),
                new(20, Materials.CinderShard),
            },
            Types.GeodeCrusher => new List<MaterialInfo>(){
                new(2, Materials.GoldBar),
                new(50, Materials.Stone),
                new(1, Materials.Diamond),
            },
            Types.SolarPanel => new List<MaterialInfo>(){
                new(5, Materials.GoldBar),
                new(5, Materials.IronBar),
                new(10, Materials.RefinedQuartz),
            },
            Types.Hopper => new List<MaterialInfo>(){
                new(10, Materials.Hardwood),
                new(1, Materials.IridiumBar),
                new(1, Materials.RadioactiveBar),
            },
            Types.BoneMill => new List<MaterialInfo>(){
                new(10, Materials.BoneFragment),
                new(3, Materials.Clay),
                new(20, Materials.Stone),
            },
            Types.MushroomLog => new List<MaterialInfo>(){
                new(10, Materials.Hardwood),
                new(10, Materials.Moss),
            },
            Types.BaitMaker => new List<MaterialInfo>(){
                new(3, Materials.IronBar),
                new(3, Materials.Coral),
                new(20, Materials.SeaUrchin),
            },
            Types.Dehydrator => new List<MaterialInfo>(){
                new(30, Materials.Wood),
                new(2, Materials.Clay),
                new(1, Materials.FireQuartz),
            },
            Types.HeavyFurnace => new List<MaterialInfo>(){
                new(2, Materials.IronBar),
                new(50 + 25 + 25, Materials.Stone), //requires 2 furnaces 25 stone and 20 copper each
                new(20 + 20, Materials.CopperOre)
            },
            Types.StatueOfBlessing => new List<MaterialInfo>(){
                new(999, Materials.Sap),
                new(999, Materials.Stone),
                new(999, Materials.Fiber),
                new(333, Materials.Moss)
            },
            Types.DwarfStatue => new List<MaterialInfo>(){
                new(20, Materials.IridiumBar),
            },
            Types.Anvil => new List<MaterialInfo>(){
                new(50, Materials.IronBar),
            },
            Types.Forge => new List<MaterialInfo>(){
                new(10, Materials.IronBar),
                new(5, Materials.DragonTooth),
                new(10, Materials.GoldBar),
                new(5, Materials.IridiumBar),
            },
            Types.FishSmoker => new List<MaterialInfo>(){
                new(10, Materials.Hardwood),
                new(1, Materials.SeaJelly),
                new(1, Materials.CaveJelly),
                new(1, Materials.RiverJelly),
            },
            Types.DeluxeWormBin => new List<MaterialInfo>(){
                new(30, Materials.Moss),
                new(15, Materials.Hardwood),
                new(1, Materials.GoldBar),
                new(1, Materials.IronBar),
                new(50, Materials.Fiber),
            },
            _ => throw new InvalidOperationException("Unexpected craftable type")
        };
    }

    public void LoadExtraBuildingData(string[] data) {
        SetType((Types)int.Parse(data[0]));
    }

    public string AddToBuildingData() {
        return $"{(int)Type}";
    }

    // protected override void OnMouseEnter() { //todo Add mouse enter/leave
    //     if (Type == Types.MushroomLog) ShowEffectRange(GetAreaAroundPosition(BaseCoordinates[0], 3).ToArray());
    //     if (Type == Types.Beehouse) ShowEffectRange(GetRangeOfBeehouse(BaseCoordinates[0]).ToArray());
    // }

    // protected override void OnMouseExit() {
    //     RangeEffectBuildingComponent.HideEffectRange();
    // }

    public void CycleType() => MultipleBuildingComponent.CycleType();

    public GameObject[] CreateButtonsForAllTypes() => MultipleBuildingComponent.CreateButtonsForAllTypes();

    public void ShowEffectRange(Vector3Int[] RangeArea) => RangeEffectBuildingComponent.ShowEffectRange(RangeArea);

    public void HideEffectRange() => RangeEffectBuildingComponent.HideEffectRange();
}
