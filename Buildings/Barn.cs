using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.U2D;
using UnityEngine.UI;
using static Utility.BuildingManager;
using static Utility.ClassManager;
using static Utility.TilemapManager;

public class Barn : Building, IExtraActionBuilding {
    public AnimalHouseComponent AnimalHouseComponent { get; private set; }
    public TieredBuildingComponent TieredBuildingComponent { get; private set; }
    public InteractableBuildingComponent InteractableBuildingComponent { get; private set; }
    public EnterableBuildingComponent EnterableBuildingComponent { get; private set; }


    public HashSet<ButtonTypes> BuildingInteractions => gameObject.GetComponent<InteractableBuildingComponent>().BuildingInteractions;

    public GameObject ButtonParentGameObject => gameObject.GetComponent<InteractableBuildingComponent>().ButtonParentGameObject;


    public override void OnAwake() {
        BaseHeight = 4;
        BuildingName = "Barn";
        base.OnAwake();
        TieredBuildingComponent = gameObject.AddComponent<TieredBuildingComponent>().SetMaxTier(3);
        AnimalHouseComponent = gameObject.AddComponent<AnimalHouseComponent>().SetAllowedAnimalsPerTier(
            new Dictionary<int, HashSet<Animals>> {
                { 1, new HashSet<Animals>{ Animals.Cow, Animals.Ostrich } },
                { 2, new HashSet<Animals>{ Animals.Cow, Animals.Ostrich, Animals.Goat } },
                { 3, new HashSet<Animals>{ Animals.Cow, Animals.Ostrich, Animals.Goat, Animals.Sheep, Animals.Pig } }
            }
        );
        EnterableBuildingComponent = gameObject.AddComponent<EnterableBuildingComponent>();
    }

    // public void SetTier(int tier) {
    //     TieredBuildingComponent.SetTier(tier);
    //     AnimalHouseComponent.UpdateMaxAnimalCapacity(tier);

    //     //Update Animals
    //     List<KeyValuePair<Animals, GameObject>> animalsToRemove = new();
    //     string animalsRemoved = GetRemovedAnimals();
    //     if (tier < 2) animalsToRemove.AddRange(AnimalHouseComponent.AnimalsInBuilding.Where(animal => animal.Key == Animals.Goat));
    //     if (tier < 3) animalsToRemove.AddRange(AnimalHouseComponent.AnimalsInBuilding.Where(animal => animal.Key == Animals.Sheep || animal.Key == Animals.Pig));
    //     if (animalsToRemove.Count != 0) GetNotificationManager().SendNotification($"Removed {animalsRemoved} because they aren't allowed in tier {tier} {GetType()}", NotificationManager.Icons.InfoIcon);

    //     foreach (var pair in animalsToRemove) {
    //         Destroy(pair.Value);
    //         AnimalHouseComponent.AnimalsInBuilding.Remove(pair);
    //     }

    //     if (AnimalHouseComponent.AnimalsInBuilding.Count > AnimalHouseComponent.MaxAnimalCapacity) GetNotificationManager().SendNotification($"Removed {AnimalHouseComponent.AnimalsInBuilding.Count - AnimalHouseComponent.MaxAnimalCapacity} animals that exceed the new capacity of {GetType()}", NotificationManager.Icons.InfoIcon);
    //     while (AnimalHouseComponent.AnimalsInBuilding.Count > AnimalHouseComponent.MaxAnimalCapacity) {
    //         Destroy(AnimalHouseComponent.AnimalsInBuilding.Last().Value);
    //         AnimalHouseComponent.AnimalsInBuilding.Remove(AnimalHouseComponent.AnimalsInBuilding.Last());
    //     }
    // }

    // private string GetRemovedAnimals() {
    //     int goatCount = AnimalHouseComponent.AnimalsInBuilding.Count(animal => animal.Key == Animals.Goat);
    //     string goatsRemoved = goatCount > 0 ? $"{goatCount} Goat" : "";
    //     if (goatCount > 1) goatsRemoved += "s";
    //     goatsRemoved += ",";

    //     int sheepCount = AnimalHouseComponent.AnimalsInBuilding.Count(animal => animal.Key == Animals.Sheep);
    //     string sheepRemoved = sheepCount > 0 ? $"{sheepCount} Sheep," : "";

    //     int pigCount = AnimalHouseComponent.AnimalsInBuilding.Count(animal => animal.Key == Animals.Pig);
    //     string pigsRemoved = pigCount > 0 ? $"{pigCount} Pig" : "";
    //     return $"{goatsRemoved} {sheepRemoved} {pigsRemoved}";
    // }

    public override List<MaterialCostEntry> GetMaterialsNeeded() {
        List<MaterialCostEntry> animalCost = new();
        foreach (var animal in AnimalHouseComponent.AnimalsInBuilding) {
            MaterialCostEntry cost = animal switch {
                Animals.Cow => new(1_500, Materials.Coins),
                Animals.Ostrich => new("Ostrich Egg"),
                Animals.Goat => new(4_000, Materials.Coins),
                Animals.Sheep => new(8_000, Materials.Coins),
                Animals.Pig => new(16_000, Materials.Coins),
                _ => throw new System.ArgumentException($"Invalid animal {animal}")
            };
            animalCost.Add(cost);
        }
        return TieredBuildingComponent.Tier switch {
            1 => new List<MaterialCostEntry>{
                new(6_000, Materials.Coins),
                new(350, Materials.Wood),
                new(150, Materials.Stone)
            }.Union(animalCost).ToList(),
            2 => new List<MaterialCostEntry>{
                new(6_000 + 12_000, Materials.Coins),
                new(350 + 450, Materials.Wood),
                new(150 + 200, Materials.Stone)
            }.Union(animalCost).ToList(),
            3 => new List<MaterialCostEntry>{
                new(6_000 + 12_000 + 25_000, Materials.Coins),
                new(350 + 450 + 550, Materials.Wood),
                new(150 + 200 + 300, Materials.Stone)
            }.Union(animalCost).ToList(),
            _ => throw new System.ArgumentException($"Invalid tier {TieredBuildingComponent.Tier}")
        };
    }

    public string GetExtraData() {
        string animals = "";
        foreach (Animals animal in AnimalHouseComponent.AnimalsInBuilding) animals += $"|{(int)animal}";
        return $"{TieredBuildingComponent.Tier}|{AnimalHouseComponent.AnimalsInBuilding.Count}{animals}";
    }

    public void LoadExtraBuildingData(int x, int y, params string[] data) {
        TieredBuildingComponent.SetTier(int.Parse(data[0]));
        int animalCount = int.Parse(data[1]);
        for (int i = 0; i < animalCount; i++) AnimalHouseComponent.AddAnimal((Animals)Enum.Parse(typeof(Animals), data[i + 2]));
    }

}
