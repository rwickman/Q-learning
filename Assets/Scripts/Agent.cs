using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Agent : MonoBehaviour
{
    public float moveDistance = 2f;
    public State initialState;
    public State curState;
    public float slowDownTime = 0f;

    private float nextMoveTime = 0f;
    private Environment environment;

    // Start is called before the first frame update
    void Start()
    {
        GameObject environmentObj = GameObject.FindWithTag("Environment");
        if (environmentObj == null)
        {
            print("ENVIRONMENT NOT FOUND");
        }
        else {
            environment = environmentObj.GetComponent<Environment>();
            initialState = curState = environment.GetStartState();
        }
    }

    // Update is called once per frame
    void Update()
    {
        nextMoveTime += Time.deltaTime;
        if (environment.curEpisode < environment.numEpisodes && nextMoveTime >= slowDownTime) {
            nextMoveTime = 0;
            if (!environment.isAgentInGoalState && environment != null)
            {
                print("Getting next action");
                StateActionPair nextStateActionPair = environment.GetNextStateActionPair(curState);
                curState = nextStateActionPair.state;
                Move(nextStateActionPair.action);
            }
            else
            {
                MoveToInitialState();
                environment.isAgentInGoalState = false;
            }
        }
        
    }
  
    public void Move(AgentAction action) {
        if (action.move == AgentAction.Move.Up)
        {
            //transform.position
            gameObject.transform.position = new Vector3(gameObject.transform.position.x + moveDistance, gameObject.transform.position.y, gameObject.transform.position.z);
        }
        else if (action.move == AgentAction.Move.Down)
        {
            gameObject.transform.position = new Vector3(gameObject.transform.position.x - moveDistance, gameObject.transform.position.y, gameObject.transform.position.z);
        }
        else if (action.move == AgentAction.Move.Left)
        {
            gameObject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, gameObject.transform.position.z + moveDistance);
        }
        else if (action.move == AgentAction.Move.Right)
        {
            gameObject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, gameObject.transform.position.z - moveDistance);
        }
        else {
            print("THIS MESSAGE IN MOVE SHOULD NOT BE SHOWN");
        }
    }

    public void MoveToInitialState() {
        gameObject.transform.position = new Vector3(initialState.position[0], gameObject.transform.position.y, initialState.position[0]);
        curState = initialState;
    }
}
