using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITieredBuilding{

    int Tier {get;}
    public void ChangeTier(int tier);
}
