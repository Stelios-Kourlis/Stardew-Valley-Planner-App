using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using static Utility.TilemapManager;

public class SlimeHutch : Building, IEnterableBuilding {
    public EnterableBuilding EnterableBuildingComponent {get; private set;}

    public Vector3Int[] InteriorUnavailableCoordinates {get; private set;}

    public Vector3Int[] InteriorPlantableCoordinates {get; private set;}

    public override void OnAwake(){
        buildingName = "Slime Hutch";
        BaseHeight = 4;
        BuildingInteractions = new ButtonTypes[]{
            ButtonTypes.ENTER
        };
        EnterableBuildingComponent = new EnterableBuilding(this);
        base.OnAwake();
    }

    public override void Place(Vector3Int position){
        base.Place(position);   
        EnterableBuildingComponent.AddBuildingInterior();
        Vector3Int interiorLowerLeftCorner = EnterableBuildingComponent.InteriorAreaCoordinates[0];
        HashSet<Vector3Int> interiorUnavailableCoordinates = new();
        for (int i = 0; i < 18; i++){//front and back walls
            if (i != 8) interiorUnavailableCoordinates.Add(interiorLowerLeftCorner + new Vector3Int(i, 1, 0));
            interiorUnavailableCoordinates.Add(interiorLowerLeftCorner + new Vector3Int(i, 0, 0));
            interiorUnavailableCoordinates.Add(interiorLowerLeftCorner + new Vector3Int(i, 10, 0));
            interiorUnavailableCoordinates.Add(interiorLowerLeftCorner + new Vector3Int(i, 11, 0));
            interiorUnavailableCoordinates.Add(interiorLowerLeftCorner + new Vector3Int(i, 12, 0));
            interiorUnavailableCoordinates.Add(interiorLowerLeftCorner + new Vector3Int(i, 13, 0));
        }
        for (int i = 0; i < 14; i++){//side walls
            interiorUnavailableCoordinates.Add(interiorLowerLeftCorner + new Vector3Int(0, i, 0));
            interiorUnavailableCoordinates.Add(interiorLowerLeftCorner + new Vector3Int(17, i, 0));
        }//slime hutch extra
        interiorUnavailableCoordinates.Add(interiorLowerLeftCorner + new Vector3Int(16, 2, 0));
        interiorUnavailableCoordinates.Add(interiorLowerLeftCorner + new Vector3Int(16, 4, 0));
        interiorUnavailableCoordinates.Add(interiorLowerLeftCorner + new Vector3Int(16, 5, 0));
        interiorUnavailableCoordinates.Add(interiorLowerLeftCorner + new Vector3Int(16, 6, 0));
        interiorUnavailableCoordinates.Add(interiorLowerLeftCorner + new Vector3Int(16, 7, 0));
        interiorUnavailableCoordinates.Add(interiorLowerLeftCorner + new Vector3Int(16, 9, 0));

        InteriorUnavailableCoordinates = GetAllInteriorUnavailableCoordinates(interiorUnavailableCoordinates.ToArray()).ToArray();
    }

    public override List<MaterialInfo> GetMaterialsNeeded(){
        return new List<MaterialInfo>(){
            new(10000, Materials.Coins),
            new(500, Materials.Stone),
            new(10, Materials.RefinedQuartz),
            new(1, Materials.IridiumBar)
        };
    }

    public override void RecreateBuildingForData(int x, int y, params string[] data){
        OnAwake();
        Place(new Vector3Int(x,y,0));
    }

    public void EditBuildingInterior() => EnterableBuildingComponent.EditBuildingInterior();

    public void ExitBuildingInteriorEditing() => EnterableBuildingComponent.ExitBuildingInteriorEditing();
    
    public void ToggleEditBuildingInterior() => EnterableBuildingComponent.ToggleEditBuildingInterior();
}
