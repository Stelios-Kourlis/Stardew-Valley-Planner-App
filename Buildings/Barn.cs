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

public class Barn : Building, ITieredBuilding, IAnimalHouse, IExtraActionBuilding, IInteractableBuilding {
    public AnimalHouseComponent AnimalHouseComponent { get; private set; }
    public TieredBuildingComponent TieredBuildingComponent { get; private set; }
    public InteractableBuildingComponent InteractableBuildingComponent { get; private set; }
    public int Tier => gameObject.GetComponent<TieredBuildingComponent>().MaxTier;

    public List<KeyValuePair<Animals, GameObject>> AnimalsInBuilding => gameObject.GetComponent<AnimalHouseComponent>().AnimalsInBuilding;

    public ButtonTypes[] BuildingInteractions => gameObject.GetComponent<InteractableBuildingComponent>().BuildingInteractions;

    public GameObject ButtonParentGameObject => gameObject.GetComponent<InteractableBuildingComponent>().ButtonParentGameObject;

    public int MaxTier => gameObject.GetComponent<TieredBuildingComponent>().MaxTier;

    public override void OnAwake() {
        BaseHeight = 4;
        BuildingName = "Barn";
        base.OnAwake();
        TieredBuildingComponent = gameObject.AddComponent<TieredBuildingComponent>().SetMaxTier(3);
        AnimalHouseComponent = gameObject.AddComponent<AnimalHouseComponent>();
        SetTier(1);
    }

    public void PerformExtraActionsOnPlace(Vector3Int position) {
        AnimalHouseComponent.AddAnimalMenuObject();
    }

    public void SetTier(int tier) {
        TieredBuildingComponent.SetTier(tier);
        AnimalHouseComponent.UpdateMaxAnimalCapacity(tier);

        //Update Animals
        List<KeyValuePair<Animals, GameObject>> animalsToRemove = new();
        string animalsRemoved = GetRemovedAnimals();
        if (tier < 2) animalsToRemove.AddRange(AnimalsInBuilding.Where(animal => animal.Key == Animals.Goat));
        if (tier < 3) animalsToRemove.AddRange(AnimalsInBuilding.Where(animal => animal.Key == Animals.Sheep || animal.Key == Animals.Pig));
        if (animalsToRemove.Count != 0) GetNotificationManager().SendNotification($"Removed {animalsRemoved} because they aren't allowed in tier {tier} {GetType()}", NotificationManager.Icons.InfoIcon);

        foreach (var pair in animalsToRemove) {
            Destroy(pair.Value);
            AnimalsInBuilding.Remove(pair);
        }

        if (AnimalsInBuilding.Count > AnimalHouseComponent.MaxAnimalCapacity) GetNotificationManager().SendNotification($"Removed {AnimalsInBuilding.Count - AnimalHouseComponent.MaxAnimalCapacity} animals that exceed the new capacity of {GetType()}", NotificationManager.Icons.InfoIcon);
        while (AnimalsInBuilding.Count > AnimalHouseComponent.MaxAnimalCapacity) {
            Destroy(AnimalsInBuilding.Last().Value);
            AnimalsInBuilding.Remove(AnimalsInBuilding.Last());
        }
    }

    private string GetRemovedAnimals() {
        int goatCount = AnimalsInBuilding.Count(animal => animal.Key == Animals.Goat);
        string goatsRemoved = goatCount > 0 ? $"{goatCount} Goat" : "";
        if (goatCount > 1) goatsRemoved += "s";
        goatsRemoved += ",";

        int sheepCount = AnimalsInBuilding.Count(animal => animal.Key == Animals.Sheep);
        string sheepRemoved = sheepCount > 0 ? $"{sheepCount} Sheep," : "";

        int pigCount = AnimalsInBuilding.Count(animal => animal.Key == Animals.Pig);
        string pigsRemoved = pigCount > 0 ? $"{pigCount} Pig" : "";
        return $"{goatsRemoved} {sheepRemoved} {pigsRemoved}";
    }

