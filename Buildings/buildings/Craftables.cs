// using System.Collections;
// using System.Collections.Generic;
// using System.Runtime.InteropServices;
// using System.Runtime.Remoting.Messaging;
// using UnityEngine;
// using static Utility.ClassManager;
// using static Utility.TilemapManager;
// using UnityEngine.U2D;
// using UnityEditor;
// using System;
// using UnityEngine.EventSystems;

// public class Craftables : Building, IExtraActionBuilding {

//     public enum Types {
//         Beehouse,
//         Keg,
//         Cask,
//         Furnace,
//         MayonnaiseMachine,
//         CheesePress,
//         SeedMaker,
//         Loom,
//         OilMaker,
//         RecyclingMachine,
//         WormBin,
//         PreservesJar,
//         CharcoalKiln,
//         LightningRod,
//         SlimeIncubator,
//         SlimeEggPress,
//         Crystalarium,
//         MiniObelisk,
//         FarmComputer,
//         OstrichIncubator,
//         GeodeCrusher,
//         SolarPanel,
//         Hopper,
//         BoneMill,
//         MushroomLog,
//         BaitMaker,
//         Dehydrator,
//         HeavyFurnace,
//         StatueOfBlessing,
//         DwarfStatue,
//         Anvil,
//         Forge,
//         FishSmoker,
//         DeluxeWormBin
//     }

//     private static int miniObeliskCount;
//     // public override string TooltipMessage => MultipleBuildingComponent.Type.ToString();
//     public MultipleTypeBuildingComponent MultipleBuildingComponent => gameObject.GetComponent<MultipleTypeBuildingComponent>();
//     public RangeEffectBuilding RangeEffectBuildingComponent { get; private set; }

//     public HashSet<ButtonTypes> BuildingInteractions => gameObject.GetComponent<InteractableBuildingComponent>().BuildingInteractions;

//     public GameObject ButtonParentGameObject => gameObject.GetComponent<InteractableBuildingComponent>().ButtonParentGameObject;

//     public override void OnAwake() {
//         BaseHeight = 1;
//         miniObeliskCount = 0;
//         base.OnAwake();
//         CanBeMassPlaced = true;
//         // gameObject.AddComponent<MultipleTypeBuildingComponent>().SetEnumType(typeof(Types));
//         // RangeEffectBuildingComponent = new RangeEffectBuilding(this);
//     }

//     protected void PerformExtraActionsOnPlacePreview(Vector3Int position) {
//         if (GetComponent<MultipleTypeBuildingComponent>().CurrentVariantIndex == (int)Types.MushroomLog) RangeEffectBuildingComponent.ShowEffectRange(GetAreaAroundPosition(position, 3).ToArray()); //7x7
//         if (GetComponent<MultipleTypeBuildingComponent>().CurrentVariantIndex == (int)Types.Beehouse) RangeEffectBuildingComponent.ShowEffectRange(GetRangeOfBeehouse(position).ToArray()); //look wiki
//     }

//     public void PerformExtraActionsOnPlace(Vector3Int position) {
//         if (GetComponent<MultipleTypeBuildingComponent>().CurrentVariantIndex == (int)Types.MiniObelisk) {
//             if (miniObeliskCount >= 2) { NotificationManager.Instance.SendNotification("You can only have 2 mini obelisks at a time", NotificationManager.Icons.ErrorIcon); return; }
//             else miniObeliskCount++;
//         }
//         RangeEffectBuildingComponent.HideEffectRange();
//     }

//     public void PerformExtraActionsOnDelete() {
//         // if (Convert.ToInt32(MultipleBuildingComponent.Type) == (int)Types.MiniObelisk) miniObeliskCount--;
//     }

