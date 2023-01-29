using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Control : MonoBehaviour
{
    [SerializeField] float moveSpeed;
    Vector2 movement = new Vector2();


    Rigidbody2D rb;
    Animator anim;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        Player_Move();
    }

    private void Update()
    {
        Get_Input();
        //Swith_Anim();
    }
    

    void Get_Input()
    {
        movement.x = Input.GetAxis("Horizontal");
        movement.y = Input.GetAxis("Vertical");
    }
    void Swith_Anim()
    {
        anim.SetFloat("Speed", movement.magnitude);
        if(movement != Vector2.zero)
        {
            anim.SetFloat("Horizontal", movement.x);
            anim.SetFloat("Vertical", movement.y);
        }
    }

    void Player_Move()
    {
        if (movement.x == 0 || movement.y == 0)
            rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
        else
            rb.MovePosition(rb.position + movement * moveSpeed * 0.71f * Time.fixedDeltaTime);
    }
}
