using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;
using static Utility.TilemapManager;
using static Utility.BuildingManager;
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
        // AddAnimalMenuObject(null, building);
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

    private void AddAnimalMenuObject(Button button, Building building){
        //Animal Add
        GameObject animalMenuPrefab = building.GetType() switch{
            Type t when t == typeof(Coop) => Resources.Load<GameObject>("UI/CoopAnimalMenu"),
            Type t when t == typeof(Barn) => Resources.Load<GameObject>("UI/BarnAnimalMenu"),
            _ => throw new ArgumentException("This should never happen")
        };
        //GameObject animalMenuPrefab = Resources.Load<GameObject>("UI/BarnAnimalMenu");
        GameObject animalMenu = GameObject.Instantiate(animalMenuPrefab);
        animalMenu.transform.SetParent(button.transform);
        Vector3 animalMenuPositionWorld = new(building.Tilemap.CellToWorld(building.BaseCoordinates[0] + new Vector3Int(1,0,0)).x, GetMiddleOfBuildingWorld(building).y + 4);
        animalMenu.transform.position = Camera.main.WorldToScreenPoint(animalMenuPositionWorld);
        animalMenu.GetComponent<RectTransform>().localScale = new Vector2(1, 1);
        animalMenu.SetActive(false);
        GameObject animalMenuContent = animalMenu.transform.GetChild(0).gameObject;
        IAnimalHouse animalHouse = building as IAnimalHouse;
        for (int childIndex = 0; childIndex < animalMenuContent.transform.childCount; childIndex++){
            Button addAnimalButton = animalMenuContent.transform.GetChild(childIndex).GetComponent<Button>();
            AddHoverEffect(addAnimalButton);
            addAnimalButton.onClick.AddListener(() => {
                animalHouse.AddAnimal((Animals)Enum.Parse(typeof(Animals), addAnimalButton.gameObject.name));
            });
        }

        //Animal Remove
        GameObject animalInBuildingMenuPrefab = Resources.Load<GameObject>("UI/AnimalsInBuilding");
        GameObject animalInBuilding = GameObject.Instantiate(animalInBuildingMenuPrefab);
        animalInBuilding.transform.SetParent(button.transform);
        Vector3 animalInBuildingMenuPositionWorld = new(building.Tilemap.CellToWorld(building.BaseCoordinates[0] + new Vector3Int(1,0,0)).x, GetMiddleOfBuildingWorld(building).y + 1);
        animalInBuilding.transform.position = Camera.main.WorldToScreenPoint(animalInBuildingMenuPositionWorld);
        animalInBuilding.GetComponent<RectTransform>().localScale = new Vector2(1, 1);
        animalInBuilding.SetActive(false);
        
    }
}
