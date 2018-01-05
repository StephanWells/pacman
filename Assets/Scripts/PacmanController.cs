using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PacmanController : MonoBehaviour
{
    enum directions { RIGHT, UP, DOWN, LEFT, STATIONARY };

    public float playerSpeed;
    private Vector2 playerDirection;
    private Animator animator;
    private directions directionState;
    public Node currentNode;
    public Node nextNode;

    void Start()
    {
        directionState = directions.STATIONARY;
        animator = this.GetComponent<Animator>();
        nextNode = null;
    }

    void Update()
    {
        CheckInput();
        //CheckNodes();
        ChangeDirection();
        Move();
    }

    //void CheckNodes()
    //{
    //    if (this.transform.localPosition.Equals(nextNode.transform.localPosition))
    //    {
    //        currentNode = nextNode;
    //    }
    //}

    void ChangeDirection()
    {
        switch (directionState)
        {
            case directions.RIGHT :
                playerDirection = Vector2.right;
                SetAnimatorState("Right");
            break;

            case directions.UP:
                playerDirection = Vector2.up;
                SetAnimatorState("Up");
            break;

            case directions.LEFT:
                playerDirection = Vector2.left;
                SetAnimatorState("Left");
            break;

            case directions.DOWN:
                playerDirection = Vector2.down;
                SetAnimatorState("Down");
            break;

            case directions.STATIONARY:
                playerDirection = Vector2.zero;
            break;
        }
    }

    void CheckInput()
    {
        if (Input.GetKey(KeyCode.RightArrow))
        {
            directionState = directions.RIGHT;
        }
        else if (Input.GetKey(KeyCode.UpArrow))
        {
            directionState = directions.UP;
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            directionState = directions.LEFT;
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            directionState = directions.DOWN;
        }
    }

    void Move()
    {
        transform.localPosition += (Vector3)(playerDirection * playerSpeed) * Time.deltaTime;
    }

    void SetAnimatorState(string state)
    {
        animator.SetBool("Right", false);
        animator.SetBool("Left", false);
        animator.SetBool("Up", false);
        animator.SetBool("Down", false);
        animator.SetBool(state, true);
    }
}
