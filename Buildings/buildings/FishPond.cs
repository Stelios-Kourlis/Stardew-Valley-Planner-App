// using System;
// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.Tilemaps;
// using UnityEngine.U2D;
// using UnityEngine.UI;
// using static Utility.TilemapManager;
// using static Utility.SpriteManager;
// using static Utility.ClassManager;
// using static Utility.BuildingManager;
// using System.Linq;


// public class FishPond : Building, IExtraActionBuilding {
//     public FishPondComponent FishPondComponent => gameObject.GetComponent<FishPondComponent>();
//     public HashSet<ButtonTypes> BuildingInteractions => gameObject.GetComponent<InteractableBuildingComponent>().BuildingInteractions;
//     public GameObject ButtonParentGameObject => gameObject.GetComponent<InteractableBuildingComponent>().ButtonParentGameObject;

//     public override void OnAwake() {
//         BuildingName = "Fish Pond";
//         BaseHeight = 5;
//         HidBuildingPreview += HideBuildingPreviewComp;
//         base.OnAwake();
//         // gameObject.AddComponent<FishPondComponent>();


//     }

//     // public override List<MaterialCostEntry> GetMaterialsNeeded() {
//     //     return new List<MaterialCostEntry> {
//     //         new(5_000, Materials.Coins),
//     //         new(200, Materials.Wood),
//     //         new(5, Materials.Seaweed),
//     //         new(5, Materials.GreenAlgae)
//     //     };
//     // }

//     public void HideBuildingPreviewComp() {
//         if (CurrentBuildingState == BuildingState.PLACED) {
//             FishPondComponent.SetDecoTilemapAlpha(1);
//             FishPondComponent.SetWaterTilemapAlpha(1);
//         }
//         else {
//             FishPondComponent.ClearDecoTilemap();
//             FishPondComponent.ClearWaterTilemap();
//         }
//     }

//     public void PerformExtraActionsOnPlace(Vector3Int position) {
//         FishPondComponent.SetDecoTilemapLocation(position); //just copy the color of the tilemap
//         FishPondComponent.SetDecoTilemapAlpha(1);
//         FishPondComponent.SetWaterTilemapLocation(position);
//         FishPondComponent.SetWaterTilemapAlpha(1);
//     }

//     public void PerformExtraActionsOnPickup() {
//         FishPondComponent.ClearDecoTilemap();
//         FishPondComponent.ClearWaterTilemap();
//     }

//     public void PerformExtraActionsOnPlacePreview(Vector3Int position) {
//         FishPondComponent.SetDecoTilemapLocation(position); //just copy the color of the tilemap
//         FishPondComponent.SetDecoTilemapAlpha(Tilemap.color.a);
//         FishPondComponent.SetWaterTilemapLocation(position);
//         FishPondComponent.SetWaterTilemapAlpha(Tilemap.color.a);
//     }

//     public void PerformExtraActionsOnPickupPreview() {
//         if (CurrentBuildingState == BuildingState.PLACED) {
//             FishPondComponent.SetDecoTilemapAlpha(Tilemap.color.a);
//             FishPondComponent.SetWaterTilemapAlpha(Tilemap.color.a);
//         }
//         else {
//             FishPondComponent.ClearDecoTilemap();
//             FishPondComponent.ClearWaterTilemap();
//         }
//     }

//     public void PerformExtraActionsOnDeletePreview() {
//         Debug.Log($"PerformExtraActionsOnDeletePreview ({CurrentBuildingState})");
//         if (CurrentBuildingState == BuildingState.PLACED) {
//             FishPondComponent.SetDecoTilemapAlpha(Tilemap.color.a);
//             FishPondComponent.SetWaterTilemapAlpha(Tilemap.color.a);
//         }
//         else {
//             FishPondComponent.ClearDecoTilemap();
//             FishPondComponent.ClearWaterTilemap();
//         }
//     }
// }
