using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHalo : MonoBehaviour
{
    [SerializeField]
    private GameObject _player;

    void Update()
    {
        transform.LookAt(_player.transform);
    }
}
