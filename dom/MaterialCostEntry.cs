using System;
using UnityEngine;
using UnityEngine.U2D;

[Serializable]
public class MaterialCostEntry : ICloneable {
    public int amount;
    public Materials materialType;

    /// <summary> in case a player need to perform an action rather than give a material, ex. obtaining rarecrows, Greenhouse write a 1 sentence instruction here </summary>
    public string howToGet;
    public bool IsSpecial { get { return materialType == Materials.DummyMaterial; } }

    public string EntryText => IsSpecial ? howToGet : System.Text.RegularExpressions.Regex.Replace(materialType.ToString(), "(?<!^)([A-Z])", " $1");

    public MaterialCostEntry() { }

    public MaterialCostEntry(int amount, Materials name) {
        this.amount = amount;
        howToGet = null;
        this.materialType = name;
    }

    public MaterialCostEntry(string howToGet) {
        amount = 1;
        materialType = Materials.DummyMaterial;
        this.howToGet = howToGet;
    }

    public override string ToString() {
        if (IsSpecial) return "SpecialMat";
        else return materialType.ToString();
    }

    public object Clone() {
        return new MaterialCostEntry {
            materialType = materialType,
            amount = amount,
            howToGet = howToGet
        };
    }
}
