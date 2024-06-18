// using System;
// using System.Collections;
// using System.Collections.Generic;
// using System.Linq;
// using UnityEngine;
// using UnityEngine.Tilemaps;
// using UnityEngine.U2D;
// using UnityEngine.UI;
// using static Utility.BuildingManager;
// using static Utility.ClassManager;

// public class Coop : Building, ITieredBuilding, IAnimalHouse {
//     public int Tier => TieredBuildingComponent.Tier;
//     public AnimalHouse AnimalHouseComponent { get; private set; }
//     public TieredBuilding TieredBuildingComponent { get; private set; }
//     public InteractableBuildingComponent InteractableBuildingComponent { get; private set; }

//     public List<KeyValuePair<Animals, GameObject>> AnimalsInBuilding => AnimalHouseComponent.AnimalsInBuilding;

//     public ButtonTypes[] BuildingInteractions => InteractableBuildingComponent.BuildingInteractions;

//     public GameObject ButtonParentGameObject => InteractableBuildingComponent.ButtonParentGameObject;

//     public int MaxTier => TieredBuildingComponent.MaxTier;

//     public override void OnAwake() {
//         BaseHeight = 3;
//         BuildingName = "Coop";
//         // TieredBuildingComponent = new TieredBuilding(this, 3);
//         InteractableBuildingComponent = new InteractableBuildingComponent(this);

//         AnimalHouseComponent = new AnimalHouse(this);
//         SetTier(1);
//         base.OnAwake();
//     }

//     public void PerformExtraActionsOnPlace(Vector3Int position) {
//         AnimalHouseComponent.AddAnimalMenuObject();
//     }

//     public void SetTier(int tier) {
//         TieredBuildingComponent.SetTier(tier);
//         AnimalHouseComponent.UpdateMaxAnimalCapacity(tier);

//         //Update Animals
//         List<KeyValuePair<Animals, GameObject>> animalsToRemove = new();

//         string animalsRemoved = GetRemovedAnimals();
//         if (tier < 2) animalsToRemove.AddRange(AnimalsInBuilding.Where(animal => animal.Key == Animals.Duck || animal.Key == Animals.VoidChicken || animal.Key == Animals.Dinosaur || animal.Key == Animals.GoldenChicken));
//         if (tier < 3) animalsToRemove.AddRange(AnimalsInBuilding.Where(animal => animal.Key == Animals.Rabbit));
//         if (animalsToRemove.Count != 0) GetNotificationManager().SendNotification($"Removed {animalsRemoved} because they aren't allowed in tier {tier} {GetType()}", NotificationManager.Icons.InfoIcon);

//         foreach (var pair in animalsToRemove) {
//             Destroy(pair.Value);
//             AnimalsInBuilding.Remove(pair);
//         }

//         if (AnimalsInBuilding.Count > AnimalHouseComponent.MaxAnimalCapacity) GetNotificationManager().SendNotification($"Removed {AnimalsInBuilding.Count - AnimalHouseComponent.MaxAnimalCapacity} animals that exceed the new capacity of {GetType()}", NotificationManager.Icons.InfoIcon);
//         while (AnimalsInBuilding.Count > AnimalHouseComponent.MaxAnimalCapacity) {
//             Destroy(AnimalsInBuilding.Last().Value);
//             AnimalsInBuilding.Remove(AnimalsInBuilding.Last());
//         }
//     }

//     public void ToggleAnimalMenu() => AnimalHouseComponent.ToggleAnimalMenu();

//     private string GetRemovedAnimals() {
//         int rabbitCount = AnimalsInBuilding.Count(animal => animal.Key == Animals.Rabbit);
//         string rabbitsRemoved = rabbitCount > 0 ? $"{rabbitCount} Rabbit" : "";
//         if (rabbitCount > 1) rabbitsRemoved += "s";
//         rabbitsRemoved += ",";

//         int duckCount = AnimalsInBuilding.Count(animal => animal.Key == Animals.Duck);
//         string ducksRemoved = duckCount > 0 ? $"{duckCount} Duck" : "";
//         if (duckCount > 1) ducksRemoved += "s";
//         ducksRemoved += ",";

