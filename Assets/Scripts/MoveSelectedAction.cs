using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoveSelectedAction : MonoBehaviour {

    public RectTransform move;
    public RectTransform attack;
    public RectTransform back;
    public RectTransform endTurn;

    public CurrentAction currentAction;

    public RectTransform cursor;

    private void Start()
    {

    }

    private void Update()
    {
        Vector2 velocity = Vector2.zero;

        switch (currentAction)
        {
            case CurrentAction.Move:
                cursor.anchoredPosition = Vector2.SmoothDamp(cursor.anchoredPosition, move.anchoredPosition, ref velocity, .8f, 8, 2);
                cursor.sizeDelta = Vector2.SmoothDamp(cursor.sizeDelta, move.sizeDelta, ref velocity, .8f, 8, 2);
                break;
            case CurrentAction.Attack:
                cursor.anchoredPosition = Vector2.SmoothDamp(cursor.anchoredPosition, attack.anchoredPosition, ref velocity, .8f, 8, 2);
                cursor.sizeDelta = Vector2.SmoothDamp(cursor.sizeDelta, attack.sizeDelta, ref velocity, .8f, 8, 2);
                break;
            case CurrentAction.Back:
                cursor.anchoredPosition = Vector2.SmoothDamp(cursor.anchoredPosition, back.anchoredPosition, ref velocity, .8f, 8, 2);
                cursor.sizeDelta = Vector2.SmoothDamp(cursor.sizeDelta, back.sizeDelta, ref velocity, .8f, 8, 2);
                break;
            case CurrentAction.EndTurn:
                cursor.anchoredPosition = Vector2.SmoothDamp(cursor.anchoredPosition, endTurn.anchoredPosition, ref velocity, .8f, 8, 2);
                cursor.sizeDelta = Vector2.SmoothDamp(cursor.sizeDelta, endTurn.sizeDelta, ref velocity, .8f, 8, 2);
                break;
        }
    }

    private IEnumerator MoveImage(RectTransform newPos)
    {
        



        yield return new WaitForFixedUpdate();
    }
}

public enum CurrentAction
{
    Move,
    Attack,
    Back,
    EndTurn
}
