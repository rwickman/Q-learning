using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QFunction
{
    public int minNumberOfVisits = 10;
    // Stores the the maximum discounted cumulative reward that can be achieved starting from State and applying 
    // AgentAction as the first action.
    private Dictionary<State, Dictionary<StateActionPair, float>> QFunctionDic;
    private Dictionary<StateActionPair, int> numVisited;

    //private float learningRate;
    private float discountFactor;
    private float initialQValue;
    private float kActionProb;

    public QFunction(float discountFactor, float kActionProb, float initialQValue = 0f) {
        // this.learningRate = learningRate;
        this.discountFactor = discountFactor;
        this.kActionProb = kActionProb;
        this.initialQValue = initialQValue;
         
        QFunctionDic = new Dictionary<State, Dictionary<StateActionPair, float>>();
        numVisited = new Dictionary<StateActionPair, int>();
    }

    public void Add(State fromState, State toState,  AgentAction action) {
        if (!QFunctionDic.ContainsKey(fromState)) {
            QFunctionDic[fromState] = new Dictionary<StateActionPair, float>();
        }
        QFunctionDic[fromState][new StateActionPair(toState, action)] = initialQValue;
        numVisited[new StateActionPair(fromState, action)] = 0;

    }

    public void UpdateQValue(State fromState, State toState, AgentAction action, float reward) {
        StateActionPair destPair = new StateActionPair(toState, action);
        float learningRate = 1f; //1 / (numVisited[new StateActionPair(fromState, action)]++ + 1);
        QFunctionDic[fromState][destPair] = (1 - learningRate) * QFunctionDic[fromState][destPair] + learningRate * (reward + discountFactor * QMax(toState));
        Debug.Log("(FROM: " + fromState.position[0] + ", " + fromState.position[2] + " TO: " + toState.position[0] + ", " + toState.position[2] + "): " +  QFunctionDic[fromState][destPair]);
    }

    // Get max Q value for given state
    private float QMax(State toState) {
        // If there are not any possible state action transitions than just return 0 reward 
        if (!QFunctionDic.ContainsKey(toState)) {
            return 0;
        }
        Dictionary<StateActionPair, float> actionDic = QFunctionDic[toState];
        float maxReward = 0;
        foreach (KeyValuePair<StateActionPair, float> pair in actionDic) {
            if (pair.Value > maxReward) {
                maxReward = pair.Value;
            }
        }
        return maxReward;
    }

    public StateActionPair ProbPickAction(State fromState)
    {
        // THIS IS MESSED UP BECAUSE YOU ARE NOT USING THE CORRECT STATE SHOULD BE USING FROM STATE AND ACTION IN STATEACTIONPAIR
        Dictionary<StateActionPair, float> actionDic = QFunctionDic[fromState];
        Dictionary<StateActionPair, float> denomDic = new Dictionary<StateActionPair, float>();
        //Dictionary<AgentAction, float> probDic = new Dictionary<AgentAction, float>();
        Dictionary<StateActionPair, float[]> intervalDic = new Dictionary<StateActionPair, float[]>();
        float maxProb = 0f;
        StateActionPair maxPair = null;

        int minVisited = 0;
        bool minVisitedSet = false;
        // Return the MAX
        
        foreach (KeyValuePair<StateActionPair, float> pair in actionDic) {
            if (pair.Value > maxProb) {
                maxProb = pair.Value;
                maxPair = pair.Key;
            }
            StateActionPair fromPair = new StateActionPair(fromState, pair.Key.action);
            if (!minVisitedSet|| numVisited[new StateActionPair(fromState, pair.Key.action)] < minVisited) {
                minVisited = numVisited[fromPair];
                minVisitedSet = true;
            }
        }

        if (minVisited < minNumberOfVisits) {

            float sumOfProb = 0;
            foreach (KeyValuePair<StateActionPair, float> pair in actionDic)
            {
                float denom = Mathf.Pow(kActionProb, pair.Value);
                sumOfProb += denom;
                denomDic.Add(pair.Key, denom);
            }

            // Create intervals for probablities
            float startInterval = 0f;
            // Shuffle as to not be biased towards picking one option
            List<StateActionPair> denomDicKeys = new List<StateActionPair>(denomDic.Keys);
            Shuffle(denomDicKeys);

            foreach (StateActionPair key in denomDicKeys)
            {
                float denom = denomDic[key];
                float prob = denom / sumOfProb;
                float[] interval = { startInterval, startInterval + prob };
                //Debug.Log("(FROM: " + fromState.position[0] + ", " + fromState.position[2] + "): " + key.action.move + ": " + interval[0] + ", " + interval[1]);
                startInterval += prob;
                intervalDic.Add(key, interval);
                if (maxProb <= prob)
                {
                    maxProb = prob;
                    maxPair = key;
                }
            }

            // Pick the max
            float randomProb = Random.Range(0.0f, 1.0f);
            //Debug.Log(randomProb);
            foreach (KeyValuePair<StateActionPair, float[]> pair in intervalDic)
            {
                float[] interval = pair.Value;
                if (randomProb >= interval[0] && randomProb <= interval[1])
                {
                    //Debug.Log("RETURING IN INTERVAL");
                    return pair.Key;
                }
            }

        }

        return maxPair;
    }

    public static void Shuffle<T>(List<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0, n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
}
