using UnityEngine;
using UnityEngine.U2D;

public class MaterialCostEntry {
    public int amount;
    public readonly Materials? name;
    public bool IsSpecial { get { return name == null; } }

    /// <summary> in case a player need to perform an action rather than give a material, ex. obtaining rarecrows, Greenhouse write a 1 sentence instruction here </summary>
    public readonly string howToGet;

    public string EntryText => IsSpecial ? howToGet : name.ToString();


    public MaterialCostEntry(int amount, Materials name) {
        this.amount = amount;
        howToGet = null;
        this.name = name;
    }

    public MaterialCostEntry(string howToGet) {
        amount = 1;
        name = null;
        this.howToGet = howToGet;
    }

    public override string ToString() {
        if (IsSpecial) return "SpecialMat";
        else return name.ToString();
    }
}