//     // public override List<MaterialCostEntry> GetMaterialsNeeded() {
//     //     return MultipleBuildingComponent.Type switch {
//     //         Types.Beehouse => new List<MaterialCostEntry>(){
//     //             new(40, Materials.Wood),
//     //             new(8, Materials.Coal),
//     //             new(1, Materials.IronBar),
//     //             new(1, Materials.MapleSyrup)
//     //         },
//     //         Types.Keg => new List<MaterialCostEntry>(){
//     //             new(30, Materials.Wood),
//     //             new(1, Materials.CopperBar),
//     //             new(1, Materials.IronBar),
//     //             new(1, Materials.OakResin)
//     //         },
//     //         Types.Cask => new List<MaterialCostEntry>(){
//     //             new(20, Materials.Wood),
//     //             new(1, Materials.Hardwood),
//     //         },
//     //         Types.Furnace => new List<MaterialCostEntry>(){
//     //             new(20, Materials.CopperBar),
//     //             new(25, Materials.Stone),
//     //         },
//     //         Types.MayonnaiseMachine => new List<MaterialCostEntry>(){
//     //             new(15, Materials.Wood),
//     //             new(1, Materials.CopperBar),
//     //             new(1, Materials.EarthCrystal),
//     //             new(15, Materials.Stone)
//     //         },
//     //         Types.CheesePress => new List<MaterialCostEntry>(){
//     //             new(45, Materials.Wood),
//     //             new(45, Materials.Stone),
//     //             new(10, Materials.Hardwood),
//     //             new(1, Materials.CopperBar)
//     //         },
//     //         Types.SeedMaker => new List<MaterialCostEntry>(){
//     //             new(25, Materials.Wood),
//     //             new(10, Materials.Coal),
//     //             new(1, Materials.GoldBar),
//     //         },
//     //         Types.Loom => new List<MaterialCostEntry>(){
//     //             new(60, Materials.Wood),
//     //             new(30, Materials.Fiber),
//     //             new(1, Materials.PineTar),
//     //         },
//     //         Types.OilMaker => new List<MaterialCostEntry>(){
//     //             new(20, Materials.Hardwood),
//     //             new(1, Materials.GoldBar),
//     //             new(50, Materials.Slime)
//     //         },
//     //         Types.RecyclingMachine => new List<MaterialCostEntry>(){
//     //             new(25, Materials.Wood),
//     //             new(25, Materials.Stone),
//     //             new(1, Materials.IronBar),
//     //         },
//     //         Types.WormBin => new List<MaterialCostEntry>(){
//     //             new(15, Materials.Hardwood),
//     //             new(1, Materials.GoldBar),
//     //             new(1, Materials.IronBar),
//     //             new(50, Materials.Fiber)
//     //         },
//     //         Types.PreservesJar => new List<MaterialCostEntry>(){
//     //             new(50, Materials.Wood),
//     //             new(40, Materials.Stone),
//     //             new(8, Materials.Coal),
//     //         },
//     //         Types.CharcoalKiln => new List<MaterialCostEntry>(){
//     //             new(20, Materials.CopperBar),
//     //             new(2, Materials.CopperBar),
//     //         },
//     //         Types.LightningRod => new List<MaterialCostEntry>(){
//     //             new(1, Materials.IronBar),
//     //             new(1, Materials.RefinedQuartz),
//     //             new(1, Materials.BatWing),
//     //         },
//     //         Types.SlimeIncubator => new List<MaterialCostEntry>(){
//     //             new(2, Materials.IridiumBar),
//     //             new(100, Materials.Slime)
//     //         },
//     //         Types.SlimeEggPress => new List<MaterialCostEntry>(){
//     //             new(25, Materials.Coal),
//     //             new(1, Materials.FireQuartz),
//     //             new(1, Materials.BatteryPack),
//     //         },
//     //         Types.Crystalarium => new List<MaterialCostEntry>(){
//     //             new(99, Materials.Stone),
//     //             new(2, Materials.IridiumBar),
//     //             new(5, Materials.GoldBar),
//     //             new(1, Materials.BatteryPack),
//     //         },
//     //         Types.MiniObelisk => new List<MaterialCostEntry>(){
//     //             new(30, Materials.Hardwood),
//     //             new(20, Materials.SolarEssence),
//     //             new(3, Materials.GoldBar),
//     //         },
//     //         Types.FarmComputer => new List<MaterialCostEntry>(){
//     //             new(50, Materials.DwarfGadget),
//     //             new(1, Materials.BatteryPack),
//     //             new(10, Materials.RefinedQuartz),
//     //         },
//     //         Types.OstrichIncubator => new List<MaterialCostEntry>(){
//     //             new(50, Materials.BoneFragment),
//     //             new(50, Materials.Hardwood),
//     //             new(20, Materials.CinderShard),
//     //         },
//     //         Types.GeodeCrusher => new List<MaterialCostEntry>(){
//     //             new(2, Materials.GoldBar),
//     //             new(50, Materials.Stone),
//     //             new(1, Materials.Diamond),
//     //         },
//     //         Types.SolarPanel => new List<MaterialCostEntry>(){
//     //             new(5, Materials.GoldBar),
//     //             new(5, Materials.IronBar),
//     //             new(10, Materials.RefinedQuartz),
//     //         },
//     //         Types.Hopper => new List<MaterialCostEntry>(){
//     //             new(10, Materials.Hardwood),
//     //             new(1, Materials.IridiumBar),
//     //             new(1, Materials.RadioactiveBar),
//     //         },
//     //         Types.BoneMill => new List<MaterialCostEntry>(){
//     //             new(10, Materials.BoneFragment),
//     //             new(3, Materials.Clay),
//     //             new(20, Materials.Stone),
//     //         },
//     //         Types.MushroomLog => new List<MaterialCostEntry>(){
//     //             new(10, Materials.Hardwood),
//     //             new(10, Materials.Moss),
//     //         },
//     //         Types.BaitMaker => new List<MaterialCostEntry>(){
//     //             new(3, Materials.IronBar),
//     //             new(3, Materials.Coral),
//     //             new(20, Materials.SeaUrchin),
//     //         },
//     //         Types.Dehydrator => new List<MaterialCostEntry>(){
//     //             new(30, Materials.Wood),
//     //             new(2, Materials.Clay),
//     //             new(1, Materials.FireQuartz),
//     //         },
//     //         Types.HeavyFurnace => new List<MaterialCostEntry>(){
//     //             new(2, Materials.IronBar),
//     //             new(50 + 25 + 25, Materials.Stone), //requires 2 furnaces 25 stone and 20 copper each
//     //             new(20 + 20, Materials.CopperOre)
//     //         },
//     //         Types.StatueOfBlessing => new List<MaterialCostEntry>(){
//     //             new(999, Materials.Sap),
//     //             new(999, Materials.Stone),
//     //             new(999, Materials.Fiber),
//     //             new(333, Materials.Moss)
//     //         },
//     //         Types.DwarfStatue => new List<MaterialCostEntry>(){
//     //             new(20, Materials.IridiumBar),
//     //         },
//     //         Types.Anvil => new List<MaterialCostEntry>(){
//     //             new(50, Materials.IronBar),
//     //         },
//     //         Types.Forge => new List<MaterialCostEntry>(){
//     //             new(10, Materials.IronBar),
//     //             new(5, Materials.DragonTooth),
//     //             new(10, Materials.GoldBar),
//     //             new(5, Materials.IridiumBar),
//     //         },
//     //         Types.FishSmoker => new List<MaterialCostEntry>(){
//     //             new(10, Materials.Hardwood),
//     //             new(1, Materials.SeaJelly),
//     //             new(1, Materials.CaveJelly),
//     //             new(1, Materials.RiverJelly),
//     //         },
//     //         Types.DeluxeWormBin => new List<MaterialCostEntry>(){
//     //             new(30, Materials.Moss),
//     //             new(15, Materials.Hardwood),
//     //             new(1, Materials.GoldBar),
//     //             new(1, Materials.IronBar),
//     //             new(50, Materials.Fiber),
//     //         },
//     //         _ => throw new InvalidOperationException("Unexpected craftable type")
//     //     };
//     // }


//     public void OnMouseEnter() {
//         // if (Convert.ToInt32(MultipleBuildingComponent.Type) == (int)Types.MushroomLog) RangeEffectBuildingComponent.ShowEffectRange(GetAreaAroundPosition(BaseCoordinates[0], 3).ToArray());
//         // if (Convert.ToInt32(MultipleBuildingComponent.Type) == (int)Types.Beehouse) RangeEffectBuildingComponent.ShowEffectRange(GetRangeOfBeehouse(BaseCoordinates[0]).ToArray());
//     }

//     public void OnMouseExit() {
//         RangeEffectBuildingComponent.HideEffectRange();
//     }
// }
