using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class aStarPath
{
    LevelPiece[,] map;
    Node[,] nodes;
    List<LevelPiece> path;

    public aStarPath(LevelPiece[,] map)
    {
        this.map = map;
        nodes = new Node[map.GetLength(0), map.GetLength(1)];

        //Construct the nodes!
        for (int i = 0; i < map.GetLength(0); i++)
        {
            for (int j = 0; j < map.GetLength(1); j++)
            {
                Node n = new Node();
                n.Walkable = map[i, j].Walkable;
                n.FCost = 0;
                n.HCost = 0;
                n.GCost = 0;
                n.X = (int)map[i, j].PosX;
                n.Z = (int)map[i, j].PosZ;

                nodes[i, j] = n;
            }
        }
    }

    public List<LevelPiece> GetPath()
    {
        return path;
    }

    public void FindPath(Node start, Node end)
    {
        //Create the open and closed lists.
        List<Node> openList = new List<Node>();
        List<Node> closedList = new List<Node>();
        Node current = null;
        Node previous = null;

        openList.Add(nodes[start.X, start.Z]); //Add the start Node to the open list.

        while (openList.Count > 0) //Keep going until the open list is empty.
        {
            current = openList[0]; //Set the first Node as the current Node. (used if there isn't any other node)
            for (int i = 1; i < openList.Count; i++)
            {
                if (openList[i].FCost < current.FCost ||
                        (openList[i].FCost == current.FCost && openList[i].GCost < current.GCost))
                {
                    current = openList[i]; //Set the Node with the lowest F-Cost as the current Node.
                }
            }

            //Remove the Current Node from the open list and add it to the closed list. (Because we are using it)
            openList.Remove(current);
            closedList.Add(current);

            //Determine if the end has been reached.
            if (current.X == end.X && current.Z == end.Z)
            {
                end.Parent = previous;
                RetracePath(start, end);
                return;
            }

            List<Node> currentNeighbours = new List<Node>(); //Create a list to store the current neighbours of a Node.

            //Add all neighbours to the list, but only if they actually exist.
            if (current.X + 1 < nodes.GetLength(0))
            {
                currentNeighbours.Add((nodes[current.X + 1, current.Z]));
            }
            if (current.X - 1 >= 0)
            {
                currentNeighbours.Add((nodes[current.X - 1, current.Z]));
            }
            if (current.Z + 1 < nodes.GetLength(1))
            {
                currentNeighbours.Add((nodes[current.X, current.Z + 1]));
            }
            if (current.Z - 1 >= 0)
            {
                currentNeighbours.Add((nodes[current.X, current.Z - 1]));
            }

            //Loop trough the neighbours
            foreach (Node neighbour in currentNeighbours)
            {
                //Determine if the neighbour is not walkable or already in the closed list
                if (!neighbour.Walkable || closedList.Contains(neighbour))
                {
                    continue; //Skip this neighbour, we don't need it.
                }

                //Is the current neighbour not in the open list? Yes? Good!
                if (!openList.Contains(neighbour))
                {
                    //Set the Cost values for the neighbour
                    neighbour.GCost = current.GCost + 1;
                    neighbour.HCost = GetDistance(neighbour, nodes[end.X, end.Z]);
                    neighbour.FCost = neighbour.GCost + neighbour.HCost;
                    neighbour.Parent = current;
                    openList.Add(neighbour);
                    nodes[neighbour.X, neighbour.Z] = neighbour;
                }
            }

            previous = current;
        }

    }

    private void RetracePath(Node start, Node end)
    {
        path = new List<LevelPiece>();

        Node current = end;

        while (current.X != start.X || current.Z != start.Z)
        {
            path.Add(map[current.X, current.Z]);
            current = current.Parent;
        }

        //path.Add(map[start.X, start.Z]);

        path.Reverse();

    }

    private int GetDistance(Node a, Node b)
    {
        int distanceX = Math.Abs(a.X - b.X);
        int distanceZ = Math.Abs(a.Z - b.Z);

        return distanceX + distanceZ;
    }
}