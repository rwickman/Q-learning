using System;

public class State : IEquatable<State>
{
    public float[] position;
    public State(float[] position)
    {
        this.position = position;
    }
    public override bool Equals(object obj)
    {
        if (obj is State)
        {
            return this.Equals((State)obj);
        }
        return false;
    }

    public bool Equals(State s)
    {
        return (s.position[0] == position[0]) && (s.position[1] == position[1]) && (s.position[2] == position[2]);
    }


    public override int GetHashCode()
    {
        return (int)position[0] ^ (int)position[2];
    }

}
