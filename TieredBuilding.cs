using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public interface ITieredBuilding{

    int Tier {get;}
    public void ChangeTier(int tier);
}
