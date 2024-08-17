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

public class Coop : Building {
    public AnimalHouseComponent AnimalHouseComponent { get; private set; }
    public TieredBuildingComponent TieredBuildingComponent { get; private set; }
    public EnterableBuildingComponent EnterableBuildingComponent { get; private set; }
    public InteractableBuildingComponent InteractableBuildingComponent { get; private set; }

    public HashSet<ButtonTypes> BuildingInteractions => gameObject.GetComponent<InteractableBuildingComponent>().BuildingInteractions;
    public GameObject ButtonParentGameObject => gameObject.GetComponent<InteractableBuildingComponent>().ButtonParentGameObject;

    public override void OnAwake() {
        BaseHeight = 3;
        BuildingName = "Coop";
        base.OnAwake();
        TieredBuildingComponent = gameObject.AddComponent<TieredBuildingComponent>().SetMaxTier(3);
        AnimalHouseComponent = gameObject.AddComponent<AnimalHouseComponent>().SetAllowedAnimalsPerTier(
            new Dictionary<int, HashSet<Animals>> {
                { 1, new HashSet<Animals>{ Animals.Chicken } },
                { 2, new HashSet<Animals>{ Animals.Chicken, Animals.Duck, Animals.VoidChicken, Animals.Dinosaur, Animals.GoldenChicken } },
                { 3, new HashSet<Animals>{ Animals.Chicken, Animals.Duck, Animals.VoidChicken, Animals.Dinosaur, Animals.GoldenChicken, Animals.Rabbit } }
            }
        );
        EnterableBuildingComponent = gameObject.AddComponent<EnterableBuildingComponent>().AddInteriorInteractions(
            new HashSet<ButtonTypes> {
                ButtonTypes.TIER_ONE,
                ButtonTypes.TIER_TWO,
                ButtonTypes.TIER_THREE,
            }
        );
    }

    public override List<MaterialCostEntry> GetMaterialsNeeded() {//todo add animal costs
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
        foreach (Animals animal in AnimalHouseComponent.AnimalsInBuilding) animals += $"|{(int)animal}";
        return $"{TieredBuildingComponent.Tier}|{AnimalHouseComponent.AnimalsInBuilding.Count}{animals}";
    }

    public void LoadExtraBuildingData(string[] data) {
        TieredBuildingComponent.SetTier(int.Parse(data[0]));
        int animalCount = int.Parse(data[1]);
        for (int i = 0; i < animalCount; i++) AnimalHouseComponent.AddAnimal((Animals)Enum.Parse(typeof(Animals), data[i + 2]));
    }



}
