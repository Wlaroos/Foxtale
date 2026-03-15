using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cleanup : MonoBehaviour
{

    private void Awake()
    {
        Invoke(nameof(Delt),2f);
    }

    private void Delt()
    {
        Destroy(this.gameObject);
    }
}
