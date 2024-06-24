// using System;
// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.Tilemaps;
// using UnityEngine.U2D;
// using static Utility.TilemapManager;
// using static Utility.ClassManager;
// using static Utility.SpriteManager;
// using UnityEngine.UI;
// using System.Linq;

// public class Sprinkler : Building, IMultipleTypeBuilding, IRangeEffectBuilding, IExtraActionBuilding {

//     public enum Types {
//         Normal,
//         Quality,
//         Iridium,
//     }
//     public MultipleTypeBuildingComponent MultipleBuildingComponent { get; private set; }
//     public SpriteAtlas Atlas => MultipleBuildingComponent.Atlas;
//     public RangeEffectBuilding RangeEffectBuildingComponent { get; private set; }
//     // public Types CurrentType { get; private set; }
//     public Enum Type => MultipleBuildingComponent.Type;

//     public List<ButtonTypes> BuildingInteractions => gameObject.GetComponent<InteractableBuildingComponent>().BuildingInteractions;

//     public GameObject ButtonParentGameObject => gameObject.GetComponent<InteractableBuildingComponent>().ButtonParentGameObject;

//     private bool hasPressureNozzle;
//     private bool hasEnricher;

//     public override void OnAwake() {
//         BaseHeight = 1;
//         BuildingName = "Sprinkler";
//         base.OnAwake();
//         MultipleBuildingComponent = gameObject.AddComponent<MultipleTypeBuildingComponent>().SetEnumType(typeof(Types));
//         RangeEffectBuildingComponent = new RangeEffectBuilding(this);
//     }

//     protected void PerformExtraActionsOnPlacePreview(Vector3Int position) {
//         Vector3Int[] coverageArea = MultipleBuildingComponent.Type switch {
//             Types.Normal => !hasPressureNozzle ? GetCrossAroundPosition(position).ToArray() : GetAreaAroundPosition(position, 1).ToArray(),
//             Types.Quality => !hasPressureNozzle ? GetAreaAroundPosition(position, 1).ToArray() : GetAreaAroundPosition(position, 2).ToArray(),
//             Types.Iridium => !hasPressureNozzle ? GetAreaAroundPosition(position, 2).ToArray() : GetAreaAroundPosition(position, 3).ToArray(),
//             _ => throw new System.ArgumentException($"Invalid type {MultipleBuildingComponent.Type}")
//         };
//         RangeEffectBuildingComponent.ShowEffectRange(coverageArea);
//     }

//     public void PerformExtraActionsOnPlace(Vector3Int position) {
//         RangeEffectBuildingComponent.HideEffectRange();
//     }

//     public override List<MaterialInfo> GetMaterialsNeeded() {
//         return MultipleBuildingComponent.Type switch {
//             Types.Normal => new List<MaterialInfo>{
//                     new(1, Materials.IronBar),
//                     new(1, Materials.CopperBar),
//                 },
//             Types.Quality => new List<MaterialInfo>{
//                     new(1, Materials.GoldBar),
//                     new(1, Materials.IronBar),
//                     new(1, Materials.RefinedQuartz)
//                 },
//             Types.Iridium => new List<MaterialInfo>{
//                     new(1, Materials.IridiumBar),
//                     new(1, Materials.GoldBar),
//                     new(1, Materials.BatteryPack),
//                 },
//             _ => throw new System.ArgumentException($"Invalid type {MultipleBuildingComponent.Type}")
//         };
//     }

//     public string AddToBuildingData() {
//         return $"{MultipleBuildingComponent.Type}";
//     }

//     public void LoadExtraBuildingData(string[] data) {
//         SetType((Types)Enum.Parse(typeof(Types), data[0]));
//     }

//     protected void OnMouseRightClick() { //todo fix
//         if (hasPressureNozzle) {
//             hasPressureNozzle = false;
//             hasEnricher = true;
//             UpdateTexture(Atlas.GetSprite($"{Type}Enricher"));
//         }
//         else if (hasEnricher) {
//             hasEnricher = false;
//             UpdateTexture(Atlas.GetSprite($"{Type}"));
//         }
//         else {
//             hasPressureNozzle = true;
//             UpdateTexture(Atlas.GetSprite($"{Type}PressureNozzle"));
//         }
//         OnMouseEnter();
//     }

//     public void ShowEffectRange(Vector3Int[] RangeArea) => RangeEffectBuildingComponent.ShowEffectRange(RangeArea);

//     public void HideEffectRange() => RangeEffectBuildingComponent.HideEffectRange();
//     public void CycleType() => MultipleBuildingComponent.CycleType();
//     public void SetType(Enum type) => MultipleBuildingComponent.SetType(type);
//     public GameObject[] CreateButtonsForAllTypes() => MultipleBuildingComponent.CreateButtonsForAllTypes();

//     public void OnMouseEnter() {
//         Vector3Int position = BaseCoordinates[0];
//         Vector3Int[] coverageArea = MultipleBuildingComponent.Type switch {
//             Types.Normal => !hasPressureNozzle ? GetCrossAroundPosition(position).ToArray() : GetAreaAroundPosition(position, 1).ToArray(),
//             Types.Quality => !hasPressureNozzle ? GetAreaAroundPosition(position, 1).ToArray() : GetAreaAroundPosition(position, 2).ToArray(),
//             Types.Iridium => !hasPressureNozzle ? GetAreaAroundPosition(position, 2).ToArray() : GetAreaAroundPosition(position, 3).ToArray(),
//             _ => throw new System.ArgumentException($"Invalid type {MultipleBuildingComponent.Type}")
//         };
//         RangeEffectBuildingComponent.ShowEffectRange(coverageArea);
//     }

//     public void OnMouseExit() {
//         RangeEffectBuildingComponent.HideEffectRange();
//     }
// }
