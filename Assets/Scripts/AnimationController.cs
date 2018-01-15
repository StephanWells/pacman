using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    private Animator animator;
    public enum State { MOVING, STILL };

    void Start ()
    {
        animator = this.GetComponent<Animator>();
    }

    // Updates the animation based on the direction parameter.
    public void SetAnimatorDirection(Vector2 direction)
    {
        foreach (AnimatorControllerParameter parameter in animator.parameters)
        {
            animator.SetBool(parameter.name, false);
        }

        if (direction == Vector2.right)
        {
            animator.SetBool("Right", true);
        }
        else if (direction == Vector2.left)
        {
            animator.SetBool("Left", true);
        }
        else if (direction == Vector2.up)
        {
            animator.SetBool("Up", true);
        }
        else if (direction == Vector2.down)
        {
            animator.SetBool("Down", true);
        }
        else if (direction == Vector2.zero)
        {
            animator.SetBool("Idle", true);
        }
    }

    // Updates the animation based on the state parameter.
    public void SetAnimatorState(State state)
    {
        if (state == State.MOVING)
        {
            animator.SetBool("Idle", false);
        }
        else if (state == State.STILL)
        {
            animator.SetBool("Idle", true);
        }
    }

    void Update ()
    {
		
	}
}