// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.Tilemaps;

// public class BarnT3 : Barn {
//     // public BarnT3(Vector3Int[] position, Vector3Int[] basePosition, Tilemap tilemap) : base(position, basePosition, tilemap) {
//     //     Init();
//     // }

//     // public BarnT3() : base() {
//     //     Init();
//     // }

//     protected override void Init(){
//         name = GetType().Name;
//         texture = Resources.Load("Buildings/Deluxe Barn") as Texture2D;
//         insideAreaTexture = Resources.Load("BuildingInsides/Barn3") as Texture2D;
//         _materialsNeeded = new Dictionary<Materials, int> {
//             {Materials.Coins, 6000 + 12000 + 25000},
//             {Materials.Wood, 350 + 450 + 550},
//             {Materials.Stone, 150 + 200 + 300},
//         };
//         base.Init();
//     }
// }
