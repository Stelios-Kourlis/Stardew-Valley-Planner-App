// using System;
// using System.Collections;
// using System.Collections.Generic;
// using System.Linq;
// using UnityEngine;
// using UnityEngine.UI;
// using UnityEngine.U2D;
// using static Utility.TilemapManager;
// using static Utility.ClassManager;
// using static Utility.SpriteManager;
// using UnityEngine.Tilemaps;

public class Fence/* : MultipleTypeBuilding<Fence.Types>*/{

    public enum Types{
        NO_TYPE,
        Wood,
        Stone,
        Iron,
        Hardwood
    }

//     public override string TooltipMessage => "";
//     public static event Action<Vector3Int> FenceWasPlaced;
//     private Vector3Int position;
//     private static readonly List<Vector3Int> fences = new List<Vector3Int>();
//     public override List<MaterialInfo> GetMaterialsNeeded(){
//         return Type switch{
//             Types.Wood => new List<MaterialInfo>(){
//                 new MaterialInfo(2, Materials.Wood)
//             },
//             Types.Stone => new List<MaterialInfo>(){
//                 new MaterialInfo(2, Materials.Stone)
//             },
//             Types.Iron => new List<MaterialInfo>(){
//                 new MaterialInfo(1, Materials.IronBar)
//             },
//             Types.Hardwood => new List<MaterialInfo>(){
//                 new MaterialInfo(1, Materials.Hardwood)
//             },
//             _ => throw new System.Exception("Invalid Fence Type")
//         };
//     }

//     public override void RecreateBuildingForData(int x, int y, params string[] data){
//         throw new System.NotImplementedException();
//     }

//     public override void OnAwake(){
//         baseHeight = 1;
//         if (CurrentType == Types.NO_TYPE) CurrentType = Types.Wood;
//         base.OnAwake();
//         defaultSprite = atlas.GetSprite($"WoodFence0");
//         FenceWasPlaced += AnotherFenceWasPlaced;
//         sprite = atlas.GetSprite($"{Type}Fence0");
//     }

//     public override void Place(Vector3Int position){
//         base.Place(position);
//         fences.Add(position);
//         this.position = position;
//         hasBeenPlaced = true;
//         FenceWasPlaced?.Invoke(position);
//         UpdateTexture(atlas.GetSprite($"{Type}Fence{GetFencesFlagsSum(position)}"));
//     }

//     protected override void PlacePreview(Vector3Int position){
//         if (hasBeenPlaced) return;
//         UpdateTexture(atlas.GetSprite($"{Type}Fence{GetFencesFlagsSum(position)}"));
//         base.PlacePreview(position);
//     }

//     public override void Delete(){
//         if (!hasBeenPlaced) return;
//         fences.Remove(position);
//         FenceWasPlaced -= AnotherFenceWasPlaced;//for some reason this is needed or it crashes at line 'this.gameObject?.GetComponent<Tilemap>().SetTiles(spriteCoordinates.ToArray(), buildingTiles);' at base.UpdateTexture
//         FenceWasPlaced?.Invoke(position);
//         base.Delete();
//     }

//     private void AnotherFenceWasPlaced(Vector3Int position){
//         List<Vector3Int> neighbors = GetCrossAroundPosition(position).ToList();
//         // Debug.Log(gameObject == null);
//         if (neighbors.Contains(this.position)){
//             UpdateTexture(atlas?.GetSprite($"{Type}Fence{GetFencesFlagsSum(this.position)}"));
//         }
//     }

//     private int GetFencesFlagsSum(Vector3Int position){
//         List<FloorFlag> flags = new List<FloorFlag>();
//         Vector3Int[] neighbors = GetCrossAroundPosition(position).ToArray();
//         if (fences.Contains(neighbors[0])) flags.Add(FloorFlag.LEFT_ATTACHED);
//         if (fences.Contains(neighbors[1])) flags.Add(FloorFlag.RIGHT_ATTACHED);
//         if (fences.Contains(neighbors[3])) flags.Add(FloorFlag.TOP_ATTACHED);
//         return flags.Cast<int>().Sum();
//     }

//     public override GameObject[] CreateButtonsForAllTypes(){
//         List<GameObject> buttons = new List<GameObject>();
//         foreach (Types type in Enum.GetValues(typeof(Types))){
//             if ((int)(object)type == 0) continue; // skip the first element (None)
//             GameObject button = Instantiate(Resources.Load<GameObject>("UI/BuildingButton"));
//             button.name = $"{type}Button";
//             button.GetComponent<Image>().sprite = atlas.GetSprite($"{type}Fence0");

//             Type buildingType = GetType();
//             BuildingController buildingController = GetBuildingController();
//             button.GetComponent<Button>().onClick.AddListener(() => { 
//                 Debug.Log($"Setting current building to {type}");
//                 buildingController.SetCurrentBuildingToMultipleTypeBuilding<Types>(buildingType, type);
//                 buildingController.SetCurrentAction(Actions.PLACE); 
//                 });
//             buttons.Add(button);
//         }
//         return buttons.ToArray();
//     }
}
