using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.U2D;
using UnityEngine.UI;
using static Utility.BuildingManager;
using static Utility.ClassManager;

public class Coop : Building, IAnimalHouse {
    public AnimalHouseComponent AnimalHouseComponent { get; private set; }
    public TieredBuildingComponent TieredBuildingComponent { get; private set; }
    public InteractableBuildingComponent InteractableBuildingComponent { get; private set; }

    public HashSet<ButtonTypes> BuildingInteractions => gameObject.GetComponent<InteractableBuildingComponent>().BuildingInteractions;

    public GameObject ButtonParentGameObject => gameObject.GetComponent<InteractableBuildingComponent>().ButtonParentGameObject;

    public override void OnAwake() {
        BaseHeight = 3;
        BuildingName = "Coop";
        base.OnAwake();
        TieredBuildingComponent = gameObject.AddComponent<TieredBuildingComponent>().SetMaxTier(3);
        AnimalHouseComponent = gameObject.AddComponent<AnimalHouseComponent>();
    }

    // public void PerformExtraActionsOnPlace(Vector3Int position) {
    //     AnimalHouseComponent.AddAnimalMenuObject();
    // }

    public void SetTier(int tier) {
        TieredBuildingComponent.SetTier(tier);
        AnimalHouseComponent.UpdateMaxAnimalCapacity(tier);

        //Update Animals
        List<KeyValuePair<Animals, GameObject>> animalsToRemove = new();

        string animalsRemoved = GetRemovedAnimals();
        if (tier < 2) animalsToRemove.AddRange(AnimalHouseComponent.AnimalsInBuilding.Where(animal => animal.Key == Animals.Duck || animal.Key == Animals.VoidChicken || animal.Key == Animals.Dinosaur || animal.Key == Animals.GoldenChicken));
        if (tier < 3) animalsToRemove.AddRange(AnimalHouseComponent.AnimalsInBuilding.Where(animal => animal.Key == Animals.Rabbit));
        if (animalsToRemove.Count != 0) GetNotificationManager().SendNotification($"Removed {animalsRemoved} because they aren't allowed in tier {tier} {GetType()}", NotificationManager.Icons.InfoIcon);

        foreach (var pair in animalsToRemove) {
            Destroy(pair.Value);
            AnimalHouseComponent.AnimalsInBuilding.Remove(pair);
        }

        if (AnimalHouseComponent.AnimalsInBuilding.Count > AnimalHouseComponent.MaxAnimalCapacity) GetNotificationManager().SendNotification($"Removed {AnimalHouseComponent.AnimalsInBuilding.Count - AnimalHouseComponent.MaxAnimalCapacity} animals that exceed the new capacity of {GetType()}", NotificationManager.Icons.InfoIcon);
        while (AnimalHouseComponent.AnimalsInBuilding.Count > AnimalHouseComponent.MaxAnimalCapacity) {
            Destroy(AnimalHouseComponent.AnimalsInBuilding.Last().Value);
            AnimalHouseComponent.AnimalsInBuilding.Remove(AnimalHouseComponent.AnimalsInBuilding.Last());
        }
    }

    public void ToggleAnimalMenu() => AnimalHouseComponent.ToggleAnimalMenu();

    private string GetRemovedAnimals() {
        int rabbitCount = AnimalHouseComponent.AnimalsInBuilding.Count(animal => animal.Key == Animals.Rabbit);
        string rabbitsRemoved = rabbitCount > 0 ? $"{rabbitCount} Rabbit" : "";
        if (rabbitCount > 1) rabbitsRemoved += "s";
        rabbitsRemoved += ",";

        int duckCount = AnimalHouseComponent.AnimalsInBuilding.Count(animal => animal.Key == Animals.Duck);
        string ducksRemoved = duckCount > 0 ? $"{duckCount} Duck" : "";
        if (duckCount > 1) ducksRemoved += "s";
        ducksRemoved += ",";

        int voidChickenCount = AnimalHouseComponent.AnimalsInBuilding.Count(animal => animal.Key == Animals.VoidChicken);
        string voidChickensRemoved = voidChickenCount > 0 ? $"{voidChickenCount} Void Chicken" : "";
        if (voidChickenCount > 1) voidChickensRemoved += "s";
        voidChickensRemoved += ",";

        int dinosaurCount = AnimalHouseComponent.AnimalsInBuilding.Count(animal => animal.Key == Animals.Dinosaur);
        string dinosaursRemoved = dinosaurCount > 0 ? $"{dinosaurCount} Dinosaur" : "";
        if (dinosaurCount > 1) dinosaursRemoved += "s";
        dinosaursRemoved += ",";

        int goldenChickenCount = AnimalHouseComponent.AnimalsInBuilding.Count(animal => animal.Key == Animals.GoldenChicken);
        string goldenChickensRemoved = goldenChickenCount > 0 ? $"{goldenChickenCount} Golden Chicken" : "";
        if (goldenChickenCount > 1) goldenChickensRemoved += "s";

        return $"{rabbitsRemoved} {ducksRemoved} {voidChickensRemoved} {dinosaursRemoved} {goldenChickensRemoved}";
    }

