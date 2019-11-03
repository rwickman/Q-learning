using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class  AgentAction : IEquatable<AgentAction>
{
    public enum Move { Up, Down, Left, Right };
    public Move move;
    public AgentAction(Move move) {
        this.move = move;
    }

    public override bool Equals(object obj)
    {
        if (obj is AgentAction)
        {
            return this.Equals((AgentAction)obj);
        }
        return false;
    }

    public bool Equals(AgentAction s)
    {
        return move == s.move;
    }


    public override int GetHashCode()
    {
        return (int)move;
    }

}
