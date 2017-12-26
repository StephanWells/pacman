using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PacmanController : MonoBehaviour
{
    public float playerSpeed;

	void Update ()
    {
		if (Input.GetKey(KeyCode.RightArrow))
        {
            transform.Translate(Vector2.right * playerSpeed);
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            transform.Translate(Vector2.left * playerSpeed);
        }
        else if (Input.GetKey(KeyCode.UpArrow))
        {
            transform.Translate(Vector2.up * playerSpeed);
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            transform.Translate(Vector2.down * playerSpeed);
        }
    }
}
