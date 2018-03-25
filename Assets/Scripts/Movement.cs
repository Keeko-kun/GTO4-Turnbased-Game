using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour {

    public float walkSpeed;
    public Pathfinding pathfinder;
    public LevelPiece currentTile;

    private Animator anim;
    private bool isTurning;



    void Start () {
        anim = GetComponent<Animator>();
        isTurning = false;
	}
	
    public void Move(LevelPiece target)
    {
        List<LevelPiece> path = pathfinder.FindPath(new Node((int)currentTile.PosX, (int)currentTile.PosZ, currentTile.Walkable),
                                                    new Node((int)target.PosX, (int)target.PosZ, target.Walkable));
        Debug.Log(path.Count);
        StartCoroutine(StartMovement(path));
    }

    public Vector3 Forward()
    {
        Vector3 forward = transform.forward;
        forward.x = Mathf.Round(forward.x);
        forward.z = Mathf.Round(forward.z);
        return forward;
    }

    IEnumerator StartMovement(List<LevelPiece> path)
    {
        LevelPiece currentlyOn = currentTile;

        foreach(LevelPiece tile in path)
        {

            Vector3 move = new Vector3(tile.PosX * -1 - currentlyOn.PosX * -1, 0, tile.PosZ - currentlyOn.PosZ);
            if (Forward().x == move.x && Forward().z == move.z)
            {
                yield return StartCoroutine(WalkForward(new Vector3(tile.PosX * 2.5f * -1, transform.position.y, tile.PosZ *2.5f)));
            }
            else
            {
                Debug.Log(move.z);
                if (move.x  < 0 || move.z * Forward().x * -1 < 0)
                {
                    yield return StartCoroutine(Turn("turnLeft", -1));
                }
                else
                {
                    yield return StartCoroutine(Turn("turnRight", 1));
                }
                yield return StartCoroutine(WalkForward(new Vector3(tile.PosX * 2.5f * -1, transform.position.y, tile.PosZ * 2.5f)));
            }
            currentlyOn = tile;
        }
    }

    IEnumerator WalkForward(Vector3 targetPos)
    {
        anim.SetBool("canWalk", true);

        Vector3 currentPos = transform.position;

        while(transform.position.x * Forward().x < targetPos.x * Forward().x  || transform.position.z * Forward().z < targetPos.z * Forward().z)
        {
            transform.Translate(0, 0, walkSpeed / 250);
            yield return new WaitForFixedUpdate();
        }

        transform.position = targetPos;
    }

    IEnumerator Turn(string animation, int direction)
    {
        anim.SetBool(animation, true);
        float currentRotation = transform.rotation.eulerAngles.y;
        float targetRotation = transform.rotation.eulerAngles.y + 90;
        float actualRotation = transform.rotation.eulerAngles.y + 90 * direction;
        while (currentRotation <= targetRotation)
        {
            transform.Rotate(0, 1.75f * direction, 0);
            currentRotation += 1.75f;
            yield return new WaitForFixedUpdate();
            anim.SetBool(animation, false);
        }

        transform.eulerAngles = new Vector3(0, actualRotation, 0);
    }
}
