// using System;
// using System.Collections;
// using System.Collections.Generic;
// using System.Linq;
// using UnityEngine;
// using UnityEngine.UI;
// using UnityEngine.U2D;
// using static Utility.TilemapManager;
// using static Utility.ClassManager;
// using static Utility.SpriteManager;
// using UnityEngine.Tilemaps;

// public class Fence : Building, IConnectingBuilding {

//     public enum Types {
//         Wood,
//         Stone,
//         Iron,
//         Hardwood
//     }

//     public override string TooltipMessage => "";
//     public MultipleTypeBuildingComponent MultipleTypeBuildingComponent { get; private set; }
//     public ConnectingBuildingComponent ConnectingBuildingComponent { get; private set; }
//     public SpriteAtlas Atlas => MultipleTypeBuildingComponent.Atlas;

//     public HashSet<ButtonTypes> BuildingInteractions => gameObject.GetComponent<InteractableBuildingComponent>().BuildingInteractions;

//     public GameObject ButtonParentGameObject => gameObject.GetComponent<InteractableBuildingComponent>().ButtonParentGameObject;

//     public override void OnAwake() {
//         BaseHeight = 1;
//         BuildingName = "Fence";
//         base.OnAwake();
//         CanBeMassPlaced = true;
//         // MultipleTypeBuildingComponent = gameObject.AddComponent<MultipleTypeBuildingComponent>().SetEnumType(typeof(Types));
//         // ConnectingBuildingComponent = gameObject.AddComponent<ConnectingBuildingComponent>();
//         BuildingPlaced += _ => gameObject.GetComponent<ConnectingBuildingComponent>().UpdateAllOtherBuildingOfSameType();
//         BuildingRemoved += gameObject.GetComponent<ConnectingBuildingComponent>().UpdateAllOtherBuildingOfSameType;
//     }

//     // public override List<MaterialCostEntry> GetMaterialsNeeded() {
//     //     return MultipleTypeBuildingComponent.Type switch {
//     //         Types.Wood => new List<MaterialCostEntry>(){
//     //             new(2, Materials.Wood)
//     //         },
//     //         Types.Stone => new List<MaterialCostEntry>(){
//     //             new(2, Materials.Stone)
//     //         },
//     //         Types.Iron => new List<MaterialCostEntry>(){
//     //             new(1, Materials.IronBar)
//     //         },
//     //         Types.Hardwood => new List<MaterialCostEntry>(){
//     //             new(1, Materials.Hardwood)
//     //         },
//     //         _ => throw new System.Exception("Invalid Fence Type")
//     //     };
//     // }



//     public void PerformExtraActionsOnPlace(Vector3Int position) {
//         UpdateTexture(MultipleTypeBuildingComponent.Atlas.GetSprite($"{gameObject.GetComponent<InteractableBuildingComponent>().GetBuildingSpriteName()}"));
//     }

//     protected void PerformExtraActionsOnPlacePreview(Vector3Int position) {
//         UpdateTexture(MultipleTypeBuildingComponent.Atlas.GetSprite($"{gameObject.GetComponent<InteractableBuildingComponent>().GetBuildingSpriteName()}"));
//     }

//     public void PerformExtraActionsOnDelete() {
//         BuildingPlaced -= _ => gameObject.GetComponent<ConnectingBuildingComponent>().UpdateAllOtherBuildingOfSameType();
//     }

//     public void UpdateSelf() {
//         UpdateTexture(MultipleTypeBuildingComponent.Atlas.GetSprite($"{gameObject.GetComponent<InteractableBuildingComponent>().GetBuildingSpriteName()}"));
//     }
// }
