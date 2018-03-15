using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class Node
{
    public int HCost { get; set; }
    public int GCost { get; set; }
    public int FCost { get; set; }
    public int X { get; set; }
    public int Z { get; set; }
    public bool Walkable { get; set; }
    public Node Parent { get; set; }

    public Node()
    {

    }

}

