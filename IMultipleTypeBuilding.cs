using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Not to be confused with ITieredBuilding, this interface is for buildings that have multiple types like floors, fences, scarecrows, etc.
/// </summary>
public interface IMultipleTypeBuilding<T>{
    public T Type {get;}
    public void CycleType();
    public void SetType(T type);
}
