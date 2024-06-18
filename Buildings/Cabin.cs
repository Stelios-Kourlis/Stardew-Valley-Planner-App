// using System;
// using System.Collections;
// using System.Collections.Generic;
// using System.Linq;
// using UnityEngine;
// using UnityEngine.Tilemaps;
// using UnityEngine.U2D;
// using UnityEngine.UI;
// using static Utility.ClassManager;

// public class Cabin : Building, ITieredBuilding, IMultipleTypeBuilding<Cabin.Types>, IInteractableBuilding, IExtraActionBuilding {

//     public enum Types {
//         Wood,
//         Plank,
//         Stone,
//         Beach,
//         Neighbor,
//         Rustic,
//         Trailer
//     }
//     private static readonly bool[] cabinTypeIsPlaced = new bool[Enum.GetValues(typeof(Types)).Length];
//     private MultipleTypeBuilding<Types> multipleTypeBuildingComponent;
//     private TieredBuilding tieredBuildingComponent;
//     private InteractableBuildingComponent interactableBuildingComponent;

//     public int Tier => tieredBuildingComponent.Tier;

//     // private SpriteAtlas Atlas => multipleTypeBuildingComponent != null ? multipleTypeBuildingComponent.Atlas : tieredBuildingComponent.Atlas;

//     public Types Type => multipleTypeBuildingComponent.Type;

//     public static Types CurrentType { get; private set; }

//     public ButtonTypes[] BuildingInteractions => interactableBuildingComponent.BuildingInteractions;

//     public GameObject ButtonParentGameObject => interactableBuildingComponent.ButtonParentGameObject;

//     public int MaxTier => tieredBuildingComponent.MaxTier;

//     public override void OnAwake() {
//         BaseHeight = 3;
//         BuildingName = "Cabin";
//         for (int i = 0; i < cabinTypeIsPlaced.Length; i++) cabinTypeIsPlaced[i] = false;
//         // tieredBuildingComponent = new TieredBuilding(this, 4);
//         interactableBuildingComponent = new InteractableBuildingComponent(this);
//         multipleTypeBuildingComponent = new MultipleTypeBuilding<Types>(this);
//         multipleTypeBuildingComponent.DefaultSprite = multipleTypeBuildingComponent.Atlas.GetSprite($"{Types.Wood}1");

//         SetType(CurrentType);
//         SetTier(1);
//         base.OnAwake();
//     }

//     public void PerformExtraActionsOnPlace(Vector3Int position) {
//         if (cabinTypeIsPlaced[(int)Type]) {//this check is not enforced
//             GetNotificationManager().SendNotification("You can only have one of each type of cabin", NotificationManager.Icons.ErrorIcon);
//             return;
//         }
//         cabinTypeIsPlaced[(int)Type] = true;
//     }

//     public void PerformExtraActionsOnDelete() {
//         cabinTypeIsPlaced[(int)Type] = false;
//     }

//     public override List<MaterialInfo> GetMaterialsNeeded() {
//         List<MaterialInfo> level1 = new() { new(100, Materials.Coins) };
//         return tieredBuildingComponent.Tier switch {
//             2 => new List<MaterialInfo>{
//                 new(10_000, Materials.Coins),
//                 new(450, Materials.Wood),
//             }.Union(level1).ToList(),
//             3 => new List<MaterialInfo>{
//                 new(60_000, Materials.Coins),
//                 new(450, Materials.Wood),
//                 new(150, Materials.Hardwood),
//             }.Union(level1).ToList(),
//             4 => new List<MaterialInfo>{
//                 new(160_000, Materials.Coins),
//                 new(450, Materials.Wood),
//                 new(150, Materials.Hardwood),
//             }.Union(level1).ToList(),
//             _ => throw new System.Exception("Invalid Cabin Tier")
//         };
//     }

//     public string AddToBuildingData() {
//         return $"{tieredBuildingComponent.Tier}|{multipleTypeBuildingComponent.Type}";
//     }

//     public void LoadExtraBuildingData(string[] data) {
//         tieredBuildingComponent.SetTier(int.Parse(data[0]));
//         multipleTypeBuildingComponent.SetType((Types)Enum.Parse(typeof(Types), data[1]));
//     }

//     public void SetTier(int tier) {
//         tieredBuildingComponent.SetTier(tier);
//         Sprite sprite;
//         Types type = multipleTypeBuildingComponent?.Type ?? Types.Wood;
//         // sprite = Atlas.GetSprite($"{type}{tier}");
//         // UpdateTexture(sprite);
//     }

//     public void CycleType() => multipleTypeBuildingComponent.CycleType();

//     public void SetType(Types type) {
//         multipleTypeBuildingComponent.SetType(type);
//         Sprite sprite;
//         // sprite = Atlas.GetSprite($"{type}{tieredBuildingComponent?.Tier ?? 1}");
//         // UpdateTexture(sprite);
//     }

//     public GameObject[] CreateButtonsForAllTypes() {
//         List<GameObject> buttons = new();
//         foreach (Types type in Enum.GetValues(typeof(Types))) {
//             GameObject button = Instantiate(Resources.Load<GameObject>("UI/BuildingButton"));
//             button.name = $"{type}Button";
//             button.GetComponent<Image>().sprite = multipleTypeBuildingComponent.Atlas.GetSprite($"{type}1");
//             Type buildingType = GetType();
//             button.GetComponent<Button>().onClick.AddListener(() => {
//                 BuildingController.SetCurrentBuildingToMultipleTypeBuilding(buildingType, type);
//                 BuildingController.SetCurrentAction(Actions.PLACE);
//             });
//             buttons.Add(button);
//         }
//         return buttons.ToArray();
//     }

// }
