using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class inControl : MonoBehaviour
{

    public void GainControl() {
        GameManager.Instance.isInControl = true;
    }


}