    public override List<MaterialCostEntry> GetMaterialsNeeded() {
        return TieredBuildingComponent.Tier switch {
            1 => new List<MaterialCostEntry>{
                new(4_000, Materials.Coins),
                new(300, Materials.Wood),
                new(100, Materials.Stone)
            },
            2 => new List<MaterialCostEntry>{
                new(4_000 + 10_000, Materials.Coins),
                new(300 + 400, Materials.Wood),
                new(100 + 150, Materials.Stone)
            },
            3 => new List<MaterialCostEntry>{
                new(4_000 + 10_000 + 20_000, Materials.Coins),
                new(300 + 400 + 500, Materials.Wood),
                new(100 + 150 + 200, Materials.Stone)
            },
            _ => throw new System.ArgumentException($"Invalid tier {TieredBuildingComponent.Tier}")
        };
    }

    public string GetExtraData() {
        string animals = "";
        foreach (Animals animal in AnimalHouseComponent.AnimalsInBuilding.Select(pair => pair.Key)) animals += $"|{(int)animal}";
        return $"{TieredBuildingComponent.Tier}|{AnimalHouseComponent.AnimalsInBuilding.Count}{animals}";
    }

    public void LoadExtraBuildingData(string[] data) {
        SetTier(int.Parse(data[0]));
        int animalCount = int.Parse(data[1]);
        for (int i = 0; i < animalCount; i++) AddAnimal((Animals)Enum.Parse(typeof(Animals), data[i + 2]));
    }

    public bool AddAnimal(Animals animal) {
        if (!AnimalHouseComponent.AddAnimal(animal)) return false;
        List<Animals> allowedAnimals = new() { Animals.Chicken };
        if (TieredBuildingComponent.Tier >= 2) allowedAnimals.AddRange(new List<Animals> { Animals.Duck, Animals.VoidChicken, Animals.Dinosaur, Animals.GoldenChicken });
        if (TieredBuildingComponent.Tier == 3) allowedAnimals.Add(Animals.Rabbit);
        if (!allowedAnimals.Contains(animal)) { GetNotificationManager().SendNotification($"Animal {animal} is not allowed in a level {TieredBuildingComponent.Tier} coop", NotificationManager.Icons.ErrorIcon); return false; }
        AddAnimalButton(animal);
        return true;
    }

    private void AddAnimalButton(Animals animal) { //todo this is the same as barn, extract?
        // ButtonParentGameObject.transform.GetChild(5);
        GameObject button = new(animal.ToString());
        AnimalHouseComponent.AnimalsInBuilding.Add(new KeyValuePair<Animals, GameObject>(animal, button));
        button.transform.SetParent(ButtonParentGameObject.transform.Find("ADD_ANIMAL").GetChild(1).GetChild(0));
        button.AddComponent<Image>().sprite = AnimalHouseComponent.AnimalAtlas.GetSprite(animal.ToString());
        button.AddComponent<Button>().onClick.AddListener(() => {
            AnimalHouseComponent.AnimalsInBuilding.Remove(new KeyValuePair<Animals, GameObject>(animal, button));
            Destroy(button);
        });
        AddHoverEffect(button.GetComponent<Button>());
        button.transform.localScale = new Vector3(1, 1);
    }

    public void OnMouseRightClick() {
        if (!BuildingController.isInsideBuilding.Key) ButtonParentGameObject.SetActive(!ButtonParentGameObject.activeSelf);
    }
}
