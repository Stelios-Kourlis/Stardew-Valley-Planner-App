// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.Tilemaps;

// public class ShippingBin : Building, IExtraActionBuilding {

//     private static bool FreeShippingBinPlaced = false;
//     private bool isFree;

//     public override void OnAwake() {
//         BuildingName = "Shipping Bin";
//         BaseHeight = 1;
//         isFree = true;
//         base.OnAwake();
//     }

//     public void PerformExtraActionsOnPlace(Vector3Int position) {
//         if (!FreeShippingBinPlaced) isFree = true;
//         else isFree = false;
//         FreeShippingBinPlaced = true;
//     }

//     // public override List<MaterialCostEntry> GetMaterialsNeeded() {
//     //     if (isFree) return new List<MaterialCostEntry>() { new("Free") };
//     //     return new List<MaterialCostEntry>(){
//     //         new(250, Materials.Coins),
//     //         new(150, Materials.Wood)
//     //     };
//     // }
// }