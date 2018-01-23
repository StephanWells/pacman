using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Level
{
    private float pacmanSpeed;

    private float ghostSpeed;
    private float ghostFrightenedTime;
    private float ghostFrightenedSlow;
    private float ghostRecoveryFactor;

    private float blinkyIdle;
    private float pinkyIdle;
    private float inkyIdle;
    private float clydeIdle;

    private float[] scatterTimers;
    private float chaseTime;

    public Level(float pacmanSpeedIn, float ghostSpeedIn, float ghostFrightenedTimeIn, float ghostFrightenedSlowIn, float ghostRecoveryFactorIn, float blinkyIdleIn, float pinkyIdleIn, float inkyIdleIn, float clydeIdleIn, float scatter1, float scatter2, float scatter3, float scatter4, float chase)
    {
        pacmanSpeed = pacmanSpeedIn;

        ghostSpeed = ghostSpeedIn;
        ghostFrightenedTime = ghostFrightenedTimeIn;
        ghostFrightenedSlow = ghostFrightenedSlowIn;
        ghostRecoveryFactor = ghostRecoveryFactorIn;

        blinkyIdle = blinkyIdleIn;
        pinkyIdle = pinkyIdleIn;
        inkyIdle = inkyIdleIn;
        clydeIdle = clydeIdleIn;

        scatterTimers = new float[4];
        scatterTimers[0] = scatter1;
        scatterTimers[1] = scatter2;
        scatterTimers[2] = scatter3;
        scatterTimers[3] = scatter4;
        chaseTime = chase;
    }

    public float GetPacmanSpeed()   {   return pacmanSpeed;    }

    public float GetGhostSpeed()            {   return ghostSpeed;          }
    public float GetGhostFrightenedTime()   {   return ghostFrightenedTime; }
    public float GetGhostFrightenedSlow()   {   return ghostFrightenedSlow; }
    public float GetGhostRecoveryFactor()   {   return ghostRecoveryFactor; }

    private float GetGhostIdleTimer(GhostController.Ghost ghost)
    {
        float idleTimer = 0f;

        switch (ghost)
        {
            case GhostController.Ghost.BLINKY:
                idleTimer = blinkyIdle;
            break;

            case GhostController.Ghost.PINKY:
                idleTimer = pinkyIdle;
            break;

            case GhostController.Ghost.INKY:
                idleTimer = inkyIdle;
            break;

            case GhostController.Ghost.CLYDE:
                idleTimer = clydeIdle;
            break;
        }

        return idleTimer;
    }

    public float[] GetGhostTimers(GhostController.Ghost ghost)
    {
        float[] tempGhostTimers = { GetGhostIdleTimer(ghost), scatterTimers[0], chaseTime, scatterTimers[1], chaseTime, scatterTimers[2], chaseTime, scatterTimers[3] };

        return tempGhostTimers;
    }

    public GhostController.Mode[] GetDefaultGhostModes()
    {
        GhostController.Mode[] tempGhostModes = { GhostController.Mode.IDLE, GhostController.Mode.SCATTER, GhostController.Mode.CHASE, GhostController.Mode.SCATTER, GhostController.Mode.CHASE, GhostController.Mode.SCATTER, GhostController.Mode.CHASE, GhostController.Mode.SCATTER, GhostController.Mode.CHASE };

        return tempGhostModes;
    }
}