using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITieredBuilding{
    int Tier {get;}
    void SetTier(int tier);
}

public interface IMultipleTypeBuilding<T> where T : Enum{
    T Type {get;}
    void CycleType();
    void SetType(T type);
    GameObject[] CreateButtonsForAllTypes();
}

public interface IAnimalHouse{
    List<KeyValuePair<Animals, GameObject>> AnimalsInBuilding {get;}
    bool AddAnimal(Animals animal);
    void ToggleAnimalMenu();
}

public interface IRangeEffectBuilding{
    void ShowEffectRange(Vector3Int[] RangeArea);
    void HideEffectRange();
}

public interface IConnectingBuilding{
    int GetConnectingFlags(Vector3Int position, List<Vector3Int> otherBuildings);
}

public interface IEnterableBuilding{
    Vector3Int[] InteriorUnavailableCoordinates {get;}
    Vector3Int[] InteriorPlantableCoordinates {get;}
    void ToggleEditBuildingInterior();
    void EditBuildingInterior();
    void ExitBuildingInteriorEditing();
    
}
