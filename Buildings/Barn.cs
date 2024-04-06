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

public class Barn : TieredBuilding, IAnimalHouse {
    private SpriteAtlas animalAtlas;
    private SpriteAtlas animalsInBuildingPanelBackgroundAtlas;
    private List<KeyValuePair<Animals, GameObject>> animals = new List<KeyValuePair<Animals, GameObject>>();
    private int animalCapacity;

    public override string TooltipMessage => "Right Click For More Options";

    public override void OnAwake(){
        baseHeight = 4;
        buildingInteractions = new ButtonTypes[]{
            ButtonTypes.TIER_ONE,
            ButtonTypes.TIER_TWO,
            ButtonTypes.TIER_THREE,
            ButtonTypes.ENTER,
            ButtonTypes.PAINT,
            ButtonTypes.ADD_ANIMAL
        };
        MaxTier = 3;
        base.OnAwake();
        animalAtlas = Resources.Load("BarnAnimalsAtlas") as SpriteAtlas;
        animalsInBuildingPanelBackgroundAtlas = Resources.Load("UI/AnimalsInBuildingAtlas") as SpriteAtlas;
    }

    public override void ChangeTier(int tier){
        base.ChangeTier(tier);

        //Update Animals
        animalCapacity = 4 * tier;
        List<KeyValuePair<Animals, GameObject>> animalsToRemove = new List<KeyValuePair<Animals, GameObject>>();
        string animalsRemoved = GetRemovedAnimals();
        if (tier < 2) animalsToRemove.AddRange(animals.Where(animal => animal.Key == Animals.Goat));
        if (tier < 3) animalsToRemove.AddRange(animals.Where(animal => animal.Key == Animals.Sheep || animal.Key == Animals.Pig));
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

        if (buttonParent == null) return;
        buttonParent.transform.GetChild(5).GetChild(1).GetComponent<RectTransform>().sizeDelta = new Vector2(620, 100 * Tier + 100);
        buttonParent.transform.GetChild(5).GetChild(1).GetComponent<Image>().sprite = animalsInBuildingPanelBackgroundAtlas.GetSprite($"AnimalsInBuilding{animalCapacity}");
        buttonParent.transform.GetChild(5).GetChild(1).GetChild(0).GetComponent<RectTransform>().sizeDelta= new Vector2(580, 100 * Tier + 100 - 50);
    }

    private string GetRemovedAnimals(){
        int goatCount = animals.Count(animal => animal.Key == Animals.Goat);
        string goatsRemoved = goatCount > 0 ? $"{goatCount} Goat" : "";
        if (goatCount > 1) goatsRemoved += "s";
        goatsRemoved += ",";

        int sheepCount = animals.Count(animal => animal.Key == Animals.Sheep);
        string sheepRemoved = sheepCount > 0 ? $"{sheepCount} Sheep," : "";

        int pigCount = animals.Count(animal => animal.Key == Animals.Pig);
        string pigsRemoved = pigCount > 0 ? $"{pigCount} Pig" : "";
        return $"{goatsRemoved} {sheepRemoved} {pigsRemoved}";
    }

    public override List<MaterialInfo> GetMaterialsNeeded(){
        int animalCost = 0;
        foreach (var animal in animals){
            animalCost += animal.Key switch{
                Animals.Cow => 1_500,
                // Animals.Ostrich => Egg,//todo Add ostrich egg
                Animals.Goat => 4_000,
                Animals.Sheep => 8_000,
                Animals.Pig => 16_000,
                _ => throw new System.ArgumentException($"Invalid animal {animal.Key}")
            };
            
        }
        return Tier switch{
            1 => new List<MaterialInfo>{
                new MaterialInfo(6_000 + animalCost, Materials.Coins),
                new MaterialInfo(350, Materials.Wood),
                new MaterialInfo(150, Materials.Stone)
            },
            2 => new List<MaterialInfo>{
                new MaterialInfo(6_000 + 12_000 + animalCost, Materials.Coins),
                new MaterialInfo(350 + 450, Materials.Wood),
                new MaterialInfo(150 + 200, Materials.Stone)
            },
            3 => new List<MaterialInfo>{
                new MaterialInfo(6_000 + 12_000 + 25_000 + animalCost, Materials.Coins),
                new MaterialInfo(350 + 450 + 550, Materials.Wood),
                new MaterialInfo(150 + 200 + 300, Materials.Stone)
            },
            _ => throw new System.ArgumentException($"Invalid tier {Tier}")
        };
    }

    public override string GetBuildingData(){
        return base.GetBuildingData() + $"|{Tier}";
    }

    public override void RecreateBuildingForData(int x, int y, params string[] data){
        OnAwake();
        Place(new Vector3Int(x,y,0));
        ChangeTier(int.Parse(data[0]));
    }

    public void AddAnimal(Animals animal){
        List<Animals> allowedAnimals = new List<Animals>{Animals.Cow, Animals.Ostrich};
        if (Tier >= 2) allowedAnimals.Add(Animals.Goat);
        if (Tier == 3) { allowedAnimals.Add(Animals.Sheep); allowedAnimals.Add(Animals.Pig); }
        if (!allowedAnimals.Contains(animal)) {GetNotificationManager().SendNotification($"Cannot add {animal} to barn tier {Tier}"); return;}
        if (animals.Count >= animalCapacity) {GetNotificationManager().SendNotification($"Barn is full ({animals.Count}/{animalCapacity}), cannot add {animal}"); return;}
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
        button.transform.localScale = new Vector3(1, 1);
    }
}
