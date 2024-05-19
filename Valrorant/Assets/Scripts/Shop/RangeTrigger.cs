using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Player player = other.GetComponent<Player>();
        if (player == null) return;

        Shop shop = FindObjectOfType<Shop>();
        if (shop == null) return;

        shop._isActive = true;
        player._isActive = true;
    }

    private void OnTriggerExit(Collider other)
    {
        Player player = other.GetComponent<Player>();
        if (player == null) return;

        Shop shop = FindObjectOfType<Shop>();
        if (shop == null) return;

        shop._isActive = false;
        player._isActive = false;
    }
}
