using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (!other.CompareTag("Player"))
            return;

        var mov_script = other.GetComponent<movement>();
        mov_script.charge_fuel();

        Destroy(this.gameObject);
    }
}
