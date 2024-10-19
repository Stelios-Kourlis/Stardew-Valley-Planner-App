// using System.Collections;
// using System.Collections.Generic;
// using System.Runtime.InteropServices;
// using UnityEngine;
// using UnityEngine.PlayerLoop;
// using UnityEngine.Tilemaps;
// using UnityEngine.U2D;
// using static Utility.SpriteManager;
// using static Utility.ClassManager;
// using static Utility.TilemapManager;
// using System.Runtime.Remoting.Messaging;
// using System;

// public class Scarecrow : Building, IExtraActionBuilding {

//     public enum Types {
//         Default,
//         Rarecrow1,
//         Rarecrow2,
//         Rarecrow3,
//         Rarecrow4,
//         Rarecrow5,
//         Rarecrow6,
//         Rarecrow7,
//         Rarecrow8,
//         Deluxe
//     }
//     private bool IsDeluxe { get; set; } = false;

//     public override string TooltipMessage {
//         get {
//             if (!IsDeluxe) return "Right Click To Cycle Scarecrow Type";
//             else return "";
//         }
//     }

//     public MultipleTypeBuildingComponent MultipleTypeBuildingComponent { get; private set; }

//     public RangeEffectBuilding RangeEffectBuildingComponent { get; private set; }

//     public HashSet<ButtonTypes> BuildingInteractions => gameObject.GetComponent<InteractableBuildingComponent>().BuildingInteractions;

//     public GameObject ButtonParentGameObject => gameObject.GetComponent<InteractableBuildingComponent>().ButtonParentGameObject;

//     public override void OnAwake() {
//         BuildingName = "Scarecrow";
//         BaseHeight = 1;
//         base.OnAwake();
//         // MultipleTypeBuildingComponent = gameObject.AddComponent<MultipleTypeBuildingComponent>().SetEnumType(typeof(Types));
//         // RangeEffectBuildingComponent = new RangeEffectBuilding(this);
//     }

//     // public void PerformExtraActionsOnPlacePreview(Vector3Int position) {
//     //     Vector3Int[] coverageArea = MultipleTypeBuildingComponent.Type switch {
//     //         Types.Deluxe => GetRangeOfDeluxeScarecrow(position).ToArray(),
//     //         _ => GetRangeOfScarecrow(position).ToArray()
//     //     };
//     //     RangeEffectBuildingComponent.ShowEffectRange(coverageArea);
//     // }

//     public void PerformExtraActionsOnPlace(Vector3Int position) {
//         RangeEffectBuildingComponent.HideEffectRange();
//     }

//     // public override List<MaterialCostEntry> GetMaterialsNeeded() {
//     //     return MultipleTypeBuildingComponent.Type switch {
//     //         Types.Deluxe => new List<MaterialCostEntry>{//Deluxe scarecrow
//     //             new(50, Materials.Wood),
//     //             new(1, Materials.IridiumOre),
//     //             new(40, Materials.Fiber)
//     //         },
//     //         Types.Default => new List<MaterialCostEntry>{//Normal scarecrow
//     //             new(50, Materials.Wood),
//     //             new(1, Materials.Coal),
//     //             new(20, Materials.Fiber)
//     //         },
//     //         Types.Rarecrow1 => new List<MaterialCostEntry>{//Rarecrows in order
//     //             new("Purchase at the Stardew Valley Fair for 800 Tokens"),
//     //         },
//     //         Types.Rarecrow2 => new List<MaterialCostEntry>{
//     //             new("	Purchase at the Spirit's Eve festival for 5,000 Coins"),
//     //         },
//     //         Types.Rarecrow3 => new List<MaterialCostEntry>{
//     //             new("Purchase at the Casino for 10,000 Qi Coins"),
//     //         },
//     //         Types.Rarecrow4 => new List<MaterialCostEntry>{
//     //             new("Purchase at the Traveling Cart randomly during fall or winter for 4,000 Coins, or purchase at the Festival of Ice for 5,000 Coins"),
//     //         },
//     //         Types.Rarecrow5 => new List<MaterialCostEntry>{
//     //             new("Purchase at the Flower Dance for 2,500 Coins"),
//     //         },
//     //         Types.Rarecrow6 => new List<MaterialCostEntry>{
//     //             new("Purchase from the Dwarf for 2,500 Coins"),
//     //         },
//     //         Types.Rarecrow7 => new List<MaterialCostEntry>{
//     //             new("Donate 20 Artifacts (not counting Minerals) to the Museum. Can be purchased from the Night Market once the first one is earned"),
//     //         },
//     //         Types.Rarecrow8 => new List<MaterialCostEntry>{
//     //             new("Donate 40 items to the Museum. Can be purchased from the Night Market once the first one is earned"),
//     //         },
//     //         _ => throw new System.ArgumentException($"Invalid scarecrow type {MultipleTypeBuildingComponent.Type}")
//     //     };
//     // }

//     public void OnMouseEnter() {
//         // Vector3Int[] coverageArea = MultipleTypeBuildingComponent.Type switch {
//         //     Types.Deluxe => GetRangeOfDeluxeScarecrow(BaseCoordinates[0]).ToArray(),
//         //     _ => GetRangeOfScarecrow(BaseCoordinates[0]).ToArray()
//         // };
//         // RangeEffectBuildingComponent.ShowEffectRange(coverageArea);
//     }

//     public void OnMouseExit() {
//         RangeEffectBuildingComponent.HideEffectRange();
//     }
// }
