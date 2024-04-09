using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBuilding{
    //not needed?
}

public interface ITieredBuilding{
    TieredBuilding TieredBuildingCompenent {get;}
    void SetTier(int tier) => TieredBuildingCompenent.SetTier(tier);
}

public interface IMultipleTypeBuilding<T> where T : Enum{
    MultipleTypeBuilding<T> MultipleTypeBuildingComponent {get;}
    T Type => MultipleTypeBuildingComponent.Type;
    static T CurrentType => MultipleTypeBuilding<T>.CurrentType;
    Sprite DefaultSprite => MultipleTypeBuildingComponent.DefaultSprite;
    void CycleType() => MultipleTypeBuildingComponent.CycleType();
    void SetType(T type) => MultipleTypeBuildingComponent.SetType(type);
    GameObject[] CreateButtonsForAllTypes() => MultipleTypeBuildingComponent.CreateButtonsForAllTypes();
}

public interface IAnimalHouse{
    AnimalHouse AnimalHouseComponent {get;}
    List<Animals> AnimalsInBuilding => AnimalHouseComponent.AnimalsInBuilding;
    void AddAnimal(Animals animal) => AnimalHouseComponent.AddAnimal(animal);
    void RemoveAnimal(Animals animal) => AnimalHouseComponent.RemoveAnimal(animal);
}
