using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //private CharacterController charController;
    private Rigidbody2D charRigid;

    public float moveSpeed = 10f;
    public float xInput;
    public float yInput;

    private void Start() {
        //charController = GetComponent<CharacterController>();
        charRigid = GetComponent<Rigidbody2D>();
    }

    private void Update() {
        xInput = Input.GetAxisRaw("Horizontal");
        yInput = Input.GetAxisRaw("Vertical");
        Vector2 moveDir = new Vector2(xInput, yInput).normalized;

        charRigid.AddForce(moveDir);
        //charController.Move(moveDir * moveSpeed * Time.deltaTime);
    }
}
