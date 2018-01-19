using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    private Animator animator;
    public enum State { MOVING, STILL, FRIGHTENED, FRIGHTENED2, RECOVERY, RECOVERED, DEAD, ALIVE };

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
        else if (state == State.FRIGHTENED)
        {
            animator.SetBool("Frightened", true);
            animator.SetBool("Frightened 2", false);
            animator.SetBool("Recovery", false);
        }
        else if (state == State.FRIGHTENED2)
        {
            animator.SetBool("Frightened", false);
            animator.SetBool("Frightened 2", true);
            animator.SetBool("Recovery", false);
        }
        else if (state == State.RECOVERY)
        {
            animator.SetBool("Frightened", false);
            animator.SetBool("Frightened 2", false);
            animator.SetBool("Recovery", true);
        }
        else if (state == State.RECOVERED)
        {
            animator.SetBool("Frightened", false);
            animator.SetBool("Frightened 2", false);
            animator.SetBool("Recovery", false);
            state = State.MOVING;
        }
        else if (state == State.DEAD)
        {
            animator.SetBool("Dead", true);
        }
        else if (state == State.ALIVE)
        {
            animator.SetBool("Dead", false);
        }
    }

    void Update ()
    {
		
	}
}