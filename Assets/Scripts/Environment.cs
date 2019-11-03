//using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Environment : MonoBehaviour
{
    public string boardID;
    //public float learningRate;
    public float discountFactor;
    public float tileDistance = 10;
    public float kActionProb;
    public bool isAgentInGoalState = false;
    public int numEpisodes = 100;
    public Text episodeText;

    public int curEpisode = 0;


    // The states are the different tiles the agent can be on
    private Dictionary<State, Tile> states;
    // The Environment Q Function
    private QFunction qFunction;
    private State startState;

    // Start is called before the first frame update
    void Awake()
    {
        if (PlayerPrefs.HasKey(boardID + "_Tile1"))
        {
            // Load tiles
            // Have key for number of tiles
        }
        else
        {
            SetupNewEnvironment(GetComponentsInChildren<Tile>());
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    void SetupNewEnvironment(Tile[] tiles) {
        SetupNewStates(tiles);
        SetupNewQFunction();
    }

    private void SetupNewStates(Tile[] tiles) {
        states = new Dictionary<State, Tile>();
        bool startStateSet = false;
        foreach (Tile tile in tiles)
        {
            Transform tileTransform = tile.GetComponent<Transform>();
            float[] tilePosition = { tileTransform.localPosition.x, tileTransform.localPosition.y, tileTransform.localPosition.z };
            State tileState = new State(tilePosition);
            if (tile.isStartState)
            {
                startStateSet = true;
                startState = tileState;
            }
            states[tileState] = tile;
        }

        if (!startStateSet) {
            print("ERROR: START STATE NOT FOUND!");
        }
    }

    private void SetupNewQFunction() {
        // Iterate through all states and create Q function entry for each potential action
        // Have to make assumption that all tiles are equally spaced
        qFunction = new QFunction(discountFactor, kActionProb);
        foreach (KeyValuePair<State, Tile> pair in states)
        {
            // Don't add transitions for goal state
            if (pair.Value.isGoal) {
                continue;
            }
            float[] upMove = { pair.Key.position[0] + tileDistance, pair.Key.position[1], pair.Key.position[2] };
            float[] downMove = { pair.Key.position[0] - tileDistance, pair.Key.position[1], pair.Key.position[2] };
            float[] leftMove = { pair.Key.position[0], pair.Key.position[1], pair.Key.position[2] + tileDistance };
            float[] rightMove = { pair.Key.position[0], pair.Key.position[1], pair.Key.position[2] - tileDistance };
            
            // Add action if there is a tile that is walkable by moving up, down, left, or right 
            AddStateIfCan(pair.Key, 
                new AgentAction(AgentAction.Move.Up),
                upMove);
            AddStateIfCan(pair.Key,
                new AgentAction(AgentAction.Move.Down),
                downMove);
            AddStateIfCan(pair.Key,
                new AgentAction(AgentAction.Move.Left),
                leftMove);
            AddStateIfCan(pair.Key,
                new AgentAction(AgentAction.Move.Right),
                rightMove);
        }
    }

    private void AddStateIfCan(State fromState, AgentAction action, float[] position) {
        State toState = new State(position);
        if (states.ContainsKey(toState)) {
            if (states[toState].isWalkable) {
                qFunction.Add(fromState, toState, action);
            }
        }
    }

    public State GetStartState() {
        return startState;
    }

    public StateActionPair GetNextStateActionPair(State fromState) {
        
        StateActionPair pair = qFunction.ProbPickAction(fromState);
        float reward = states[pair.state].reward;
        if (states[pair.state].isGoal)
        {
            Debug.Log("AGENT IN GOAL");
            isAgentInGoalState = true;
            episodeText.text = "Episode: " + ++curEpisode;
        }
        qFunction.UpdateQValue(fromState, pair.state, pair.action, reward);
        return pair;
    }
}
