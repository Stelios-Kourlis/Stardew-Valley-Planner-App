// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.PlayerLoop;
// using UnityEngine.Tilemaps;

// public class BarnT2 : Barn {
//     // public BarnT2(Vector3Int[] position, Vector3Int[] basePosition, Tilemap tilemap) : base(position, basePosition, tilemap) {
//     //     Init();
//     // }

//     // public BarnT2() : base() {
//     //     Init();
//     // }

//     protected override void Init(){
//         name = GetType().Name;
//         texture = Resources.Load("Buildings/Big Barn") as Texture2D;
//         insideAreaTexture = Resources.Load("BuildingInsides/Barn2") as Texture2D;
//         _materialsNeeded = new Dictionary<Materials, int> {
//             {Materials.Coins, 6000 + 12000},
//             {Materials.Wood, 350 + 450},
//             {Materials.Stone, 150 + 200},
//         };
//         base.Init();
//     }
// }
