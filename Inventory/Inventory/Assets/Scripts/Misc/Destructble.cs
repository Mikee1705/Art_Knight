using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructble : MonoBehaviour
{
    [SerializeField] private GameObject destroyVFX;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.GetComponent<DamageSource>())
        {
            Instantiate(destroyVFX, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