//         int voidChickenCount = AnimalsInBuilding.Count(animal => animal.Key == Animals.VoidChicken);
//         string voidChickensRemoved = voidChickenCount > 0 ? $"{voidChickenCount} Void Chicken" : "";
//         if (voidChickenCount > 1) voidChickensRemoved += "s";
//         voidChickensRemoved += ",";

//         int dinosaurCount = AnimalsInBuilding.Count(animal => animal.Key == Animals.Dinosaur);
//         string dinosaursRemoved = dinosaurCount > 0 ? $"{dinosaurCount} Dinosaur" : "";
//         if (dinosaurCount > 1) dinosaursRemoved += "s";
//         dinosaursRemoved += ",";

//         int goldenChickenCount = AnimalsInBuilding.Count(animal => animal.Key == Animals.GoldenChicken);
//         string goldenChickensRemoved = goldenChickenCount > 0 ? $"{goldenChickenCount} Golden Chicken" : "";
//         if (goldenChickenCount > 1) goldenChickensRemoved += "s";

//         return $"{rabbitsRemoved} {ducksRemoved} {voidChickensRemoved} {dinosaursRemoved} {goldenChickensRemoved}";
//     }

//     public override List<MaterialInfo> GetMaterialsNeeded() {
//         return Tier switch {
//             1 => new List<MaterialInfo>{
//                 new(4_000, Materials.Coins),
//                 new(300, Materials.Wood),
//                 new(100, Materials.Stone)
//             },
//             2 => new List<MaterialInfo>{
//                 new(4_000 + 10_000, Materials.Coins),
//                 new(300 + 400, Materials.Wood),
//                 new(100 + 150, Materials.Stone)
//             },
//             3 => new List<MaterialInfo>{
//                 new(4_000 + 10_000 + 20_000, Materials.Coins),
//                 new(300 + 400 + 500, Materials.Wood),
//                 new(100 + 150 + 200, Materials.Stone)
//             },
//             _ => throw new System.ArgumentException($"Invalid tier {Tier}")
//         };
//     }

//     public string AddToBuildingData() {
//         string animals = "";
//         foreach (Animals animal in AnimalsInBuilding.Select(pair => pair.Key)) animals += $"|{(int)animal}";
//         return $"{Tier}|{AnimalsInBuilding.Count}{animals}";
//     }

//     public void LoadExtraBuildingData(string[] data) {
//         SetTier(int.Parse(data[0]));
//         int animalCount = int.Parse(data[1]);
//         for (int i = 0; i < animalCount; i++) AddAnimal((Animals)Enum.Parse(typeof(Animals), data[i + 2]));
//     }

//     public bool AddAnimal(Animals animal) {
//         if (!AnimalHouseComponent.AddAnimal(animal)) return false;
//         List<Animals> allowedAnimals = new() { Animals.Chicken };
//         if (Tier >= 2) allowedAnimals.AddRange(new List<Animals> { Animals.Duck, Animals.VoidChicken, Animals.Dinosaur, Animals.GoldenChicken });
//         if (Tier == 3) allowedAnimals.Add(Animals.Rabbit);
//         if (!allowedAnimals.Contains(animal)) { GetNotificationManager().SendNotification($"Animal {animal} is not allowed in a level {Tier} coop", NotificationManager.Icons.ErrorIcon); return false; }
//         AddAnimalButton(animal);
//         return true;
//     }

//     private void AddAnimalButton(Animals animal) {
//         ButtonParentGameObject.transform.GetChild(5);
//         GameObject button = new(animal.ToString());
//         AnimalsInBuilding.Add(new KeyValuePair<Animals, GameObject>(animal, button));
//         button.transform.SetParent(ButtonParentGameObject.transform.GetChild(5).GetChild(1).GetChild(0));
//         button.AddComponent<Image>().sprite = AnimalHouseComponent.AnimalAtlas.GetSprite(animal.ToString());
//         button.AddComponent<Button>().onClick.AddListener(() => {
//             AnimalsInBuilding.Remove(new KeyValuePair<Animals, GameObject>(animal, button));
//             Destroy(button);
//         });
//         AddHoverEffect(button.GetComponent<Button>());
//         button.transform.localScale = new Vector3(1, 1);
//     }
// }