    public override List<MaterialInfo> GetMaterialsNeeded() {
        List<MaterialInfo> animalCost = new();
        foreach (var animal in AnimalHouseComponent.AnimalsInBuilding.Select(pair => pair.Key)) {
            MaterialInfo cost = animal switch {
                Animals.Cow => new(1_500, Materials.Coins),
                Animals.Ostrich => new("Ostrich Egg"),
                Animals.Goat => new(4_000, Materials.Coins),
                Animals.Sheep => new(8_000, Materials.Coins),
                Animals.Pig => new(16_000, Materials.Coins),
                _ => throw new System.ArgumentException($"Invalid animal {animal}")
            };
            animalCost.Add(cost);
        }
        return Tier switch {
            1 => new List<MaterialInfo>{
                new(6_000, Materials.Coins),
                new(350, Materials.Wood),
                new(150, Materials.Stone)
            }.Union(animalCost).ToList(),
            2 => new List<MaterialInfo>{
                new(6_000 + 12_000, Materials.Coins),
                new(350 + 450, Materials.Wood),
                new(150 + 200, Materials.Stone)
            }.Union(animalCost).ToList(),
            3 => new List<MaterialInfo>{
                new(6_000 + 12_000 + 25_000, Materials.Coins),
                new(350 + 450 + 550, Materials.Wood),
                new(150 + 200 + 300, Materials.Stone)
            }.Union(animalCost).ToList(),
            _ => throw new System.ArgumentException($"Invalid tier {Tier}")
        };
    }

    public string AddToBuildingData() {
        string animals = "";
        foreach (Animals animal in AnimalsInBuilding.Select(pair => pair.Key)) animals += $"|{(int)animal}";
        return $"{Tier}|{AnimalsInBuilding.Count}{animals}";
    }

    public void LoadExtraBuildingData(int x, int y, params string[] data) {
        TieredBuildingComponent.SetTier(int.Parse(data[0]));
        int animalCount = int.Parse(data[1]);
        for (int i = 0; i < animalCount; i++) AddAnimal((Animals)Enum.Parse(typeof(Animals), data[i + 2]));
    }

    public bool AddAnimal(Animals animal) {
        if (!AnimalHouseComponent.AddAnimal(animal)) return false;
        List<Animals> allowedAnimals = new() { Animals.Cow, Animals.Ostrich };
        if (Tier >= 2) allowedAnimals.Add(Animals.Goat);
        if (Tier == 3) { allowedAnimals.Add(Animals.Sheep); allowedAnimals.Add(Animals.Pig); }
        if (!allowedAnimals.Contains(animal)) { GetNotificationManager().SendNotification($"Cannot add {animal} to Barn tier {Tier}", NotificationManager.Icons.ErrorIcon); return false; }
        AddAnimalButton(animal);
        return true;
    }

    public void ToggleAnimalMenu() => AnimalHouseComponent.ToggleAnimalMenu();

    private void AddAnimalButton(Animals animal) {
        GameObject button = new(animal.ToString());
        AnimalsInBuilding.Add(new KeyValuePair<Animals, GameObject>(animal, button));
        button.transform.SetParent(ButtonParentGameObject.transform.GetChild(5).GetChild(1).GetChild(0));
        button.AddComponent<Image>().sprite = AnimalHouseComponent.AnimalAtlas.GetSprite(animal.ToString());
        button.AddComponent<Button>().onClick.AddListener(() => {
            AnimalsInBuilding.Remove(new KeyValuePair<Animals, GameObject>(animal, button));
            Destroy(button);
        });
        AddHoverEffect(button.GetComponent<Button>());
        button.transform.localScale = new Vector3(1, 1);
    }

    public void OnMouseRightClick() {
        ButtonParentGameObject.SetActive(!ButtonParentGameObject.activeSelf);
    }
}
