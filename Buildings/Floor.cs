// using System;
// using System.Collections;
// using System.Collections.Generic;
// using System.Data.Common;
// using System.Linq;
// using UnityEngine.UI;
// using UnityEngine;
// using UnityEngine.Tilemaps;
// using static Utility.ClassManager;
// using static Utility.SpriteManager;
// using static Utility.TilemapManager;

// public class Floor : Building, IMultipleTypeBuilding<Floor.Types>, IConnectingBuilding {

//     public enum Types {
//         WOOD_FLOOR,
//         RUSTIC_PLANK_FLOOR,
//         STRAW_FLOOR,
//         WEATHERED_FLOOR,
//         CRYSTAL_FLOOR,
//         STONE_FLOOR,
//         STONE_WALKWAY_FLOOR,
//         BRICK_FLOOR,
//         WOOD_PATH,
//         GRAVEL_PATH,
//         COBBLESTONE_PATH,
//         STEPPING_STONE_PATH,
//         CRYSTAL_PATH
//     }

//     public static event Action<Vector3Int> FloorWasPlaced;
//     public static List<Vector3Int> OtherFloors { get; private set; }
//     public override string TooltipMessage => "";
//     public MultipleTypeBuilding<Types> MultipleTypeBuildingComponent { get; private set; }
//     public ConnectingBuilding ConnectingBuildingComponent { get; private set; }
//     public Types Type { get; private set; }
//     public static Types CurrentType { get; private set; }

//     public override void OnAwake() {
//         BaseHeight = 1;
//         base.OnAwake();
//         BuildingName = "Floor";
//         OtherFloors ??= new List<Vector3Int>();
//         FloorWasPlaced += AnotherFloorPlaced;
//         MultipleTypeBuildingComponent = gameObject.AddComponent<MultipleTypeBuilding<Types>>();
//         ConnectingBuildingComponent = new ConnectingBuilding();
//         SetType(CurrentType);
//     }

//     protected void PerformExtraActionsOnPlacePreview(Vector3Int position) {
//         UpdateTexture(MultipleTypeBuildingComponent.Atlas.GetSprite($"{Type}{ConnectingBuildingComponent.GetConnectingFlags(position, OtherFloors)}"));
//     }

//     public void PerformExtraActionsOnPlace(Vector3Int position) {
//         OtherFloors.Add(position);
//         FloorWasPlaced?.Invoke(position);
//     }

//     public void PerformExtraActionsOnDelete() {
//         OtherFloors.Remove(BaseCoordinates[0]);
//         FloorWasPlaced -= AnotherFloorPlaced;
//         FloorWasPlaced?.Invoke(BaseCoordinates[0]);
//     }

//     public override List<MaterialInfo> GetMaterialsNeeded() {
//         return MultipleTypeBuildingComponent.Type switch {
//             Types.WOOD_FLOOR => new List<MaterialInfo>(){
//                 new(1, Materials.Wood)
//             },
//             Types.RUSTIC_PLANK_FLOOR => new List<MaterialInfo>(){
//                 new(1, Materials.Wood)
//             },
//             Types.STRAW_FLOOR => new List<MaterialInfo>(){
//                 new(1, Materials.Wood),
//                 new(1, Materials.Fiber)
//             },
//             Types.WEATHERED_FLOOR => new List<MaterialInfo>(){
//                 new(1, Materials.Wood)
//             },
//             Types.CRYSTAL_FLOOR => new List<MaterialInfo>(){
//                 new(1, Materials.RefinedQuartz)
//             },
//             Types.STONE_FLOOR => new List<MaterialInfo>(){
//                 new(1, Materials.Stone)
//             },
//             Types.STONE_WALKWAY_FLOOR => new List<MaterialInfo>(){
//                 new(1, Materials.Stone)
//             },
//             Types.BRICK_FLOOR => new List<MaterialInfo>(){
//                 new(2, Materials.Clay),
//                 new(5, Materials.Stone)
//             },
//             Types.WOOD_PATH => new List<MaterialInfo>(){
//                 new(1, Materials.Wood)
//             },
//             Types.GRAVEL_PATH => new List<MaterialInfo>(){
//                 new(1, Materials.Stone)
//             },
//             Types.COBBLESTONE_PATH => new List<MaterialInfo>(){
//                 new(1, Materials.Stone)
//             },
//             Types.STEPPING_STONE_PATH => new List<MaterialInfo>(){
//                 new(1, Materials.Stone)
//             },
//             Types.CRYSTAL_PATH => new List<MaterialInfo>(){
//                 new(1, Materials.RefinedQuartz)
//             },
//             _ => throw new Exception("Invalid Floor Type")
//         };
//     }

//     public string AddToBuildingData() {
//         return $"{MultipleTypeBuildingComponent.Type}";
//     }

//     public void LoadExtraBuildingData(string[] data) {
//         MultipleTypeBuildingComponent.SetType((Types)Enum.Parse(typeof(Types), data[0]));
//     }

//     private void AnotherFloorPlaced(Vector3Int position) {
//         if (BaseCoordinates?.Contains(position) ?? true) return;
//         UpdateTexture(MultipleTypeBuildingComponent.Atlas.GetSprite($"{Type}{ConnectingBuildingComponent.GetConnectingFlags(BaseCoordinates[0], OtherFloors)}"));
//     }
//     public void CycleType() {
//         int enumLength = Enum.GetValues(typeof(Types)).Length;
//         int intValue = (int)Type;
//         intValue = (intValue + 1) % enumLength;
//         SetType((Types)Enum.ToObject(typeof(Types), intValue));
//     }

//     public void SetType(Types type) {
//         CurrentType = type;
//         Type = type;
//         UpdateTexture(MultipleTypeBuildingComponent.Atlas.GetSprite($"{type}0"));
//     }

//     public GameObject[] CreateButtonsForAllTypes() {
//         List<GameObject> buttons = new();
//         foreach (Types type in Enum.GetValues(typeof(Types))) {
//             GameObject button = GameObject.Instantiate(Resources.Load<GameObject>("UI/BuildingButton"));
//             button.name = $"{type}";
//             button.GetComponent<Image>().sprite = MultipleTypeBuildingComponent.Atlas.GetSprite($"{type}0");

//             Type buildingType = GetType();
//             button.GetComponent<Button>().onClick.AddListener(() => {
//                 BuildingController.SetCurrentBuildingToMultipleTypeBuilding(buildingType, type);
//                 BuildingController.SetCurrentAction(Actions.PLACE);
//             });
//             buttons.Add(button);
//         }
//         return buttons.ToArray();
//     }

//     public int GetConnectingFlags(Vector3Int position, List<Vector3Int> otherBuildings) => ConnectingBuildingComponent.GetConnectingFlags(position, otherBuildings);
// }
