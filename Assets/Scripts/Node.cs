using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public Node[] neighbouringNodes; // All the neighbouring nodes that Pacman can go to.
    public Vector2[] validDirections; // All the directions Pacman can choose from at the current node.
    public bool isGhostNode;

    void Start ()
    {
        validDirections = new Vector2[neighbouringNodes.Length];

        // Go through each neighbouring node, find the direction Pacman can take to get to it, and add it to validDirections.
        for (int i = 0; i < neighbouringNodes.Length; i++)
        {
            validDirections[i] = (neighbouringNodes[i].transform.localPosition - this.transform.localPosition).normalized;
        }
	}
}