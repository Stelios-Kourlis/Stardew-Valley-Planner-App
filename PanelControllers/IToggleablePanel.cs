using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IToggleablePanel{
    static float MOVE_SCALE = 1250f;
    public bool IsMoving {get;}
    public bool IsOpen {get;}
    void TogglePanel();
    IEnumerator OpenPanel();
    IEnumerator ClosePanel();
}
