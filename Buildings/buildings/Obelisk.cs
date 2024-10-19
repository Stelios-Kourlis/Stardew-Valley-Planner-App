// using System;
// using System.Collections;
// using System.Collections.Generic;
// using System.Linq;
// using UnityEngine;
// using UnityEngine.U2D;

// public class Obelisk : Building {

//     public MultipleTypeBuildingComponent MultipleTypeBuildingComponent { get; private set; }

//     public enum Types {
//         Water,
//         Desert,
//         Island,
//         Earth
//     }

//     public HashSet<ButtonTypes> BuildingInteractions => gameObject.GetComponent<InteractableBuildingComponent>().BuildingInteractions;

//     public GameObject ButtonParentGameObject => gameObject.GetComponent<InteractableBuildingComponent>().ButtonParentGameObject;

//     public override void OnAwake() {
//         BaseHeight = 3;
//         BuildingName = "Obelisk";
//         base.OnAwake();
//         // MultipleTypeBuildingComponent = gameObject.AddComponent<MultipleTypeBuildingComponent>().SetEnumType(typeof(Types));
//         // Debug.Log($"1: {sprite == null}");
//         // Debug.Log($"2: {sprite == null}");
//     }

//     // public override List<MaterialCostEntry> GetMaterialsNeeded() {
//     //     return MultipleTypeBuildingComponent.Type switch {
//     //         Types.Water => new System.Collections.Generic.List<MaterialCostEntry>{
//     //             new(500000, Materials.Coins),
//     //             new(5, Materials.IridiumBar),
//     //             new(10, Materials.Clam),
//     //             new(10, Materials.Coral)
//     //         },
//     //         Types.Desert => new List<MaterialCostEntry>{
//     //             new(1000000, Materials.Coins),
//     //             new(20, Materials.IridiumBar),
//     //             new(10, Materials.Coconut),
//     //             new(10, Materials.CactusFruit)
//     //         },
//     //         Types.Island => new List<MaterialCostEntry>{
//     //             new(1000000, Materials.Coins),
//     //             new(10, Materials.IridiumBar),
//     //             new(10, Materials.DragonTooth),
//     //             new(10, Materials.Banana)
//     //         },
//     //         Types.Earth => new List<MaterialCostEntry>{
//     //             new(500000, Materials.Coins),
//     //             new(10, Materials.IridiumBar),
//     //             new(10, Materials.EarthCrystal)
//     //         },
//     //         _ => new List<MaterialCostEntry>()
//     //     };
//     // }

//     // public string AddToBuildingData() {
//     //     return $"{(int)MultipleTypeBuildingComponent.Type}";
//     // }

//     // public void LoadExtraBuildingData(string[] data) {
//     //     MultipleTypeBuildingComponent.SetType((Types)int.Parse(data[0]));
//     // }


// }
