using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class NodeMapCamera : MonoBehaviour
{
    [SerializeField] float moveSpeed = 5;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.position += new Vector3(0, 0, InputManager.Instance.MapScroll()) * moveSpeed * Time.deltaTime;
    }
}
