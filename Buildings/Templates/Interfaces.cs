using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBuilding{
    Vector3Int[] SpriteCoordinates {get;}
    Vector3Int[] BaseCoordinates {get;}
}

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
    // AnimalHouse AnimalHouseComponent {get;}
    List<KeyValuePair<Animals, GameObject>> AnimalsInBuilding {get;}
    bool AddAnimal(Animals animal);
    void ToggleAnimalMenu();
}

public interface IRangeEffectBuilding{
    void ShowEffectRange(Vector3Int[] RangeArea);
    void HideEffectRange();
}

partial interface IConnectingBuilding{
    int GetConnectingFlags(Vector3Int position, List<Vector3Int> otherBuildings);
}
