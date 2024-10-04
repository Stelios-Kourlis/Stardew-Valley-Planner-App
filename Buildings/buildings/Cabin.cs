using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.U2D;
using UnityEngine.UI;
using static FlooringComponent;
using static Utility.ClassManager;
using static WallsComponent;

public class Cabin : Building, IExtraActionBuilding {

    public enum Types {
        Wood,
        Plank,
        Stone,
        Beach,
        Neighbor,
        Rustic,
        Trailer
    }
    private static readonly bool[] cabinTypeIsPlaced = new bool[Enum.GetValues(typeof(Types)).Length];
    private MultipleTypeBuildingComponent MultipleTypeBuildingComponent => gameObject.GetComponent<MultipleTypeBuildingComponent>();
    private TieredBuildingComponent TieredBuildingComponent => gameObject.GetComponent<TieredBuildingComponent>();
    private InteractableBuildingComponent InteractableBuildingComponent => gameObject.GetComponent<InteractableBuildingComponent>();
    private EnterableBuildingComponent EnterableBuildingComponent => gameObject.GetComponent<EnterableBuildingComponent>();

    public Enum Type => GetComponent<MultipleTypeBuildingComponent>().Type;

    public override void OnAwake() {
        BaseHeight = 3;
        BuildingName = "Cabin";
        base.OnAwake();
        for (int i = 0; i < cabinTypeIsPlaced.Length; i++) cabinTypeIsPlaced[i] = false;
        gameObject.AddComponent<MultipleTypeBuildingComponent>().SetEnumType(typeof(Types));
        gameObject.AddComponent<TieredBuildingComponent>().SetMaxTier(4);
        gameObject.AddComponent<EnterableBuildingComponent>().AddInteriorInteractions(
            new HashSet<ButtonTypes> {
                ButtonTypes.TIER_ONE,
                ButtonTypes.TIER_TWO,
                ButtonTypes.TIER_THREE,
            }
        ).AddWalls(new Dictionary<int, List<WallOrigin>>{
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
                    new FlooringOrigin(new Vector3Int(20, 1, 0), 9, 10)
                    }
                },
            {3, new(){
                    new FlooringOrigin(new Vector3Int(1, 1, 0), 9, 12, 47),
                    new FlooringOrigin(new Vector3Int(10, 3, 0), 11, 11),
                    new FlooringOrigin(new Vector3Int(1, 15, 0), 12, 6, 1),
                    new FlooringOrigin(new Vector3Int(13, 15, 0), 15, 6, 31),
                    new FlooringOrigin(new Vector3Int(21, 5, 0), 2, 2),
                    new FlooringOrigin(new Vector3Int(23, 1, 0), 11, 11)
                }
            }
        });
    }

    public void PerformExtraActionsOnPlace(Vector3Int position) {
        if (cabinTypeIsPlaced[Convert.ToInt32(Type)]) {//this check is not enforced
            NotificationManager.Instance.SendNotification("You can only have one of each type of cabin", NotificationManager.Icons.ErrorIcon);
            return;
        }
        cabinTypeIsPlaced[Convert.ToInt32(Type)] = true;
    }

    public void PerformExtraActionsOnDelete() {
        cabinTypeIsPlaced[Convert.ToInt32(Type)] = false;
    }

    public override List<MaterialCostEntry> GetMaterialsNeeded() {
        List<MaterialCostEntry> level1 = new() { new(100, Materials.Coins) };
        return TieredBuildingComponent.Tier switch {
            2 => new List<MaterialCostEntry>{
                new(10_000, Materials.Coins),
                new(450, Materials.Wood),
            }.Union(level1).ToList(),
            3 => new List<MaterialCostEntry>{
                new(60_000, Materials.Coins),
                new(450, Materials.Wood),
                new(150, Materials.Hardwood),
            }.Union(level1).ToList(),
            4 => new List<MaterialCostEntry>{
                new(160_000, Materials.Coins),
                new(450, Materials.Wood),
                new(150, Materials.Hardwood),
            }.Union(level1).ToList(),
            _ => throw new System.Exception("Invalid Cabin Tier")
        };
    }

    public string GetExtraData() {
        return $"{TieredBuildingComponent.Tier}|{MultipleTypeBuildingComponent.Type}";
    }

    public void LoadExtraBuildingData(string[] data) {
        TieredBuildingComponent.SetTier(int.Parse(data[0]));
        MultipleTypeBuildingComponent.SetType((Types)Enum.Parse(typeof(Types), data[1]));
    }
}
