// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.PlayerLoop;
// using UnityEngine.Tilemaps;

// public class BarnT1 : Barn {
//     // public BarnT1(Vector3Int[] position, Vector3Int[] basePosition, Tilemap tilemap) : base(position, basePosition, tilemap) {
//     //     Init();
//     // }

//     // public BarnT1() : base() {
//     //     Init();
//     // }

//     protected override void Init(){
//         name = GetType().Name;
//         texture = Resources.Load("Buildings/Barn") as Texture2D;
//         insideAreaTexture = Resources.Load("BuildingInsides/Barn1") as Texture2D;
//         _materialsNeeded = new Dictionary<Materials, int> {
//             {Materials.Coins, 6000},
//             {Materials.Wood, 350},
//             {Materials.Stone, 150},
//         };
//         base.Init();
//     }
// }
