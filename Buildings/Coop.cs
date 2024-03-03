using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.U2D;
using UnityEngine.UI;
using static Utility.BuildingManager;
using static Utility.ClassManager;

public class Coop : Building, ITieredBuilding, IAnimalHouse {
    private SpriteAtlas atlas;
    private SpriteAtlas animalAtlas;
    private int tier = 0;
    private List<KeyValuePair<Animals, GameObject>> animals = new List<KeyValuePair<Animals, GameObject>>();
    private int animalCapacity;

    public new void Start(){
        baseHeight = 3;
        buildingInteractions = new ButtonTypes[]{
            ButtonTypes.TIER_ONE,
            ButtonTypes.TIER_TWO,
            ButtonTypes.TIER_THREE,
            ButtonTypes.ENTER,
            ButtonTypes.PAINT,
            ButtonTypes.ADD_ANIMAL
        };
        base.Start();
        atlas = Resources.Load("Buildings/CoopAtlas") as SpriteAtlas;
        animalAtlas = Resources.Load("CoopAnimalsAtlas") as SpriteAtlas;
        if (tier == 0) ChangeTier(1);
    }

    public void ChangeTier(int tier){
        if (tier < 0 || tier > 3) throw new System.ArgumentException($"Tier must be between 1 and 3 (got {tier})");
        this.tier = tier;
        animalCapacity = 4 * tier;
        UpdateTexture(atlas.GetSprite($"CoopAtlas_{tier}"));

        //Update Animals
        List<KeyValuePair<Animals, GameObject>> animalsToRemove = new List<KeyValuePair<Animals, GameObject>>();

        string animalsRemoved = GetRemovedAnimals();
        if (tier < 2) animalsToRemove.AddRange(animals.Where(animal => animal.Key == Animals.Duck || animal.Key == Animals.VoidChicken || animal.Key == Animals.Dinosaur || animal.Key == Animals.GoldenChicken));
        if (tier < 3) animalsToRemove.AddRange(animals.Where(animal => animal.Key == Animals.Rabbit));
        if (animalsToRemove.Count != 0 ) GetNotificationManager().SendNotification($"Removed {animalsRemoved} because they aren't allowed in tier {tier} {GetType()}");

        foreach (var pair in animalsToRemove){
                Destroy(pair.Value);
                animals.Remove(pair);
            }

        if (animals.Count > animalCapacity) GetNotificationManager().SendNotification($"Removed {animals.Count - animalCapacity} animals that exceed the new capacity of {GetType()}");
        while (animals.Count > animalCapacity){
            Destroy(animals.Last().Value);
            animals.Remove(animals.Last());
        }
    }

    private string GetRemovedAnimals(){
        int rabbitCount = animals.Count(animal => animal.Key == Animals.Rabbit);
        string rabbitsRemoved = rabbitCount > 0 ? $"{rabbitCount} Rabbit" : "";
        if (rabbitCount > 1) rabbitsRemoved += "s";
        rabbitsRemoved += ",";

        int duckCount = animals.Count(animal => animal.Key == Animals.Duck);
        string ducksRemoved = duckCount > 0 ? $"{duckCount} Duck" : "";
        if (duckCount > 1) ducksRemoved += "s";
        ducksRemoved += ",";

        int voidChickenCount = animals.Count(animal => animal.Key == Animals.VoidChicken);
        string voidChickensRemoved = voidChickenCount > 0 ? $"{voidChickenCount} Void Chicken" : "";
        if (voidChickenCount > 1) voidChickensRemoved += "s";
        voidChickensRemoved += ",";

        int dinosaurCount = animals.Count(animal => animal.Key == Animals.Dinosaur);
        string dinosaursRemoved = dinosaurCount > 0 ? $"{dinosaurCount} Dinosaur" : "";
        if (dinosaurCount > 1) dinosaursRemoved += "s";
        dinosaursRemoved += ",";

        int goldenChickenCount = animals.Count(animal => animal.Key == Animals.GoldenChicken);
        string goldenChickensRemoved = goldenChickenCount > 0 ? $"{goldenChickenCount} Golden Chicken" : "";
        if (goldenChickenCount > 1) goldenChickensRemoved += "s";

        return $"{rabbitsRemoved} {ducksRemoved} {voidChickensRemoved} {dinosaursRemoved} {goldenChickensRemoved}";
    }

    public override List<MaterialInfo> GetMaterialsNeeded(){
        return tier switch{
            1 => new List<MaterialInfo>{
                new MaterialInfo(4_000, Materials.Coins),
                new MaterialInfo(300, Materials.Wood),
                new MaterialInfo(100, Materials.Stone)
            },
            2 => new List<MaterialInfo>{
                new MaterialInfo(4_000 + 10_000, Materials.Coins),
                new MaterialInfo(300 + 400, Materials.Wood),
                new MaterialInfo(100 + 150, Materials.Stone)
            },
            3 => new List<MaterialInfo>{
                new MaterialInfo(4_000 + 10_000 + 20_000, Materials.Coins),
                new MaterialInfo(300 + 400 + 500, Materials.Wood),
                new MaterialInfo(100 + 150 + 200, Materials.Stone)
            },
            _ => throw new System.ArgumentException($"Invalid tier {tier}")
        };
    }

    public override string GetBuildingData(){
        return base.GetBuildingData() + $"|{tier}";
    }

    public override void RecreateBuildingForData(int x, int y, params string[] data){
        Start();
        Place(new Vector3Int(x,y,0));
        ChangeTier(int.Parse(data[0]));
    }

    public void AddAnimal(Animals animal){
        List<Animals> allowedAnimals = new List<Animals>{Animals.Chicken};
        if (tier >= 2) allowedAnimals.AddRange( new List<Animals>{Animals.Duck, Animals.VoidChicken, Animals.Dinosaur, Animals.GoldenChicken});
        if (tier == 3) allowedAnimals.Add(Animals.Rabbit);
        if (!allowedAnimals.Contains(animal)) {GetNotificationManager().SendNotification($"Animal {animal} is not allowed in a level {tier} coop"); return;}
        if (animals.Count >= animalCapacity) {GetNotificationManager().SendNotification($"Coop is full ({animals.Count}/{animalCapacity}), cannot add {animal}"); return;}
        AddAnimalButton(animal);
    }

    private void AddAnimalButton(Animals animal){
        buttonParent.transform.GetChild(5);
        GameObject button = new GameObject(animal.ToString());
        animals.Add(new KeyValuePair<Animals, GameObject>(animal, button));
        button.transform.SetParent(buttonParent.transform.GetChild(5).GetChild(1).GetChild(0));
        button.AddComponent<Image>().sprite = animalAtlas.GetSprite(animal.ToString());
        button.AddComponent<Button>().onClick.AddListener(() => {
            animals.Remove(new KeyValuePair<Animals, GameObject>(animal, button));
            Destroy(button);
        });
        AddHoverEffect(button.GetComponent<Button>());
    }
}
