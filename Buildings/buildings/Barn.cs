// using System;
// using System.Collections;
// using System.Collections.Generic;
// using System.Linq;
// using System.Runtime.Remoting.Messaging;
// using UnityEngine;
// using UnityEngine.Tilemaps;
// using UnityEngine.U2D;
// using UnityEngine.UI;
// using static Utility.BuildingManager;
// using static Utility.ClassManager;
// using static Utility.TilemapManager;

// public class Barn : Building, IExtraActionBuilding {
//     public AnimalHouseComponent AnimalHouseComponent { get; private set; }
//     public TieredBuildingComponent TieredBuildingComponent { get; private set; }
//     public InteractableBuildingComponent InteractableBuildingComponent { get; private set; }
//     public EnterableBuildingComponent EnterableBuildingComponent { get; private set; }


//     public HashSet<ButtonTypes> BuildingInteractions => gameObject.GetComponent<InteractableBuildingComponent>().BuildingInteractions;

//     public GameObject ButtonParentGameObject => gameObject.GetComponent<InteractableBuildingComponent>().ButtonParentGameObject;


//     public override void OnAwake() {
//         // Debug.Log("Barn OnAwake");
//         BaseHeight = 4;
//         BuildingName = "Barn";
//         base.OnAwake();
//         // TieredBuildingComponent = gameObject.AddComponent<TieredBuildingComponent>().SetMaxTier(3);
//         // AnimalHouseComponent = gameObject.AddComponent<AnimalHouseComponent>().SetAllowedAnimalsPerTier(
//         //     new Dictionary<int, HashSet<Animals>> {
//         //         { 1, new HashSet<Animals>{ Animals.Cow, Animals.Ostrich } },
//         //         { 2, new HashSet<Animals>{ Animals.Cow, Animals.Ostrich, Animals.Goat } },
//         //         { 3, new HashSet<Animals>{ Animals.Cow, Animals.Ostrich, Animals.Goat, Animals.Sheep, Animals.Pig } }
//         //     }
//         // );
//         // EnterableBuildingComponent = gameObject.AddComponent<EnterableBuildingComponent>().AddInteriorInteractions(
//         //     new HashSet<ButtonTypes> {
//         //         ButtonTypes.TIER_ONE,
//         //         ButtonTypes.TIER_TWO,
//         //         ButtonTypes.TIER_THREE,
//         //     }
//         // );
//     }

//     // public override List<MaterialCostEntry> GetMaterialsNeeded() {
//     //     List<MaterialCostEntry> animalCost = new();
//     //     foreach (var animal in AnimalHouseComponent.AnimalsInBuilding) {
//     //         MaterialCostEntry cost = animal switch {
//     //             Animals.Cow => new(1_500, Materials.Coins),
//     //             Animals.Ostrich => new("Ostrich Egg"),
//     //             Animals.Goat => new(4_000, Materials.Coins),
//     //             Animals.Sheep => new(8_000, Materials.Coins),
//     //             Animals.Pig => new(16_000, Materials.Coins),
//     //             _ => throw new System.ArgumentException($"Invalid animal {animal}")
//     //         };
//     //         animalCost.Add(cost);
//     //     }
//     //     return TieredBuildingComponent.Tier switch {
//     //         1 => new List<MaterialCostEntry>{
//     //             new(6_000, Materials.Coins),
//     //             new(350, Materials.Wood),
//     //             new(150, Materials.Stone)
//     //         }.Union(animalCost).ToList(),
//     //         2 => new List<MaterialCostEntry>{
//     //             new(6_000 + 12_000, Materials.Coins),
//     //             new(350 + 450, Materials.Wood),
//     //             new(150 + 200, Materials.Stone)
//     //         }.Union(animalCost).ToList(),
//     //         3 => new List<MaterialCostEntry>{
//     //             new(6_000 + 12_000 + 25_000, Materials.Coins),
//     //             new(350 + 450 + 550, Materials.Wood),
//     //             new(150 + 200 + 300, Materials.Stone)
//     //         }.Union(animalCost).ToList(),
//     //         _ => throw new System.ArgumentException($"Invalid tier {TieredBuildingComponent.Tier}")
//     //     };
//     // }
// }
