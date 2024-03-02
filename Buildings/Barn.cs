using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.U2D;

public class Barn : Building, ITieredBuilding, IAnimalHouse {
    private SpriteAtlas atlas;
    private int tier = 0;
    private List<Animals> animals = new List<Animals>();
    private int animalCapacity;

    public new void Start(){
        baseHeight = 4;
        buildingInteractions = new ButtonTypes[]{
            ButtonTypes.TIER_ONE,
            ButtonTypes.TIER_TWO,
            ButtonTypes.TIER_THREE,
            ButtonTypes.ENTER,
            ButtonTypes.PAINT,
            ButtonTypes.ADD_ANIMAL
        };
        base.Start();
        atlas = Resources.Load("Buildings/BarnAtlas") as SpriteAtlas;
        if (tier == 0) ChangeTier(1);
    }

    public void ChangeTier(int tier){//todo on tier decrease check animals
        if (tier < 0 || tier > 3) throw new System.ArgumentException($"Tier must be between 1 and 3 (got {tier})");
        this.tier = tier;
        animalCapacity = 4 * tier;
        UpdateTexture(atlas.GetSprite($"BarnA_{tier}"));
    }

    public override List<MaterialInfo> GetMaterialsNeeded(){
        return tier switch{
            1 => new List<MaterialInfo>{
                new MaterialInfo(6_000, Materials.Coins),
                new MaterialInfo(350, Materials.Wood),
                new MaterialInfo(150, Materials.Stone)
            },
            2 => new List<MaterialInfo>{
                new MaterialInfo(6_000 + 12_000, Materials.Coins),
                new MaterialInfo(350 + 450, Materials.Wood),
                new MaterialInfo(150 + 200, Materials.Stone)
            },
            3 => new List<MaterialInfo>{
                new MaterialInfo(6_000 + 12_000 + 25_000, Materials.Coins),
                new MaterialInfo(350 + 450 + 550, Materials.Wood),
                new MaterialInfo(150 + 200 + 300, Materials.Stone)
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
        List<Animals> allowedAnimals = new List<Animals>{Animals.Cow, Animals.Ostrich};
        if (tier >= 2) allowedAnimals.Add(Animals.Goat);
        if (tier == 3) { allowedAnimals.Add(Animals.Sheep); allowedAnimals.Add(Animals.Pig); }
        if (!allowedAnimals.Contains(animal)) throw new System.ArgumentException($"Animal {animal} is not allowed in a level {tier} barn");
        if (animals.Count >= animalCapacity) throw new System.ArgumentException($"Barn is full ({animals.Count}/{animalCapacity}), cannot add {animal}");
        animals.Add(animal);
        Debug.Log($"Added {animal} to the list\n--||--");
        foreach (var item in animals){
            Debug.Log(item);
        }

    }
}
