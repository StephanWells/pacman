using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PacmanController : MonoBehaviour
{
    public float playerSpeed;
    private Vector2 direction;
    Animator animator;

    void Start()
    {
        direction = Vector2.zero;
        animator = this.GetComponent<Animator>();
    }

    void Update ()
    {
        CheckInput();

        Move();
    }

    void CheckInput()
    {
        if (Input.GetKey(KeyCode.RightArrow))
        {
            //transform.Translate(Vector2.right * playerSpeed);
            direction = Vector2.right;

            SetState("Right");
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            //transform.Translate(Vector2.left * playerSpeed);
            direction = Vector2.left;

            SetState("Left");
        }
        else if (Input.GetKey(KeyCode.UpArrow))
        {
            //transform.Translate(Vector2.up * playerSpeed);
            direction = Vector2.up;

            SetState("Up");
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            //transform.Translate(Vector2.down * playerSpeed);
            direction = Vector2.down;

            SetState("Down");
        }
    }

    void Move()
    {
        transform.localPosition += (Vector3)(direction * playerSpeed) * Time.deltaTime;
    }

    void SetState(string state)
    {
        animator.SetBool("Right", false);
        animator.SetBool("Left", false);
        animator.SetBool("Up", false);
        animator.SetBool("Down", false);
        animator.SetBool(state, true);
    }
}
