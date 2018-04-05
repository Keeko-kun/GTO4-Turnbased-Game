using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour {

    public float walkSpeed;
    public Pathfinding pathfinder;
    public LevelPiece currentTile;
    public bool walking;

    private Animator anim;
    private bool isTurning;



    void Start () {
        anim = GetComponent<Animator>();
        isTurning = false;
        walking = false;
	}

    public Vector3 Forward()
    {
        Vector3 forward = transform.forward;
        forward.x = Mathf.Round(forward.x);
        forward.z = Mathf.Round(forward.z);
        return forward;
    }

    public IEnumerator StartMovement(LevelPiece target)
    {
        walking = true;

        List<LevelPiece> path = pathfinder.FindPath(new Node((int)currentTile.PosX, (int)currentTile.PosZ, currentTile.Walkable),
                                            new Node((int)target.PosX, (int)target.PosZ, target.Walkable));
        

        foreach(LevelPiece tile in path)
        {
            LevelPiece currentlyOn = currentTile;

            Vector3 move = new Vector3(tile.PosX * -1 - currentlyOn.PosX * -1, 0, tile.PosZ - currentlyOn.PosZ);
            if (Forward().x == move.x && Forward().z == move.z)
            {
                yield return StartCoroutine(WalkForward(new Vector3(tile.PosX * Globals.spacing * -1, transform.position.y, tile.PosZ * Globals.spacing)));
            }
            else
            {
                Debug.Log(move);
                if (move.x * Forward().z  < 0 || move.z * Forward().x * -1 < 0)
                {
                    yield return StartCoroutine(Turn("turnLeft", -1, 90, Globals.spacing));
                }
                else if (Forward() * -1 == move)
                {
                    yield return StartCoroutine(Turn("turnLeft", -1, 180, Globals.spacing));
                }
                else
                {
                    yield return StartCoroutine(Turn("turnRight", 1, 90, Globals.spacing));
                }


                yield return StartCoroutine(WalkForward(new Vector3(tile.PosX * Globals.spacing * -1, transform.position.y, tile.PosZ * Globals.spacing)));
            }
            currentTile = tile;
        }

        anim.SetBool("canWalk", false);
        walking = false;
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

    IEnumerator Turn(string animation, int direction, int degrees, float speed)
    {
        anim.SetBool(animation, true);
        float currentRotation = transform.rotation.eulerAngles.y;
        float targetRotation = transform.rotation.eulerAngles.y + degrees;
        float actualRotation = transform.rotation.eulerAngles.y + degrees * direction;
        while (currentRotation <= targetRotation)
        {
            transform.Rotate(0, speed * direction, 0);
            currentRotation += speed;
            yield return new WaitForFixedUpdate();
            anim.SetBool(animation, false);
        }

        transform.eulerAngles = new Vector3(0, actualRotation, 0);
    }
}
