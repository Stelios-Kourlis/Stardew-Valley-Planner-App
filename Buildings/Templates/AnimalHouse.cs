using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.U2D;

public class AnimalHouse{//todo this not work work fix fix
    private readonly TieredBuilding tieredBuildingComponent;
    private readonly Animals[][] animalsPerTier;
    private readonly SpriteAtlas animalAtlas;
    private readonly SpriteAtlas animalsInBuildingPanelBackgroundAtlas;
    public List<Animals> AnimalsInBuilding {get; private set;}
    public int MaxAnimalCapacity {get; private set;}
    private Building Building {get; set;}

    /// <summary>
    /// Constructor for AnimalHouse, when adding tier animals ONLY add those that weren't in the previous tier
    /// </summary>
    public AnimalHouse(Building building, Animals[] tierOneAnimals, Animals[] tierTwoAnimals, Animals[] tierThreeAnimals, TieredBuilding tieredBuildingComponent){
        Building = building;
        animalsPerTier = new Animals[][]{
            tierOneAnimals,
            tierOneAnimals.Concat(tierTwoAnimals).ToArray(),
            tierTwoAnimals.Concat(tierThreeAnimals).ToArray()
        };
        this.tieredBuildingComponent = tieredBuildingComponent;
        animalAtlas = Resources.Load("BarnAnimalsAtlas") as SpriteAtlas;
        animalsInBuildingPanelBackgroundAtlas = Resources.Load("UI/AnimalsInBuildingAtlas") as SpriteAtlas;
        AnimalsInBuilding = new List<Animals>();
    }

    public void AddAnimal(Animals animal){
        if (AnimalsInBuilding.Count >= MaxAnimalCapacity) return;
        if (!animalsPerTier[tieredBuildingComponent.Tier - 1].Contains(animal)) return;
        AnimalsInBuilding.Add(animal);
    }

    public void RemoveAnimal(Animals animal){
        AnimalsInBuilding.Remove(animal);
    }
}
