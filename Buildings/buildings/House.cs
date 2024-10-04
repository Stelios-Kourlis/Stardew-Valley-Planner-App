using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.U2D;
using static FlooringComponent;
using static WallsComponent;

public class House : Building {

    public TieredBuildingComponent TieredBuildingComponent { get; private set; }

    public EnterableBuildingComponent EnterableBuildingComponent { get; private set; }

    public HashSet<ButtonTypes> BuildingInteractions => gameObject.GetComponent<InteractableBuildingComponent>().BuildingInteractions;

    public GameObject ButtonParentGameObject => gameObject.GetComponent<InteractableBuildingComponent>().ButtonParentGameObject;

    public override void OnAwake() {
        BaseHeight = 6;
        BuildingName = "House";
        base.OnAwake();
        TieredBuildingComponent = gameObject.AddComponent<TieredBuildingComponent>().SetMaxTier(3);
        EnterableBuildingComponent = gameObject.AddComponent<EnterableBuildingComponent>().AddInteriorInteractions(
            new HashSet<ButtonTypes> {
                ButtonTypes.TIER_ONE,
                ButtonTypes.TIER_TWO,
                ButtonTypes.TIER_THREE,
                ButtonTypes.CUSTOMIZE_HOUSE_RENOVATIONS
            }).AddWalls(new Dictionary<int, List<WallOrigin>>{
            {1, new(){
                    new WallOrigin(new Vector3Int(1, 8, 0), 10)
                    }
            },
            {2, new(){
                    new WallOrigin(new Vector3Int(1, 8, 0), 17),
                    new WallOrigin(new Vector3Int(18, 3, 0), 2),
                    new WallOrigin(new Vector3Int(20, 8, 0), 9),
                }
            },
            {3, new(){
                    new WallOrigin(new Vector3Int(1, 11, 0), 10),
                    new WallOrigin(new Vector3Int(13, 11, 0), 8),
                    new WallOrigin(new Vector3Int(1, 20, 0), 12, 11),
                    new WallOrigin(new Vector3Int(13, 18, 0), 2, 61),
                    new WallOrigin(new Vector3Int(15, 20, 0), 13, 61),
                    new WallOrigin(new Vector3Int(21, 7, 0), 2),
                    new WallOrigin(new Vector3Int(23, 11, 0), 11),
                }
            }
        }).AddFloors(new Dictionary<int, List<FlooringOrigin>>{
            {1, new(){
                    new FlooringOrigin(new Vector3Int(1, 0, 0), 10, 9)
                    }
                },
            {2, new(){
                    new FlooringOrigin(new Vector3Int(1, 0, 0), 6, 9, 47),
                    new FlooringOrigin(new Vector3Int(7, 0, 0), 11, 9),
                    new FlooringOrigin(new Vector3Int(18, 2, 0), 2, 2),
                    new FlooringOrigin(new Vector3Int(20, 1, 0), 10, 10)
                    }
                },
            {3, new(){
                    new FlooringOrigin(new Vector3Int(1, 1, 0), 9, 12, 47),
                    new FlooringOrigin(new Vector3Int(10, 3, 0), 11, 11),
                    new FlooringOrigin(new Vector3Int(1, 15, 0), 12, 6, 1),
                    new FlooringOrigin(new Vector3Int(13, 15, 0), 15, 6, 31),
                    new FlooringOrigin(new Vector3Int(21, 5, 0), 2, 2),
                    new FlooringOrigin(new Vector3Int(23, 1, 0), 12, 11)
                }
            }
        });
        gameObject.AddComponent<HouseExtensionsComponent>();
    }

    public void SetTier(int tier) {
        TieredBuildingComponent.SetTier(tier);
        // if (tier == 4) UpdateTexture(TieredBuildingComponent.Atlas.GetSprite($"House3"));//4 has same sprite as 3
    }

    public override List<MaterialCostEntry> GetMaterialsNeeded() {
        return TieredBuildingComponent.Tier switch {
            1 => new List<MaterialCostEntry> { new("Free") },
            2 => new List<MaterialCostEntry>{
                new(10_000, Materials.Coins),
                new(450, Materials.Wood),
            },
            3 => new List<MaterialCostEntry>{
                new(10_000 + 65_000, Materials.Coins),
                new(450, Materials.Wood),
                new(100, Materials.Hardwood),
            },
            4 => new List<MaterialCostEntry>{
                new(10_000 + 65_000 + 100_000, Materials.Coins),
                new(450, Materials.Wood),
                new(100, Materials.Hardwood),
            },
            _ => throw new System.ArgumentException($"Invalid tier {TieredBuildingComponent.Tier}")
        };
    }

    public string GetExtraData() {
        return $"{TieredBuildingComponent.Tier}";
    }

    public void LoadExtraBuildingData(string[] data) {
        SetTier(int.Parse(data[0]));
    }

    public void OnMouseRightClick() {
        if (!BuildingController.isInsideBuilding.Key) ButtonParentGameObject.SetActive(!ButtonParentGameObject.activeSelf);
    }
}
