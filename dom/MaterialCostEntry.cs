using System;
using UnityEngine;
using UnityEngine.U2D;

[Serializable]
public class MaterialCostEntry {
    public int amount;
    public Materials name;
    public bool IsSpecial { get { return name == Materials.DummyMaterial; } }

    /// <summary> in case a player need to perform an action rather than give a material, ex. obtaining rarecrows, Greenhouse write a 1 sentence instruction here </summary>
    public string howToGet;

    public string EntryText => IsSpecial ? howToGet : name.ToString();


    public MaterialCostEntry(int amount, Materials name) {
        this.amount = amount;
        howToGet = null;
        this.name = name;
    }

    public MaterialCostEntry(string howToGet) {
        amount = 1;
        name = Materials.DummyMaterial;
        this.howToGet = howToGet;
    }

    public override string ToString() {
        if (IsSpecial) return "SpecialMat";
        else return name.ToString();
    }
}
