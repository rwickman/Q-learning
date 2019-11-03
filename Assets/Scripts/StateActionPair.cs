using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateActionPair : IEquatable<StateActionPair>
{
    public State state;
    public AgentAction action;

    public StateActionPair(State state, AgentAction action)
    {
        this.state = state;
        this.action = action;
    }
    public override bool Equals(object obj)
    {
        if (obj is StateActionPair)
        {
            return this.Equals((StateActionPair)obj);
        }
        return false;
    }

    public bool Equals(StateActionPair s)
    {
        return state.Equals(s.state) && action.Equals(s.action);
    }


    public override int GetHashCode()
    {
        return state.GetHashCode() ^ (int)action.move;
    }
}