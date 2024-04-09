using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class AnimalHouse{
    private readonly Animals[] barnAnimals = new Animals[]{
        Animals.Cow,
        Animals.Goat,
        Animals.Sheep,
        Animals.Pig,
        Animals.Ostrich
    };
    private readonly Animals[] coopAnimals = new Animals[]{
        Animals.Chicken,
        Animals.Dinosaur,
        Animals.Duck,
        Animals.Rabbit,
        Animals.VoidChicken,
        Animals.GoldenChicken
    };
    private readonly SpriteAtlas animalAtlas;
    private readonly SpriteAtlas animalsInBuildingPanelBackgroundAtlas;
    public List<Animals> AnimalsInBuilding {get; private set;}
    public int MaxAnimalCapacity {get; private set;}
    public Building Building {get; private set;}

    public AnimalHouse(Building building){
        Building = building;
        animalAtlas = Resources.Load("BarnAnimalsAtlas") as SpriteAtlas;
        animalsInBuildingPanelBackgroundAtlas = Resources.Load("UI/AnimalsInBuildingAtlas") as SpriteAtlas;
        AnimalsInBuilding = new List<Animals>();
    }

    public void AddAnimal(Animals animal){
        if (AnimalsInBuilding.Count >= MaxAnimalCapacity) return;
        AnimalsInBuilding.Add(animal);
    }

    public void RemoveAnimal(Animals animal){
        AnimalsInBuilding.Remove(animal);
    }
}
